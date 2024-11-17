using SteamIntegration;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static Action roundOverObservers;
    // Singleton pattern
    public static GameManager Instance { get; private set; }
    public PlayerController playerPrefab;

    public MusicTrack roundEndTheme;     //putting this here so it can be called by GameMode subclasses
    //public AudioClip failstateTheme;  //for future implementation of losable game mode(s), e.g. defense mode

    // Change in Inspector
    public EGameMode gameModeType = EGameMode.Classic;

    public AbsGameMode ActiveGameMode { get; private set; }
    // Public properties
    public InputManager InputManager { get; private set;  }
    public TargetManager TargetManager { get; private set; }
    public PointsManager PointsManager { get; private set; }

    public SteamInterface SteamInterface { get; private set; }

    public UIManager UIManager { get; private set; }
    public bool isSlowed = false;

    public int rustedWingsStack = 0;

    public bool debuffActive = false;
    public List<GameObject> buffs;
    public List<GameObject> debuffs;
    public List<GameObject> selfDebuffs;
    public ModifierWeightsConfig weightConfig;
    public OverheatParameters overheatParams;

    List<GameObject> debugBuffs;
    List<GameObject> debugDebuffs;
    
    [Tooltip("Debug to spawn a Player Controller for testing without having to go through the join screen")]
    public bool debug;
    private float gameStartTime;
    private List<PlayerController> players;

    public SpawnRateConfig spawnConfig;


    // Kinematic Movement Flywheel

    [Range(.1f, 2.0f)]
    public float maxForce = .25f;

    private void Awake()
    {
        // Check if the static reference matches the script instance
        if(Instance != null && Instance != this)
        {
            // If not, then the script is a duplicate and can delete itself
            Destroy(this);
        }
        else
        {
            Instance = this;

            // Initialize managers
            InputManager = GetComponent<InputManager>();
            TargetManager = GetComponent<TargetManager>();
            PointsManager = GetComponent<PointsManager>();
            SteamInterface = GetComponent<SteamInterface>();
            UIManager = GetComponent<UIManager>();
            players = new List<PlayerController>();

            if (PlayerData.activePlayers.Count == 0 && debug) {
                float sensitivity = PlayerPrefs.GetFloat("Sensitivity", 3.25f);

                /*
                PlayerConfig defaultConfig = new PlayerConfig(0, PlayerData.defaultColors[0], new Vector2(sensitivity, sensitivity));
                PlayerData.activePlayers.Add(defaultConfig);
                PlayerController pc = Instantiate(playerPrefab, transform.position, Quaternion.identity);
                pc.SetConfig(defaultConfig, PlayerController.ControllerState.Gameplay);
                PlayerInput pi = pc.GetComponent<PlayerInput>();
                pi.SwitchCurrentControlScheme(pi.currentControlScheme, pi.devices[0]);
                */
            }
            else
            {
                // Main Gameplay Player Instantiation
                for(int i = 0; i < PlayerData.activePlayers.Count; i++)
                {
                    PlayerController pc = Instantiate(playerPrefab, transform.position, Quaternion.identity);
                    pc.SetOverheatParameters(overheatParams);
                    pc.SetConfig(PlayerData.activePlayers[i], PlayerController.ControllerState.Gameplay);
                    PlayerInput pi = pc.GetComponent<PlayerInput>();
                    if (PlayerData.activePlayers[i].controlScheme == "KnM")
                    {
                        InputDevice[] devices = new InputDevice[2];
                        devices[0] = PlayerData.activePlayers[i].device;
                        if (devices[0] == Mouse.current)
                        {
                            devices[1] = Keyboard.current;
                        } else
                        {
                            devices[1] = Mouse.current;
                        }
                        pi.SwitchCurrentControlScheme(PlayerData.activePlayers[i].controlScheme, devices);
                    } else
                    {
                        InputDevice[] devices = new InputDevice[] { PlayerData.activePlayers[i].device };
                        pi.SwitchCurrentControlScheme(PlayerData.activePlayers[i].controlScheme, devices);
                    }
                    
                    players.Add(pc);
                }
            }
        }

        debugBuffs = buffs;
        debugDebuffs = debuffs;
    }

    private void Start() 
    {
        InitGameMode(gameModeType);
    }

    public void UpdateModifiers()
    {
        debugBuffs = new List<GameObject>();
        foreach (GameObject buff in buffs)
        {
            AbsModifierEffect ame = buff.GetComponent<AbsModifierEffect>();
            if (ActiveGameMode.isModifierAllowed(ame.GetModType()))
            {
                debugBuffs.Add(buff);
            }
        }

        debugDebuffs = new List<GameObject>();
        foreach (GameObject debuff in debuffs)
        {
            AbsModifierEffect ame = debuff.GetComponent<AbsModifierEffect>();
            if (ActiveGameMode.isModifierAllowed(ame.GetModType()))
            {
                debugDebuffs.Add(debuff);
            }
        }
    }

    public List<GameObject> GetBuffs()
    {
        return debugBuffs;
    }

    public List<GameObject> GetDebuffs()
    {
        return debugDebuffs;
    }

    public void InitGameMode(EGameMode gameModeType)
    {
        switch(gameModeType)
        {
            case EGameMode.Competitive:
                ActiveGameMode = new CompetitiveMode();
                break;
            case EGameMode.Classic:
                ActiveGameMode = new ClassicMode();
                break;
            case EGameMode.Defense:
                ActiveGameMode = new CooperativeMode();
                break;
            default:
                break;
        }
        Scene activeScene = SceneManager.GetActiveScene();
        if (activeScene.buildIndex > 1)
        {
            if(ActiveGameMode != null)
            {
                ActiveGameMode.StartGame();
                gameStartTime = Time.time;
            }
        }
    }

    public static void EmitRoundOverEvent()
    {
        roundOverObservers?.Invoke();
        GameManager.Instance.PointsManager.ResetRoundPoints();
    }

    /// <summary>
    /// Function called by a game mode when the last round is exceeded.
    /// Saves play data
    /// </summary>
    public void HandleGameOver()
    {
        // Overall Game Data
        SaveDataItem dataToSave = new SaveDataItem();
        dataToSave.gameMode = GameModeData.GameModeToString();
        dataToSave.batsShot = new BatData(TargetManager.killCount);
        dataToSave.batsSpawned = new BatData(TargetManager.spawnCount);
        dataToSave.duration = (int)(Time.time - gameStartTime);
        dataToSave.playerCount = PlayerData.activePlayers.Count;
        dataToSave.playerData = new PlayerSaveData[players.Count];

        // Per Player data. Not a fan of how this works.
        // This loop requires strong coupling between this class and the active players,
        // which have no reason to know about each other until now. Might turn this around in a refactor
        for (int i = 0; i < players.Count; i++)
        {
            PlayerSaveData playerData = new PlayerSaveData();
            PlayerConfig config = PlayerData.activePlayers[players[i].Order];
            playerData.shotsFired = players[i].GetShotsFired();
            playerData.shotsHit = players[i].GetShotsLanded();
            playerData.device = config.controlScheme;
            playerData.deviceMake = config.device.name;
            playerData.crossHairColor = new float[] { config.crossHairColor.r, config.crossHairColor.g, config.crossHairColor.b };
            playerData.crossHairIndex = config.crossHairIndex;
            playerData.sensitivity = new SensitivityData(config.sensitivity.x, config.sensitivity.y);
            playerData.modifiersCollected = new ModifierData(players[i].modifierCounter);
            playerData.score = PointsManager.TotalPointsByPlayer[i];
            dataToSave.playerData[i] = playerData;
        }
        DataSaver saver = DataSaveFactory.MakeSaver(true);
        StartCoroutine(saver.Save(dataToSave));
    }

    /// <summary>
    /// Returns number of players in the game
    /// </summary>
    public int GetPlayerCount()
    {
        return players.Count;
    }

    public PlayerController GetPlayer(int index)
    {
        return players[index]; 
    }
    /// <summary>
    /// Calls upon active game mode's coroutine function for starting delayed rounds
    /// </summary>
    public void StartRoundDelay()
    {
        float delay = roundEndTheme.Clip.length - (float)roundEndTheme.EndOffset;
        StartCoroutine(ActiveGameMode.DelayNextRound(delay));
    }

    /// <summary>
    /// General call for debug lines for non-monobehavior scripts
    /// </summary>
    public void AddDebugMessage(string msg)
    {
        Debug.Log(msg);
    }

    private void OnPlayerJoined(PlayerInput playerInput)
    {
        Debug.Log(playerInput.currentActionMap.name);
        if (PlayerData.activePlayers.Count == 0 && debug)
        {
            float sensitivity = PlayerPrefs.GetFloat("Sensitivity", 3.25f);
            PlayerConfig defaultConfig = new PlayerConfig(0, PlayerData.defaultColors[0], new Vector2(sensitivity, sensitivity));
            defaultConfig.controlScheme = playerInput.currentControlScheme;
            defaultConfig.device = playerInput.devices[0];
            PlayerData.activePlayers.Add(defaultConfig);
        }
    }
}