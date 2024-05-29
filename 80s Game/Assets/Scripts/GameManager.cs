using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static Action roundOverObservers;
    // Singleton pattern
    public static GameManager Instance { get; private set; }
    public PlayerController playerPrefab;

    public AudioClip roundEndTheme;     //putting this here so it can be called by GameMode subclasses
    //public AudioClip failstateTheme;  //for future implementation of losable game mode(s), e.g. defense mode

    // Change in Inspector
    public EGameMode gameModeType = EGameMode.Classic;

    public AbsGameMode ActiveGameMode { get; private set; }
    // Public properties
    public InputManager InputManager { get; private set;  }
    public TargetManager TargetManager { get; private set; }
    public PointsManager PointsManager { get; private set; }

    public UIManager UIManager { get; private set; }
    
    [Tooltip("Debug to spawn a Player Controller for testing without having to go through the join screen")]
    public bool debug;

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
            UIManager = GetComponent<UIManager>();

            if (PlayerData.activePlayers.Count == 0 && debug) {
                float sensitivity = PlayerPrefs.GetFloat("Sensitivity", 1.0f);

                PlayerConfig defaultConfig = new PlayerConfig(0, PlayerData.defaultColors[0], new Vector2(sensitivity, sensitivity));
                PlayerData.activePlayers.Add(defaultConfig);
                PlayerController pc = Instantiate(playerPrefab, transform.position, Quaternion.identity);
                pc.SetConfig(defaultConfig, PlayerController.ControllerState.Gameplay);
            } else
            {
                for(int i = 0; i < PlayerData.activePlayers.Count; i++)
                {
                    PlayerController pc = Instantiate(playerPrefab, transform.position, Quaternion.identity);
                    pc.SetConfig(PlayerData.activePlayers[i], PlayerController.ControllerState.Gameplay);
                    PlayerInput pi = pc.GetComponent<PlayerInput>();
                    InputDevice[] devices = new InputDevice[] { PlayerData.activePlayers[i].device };
                    pi.SwitchCurrentControlScheme(PlayerData.activePlayers[i].controlScheme, devices);
                }
            }
        }
    }

    private void Start() 
    {
        InitGameMode(gameModeType);
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
            default:
                break;
        }
    }

    public static void EmitRoundOverEvent()
    {
        roundOverObservers?.Invoke();
    }
}