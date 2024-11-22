using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerController : MonoBehaviour
{
    // This class is the public facing class for player logic
    // It controls the effect of input events and exposes the scoring system for each player
    // It also exposes the presentation layer of the player to the code but all modifications to it must happen
    // through this controller

    public enum ControllerState
    {
        JoinScreen,
        Gameplay
    };

    private ControllerState currentState;
    private PlayerJoinManager pjm;

    [SerializeField]
    private AudioClip shootSound;

    public PlayerScoreController scoreController;
    public float originalShotRadius = 0.3f;
    private float modifiedShotRadius;

    private ModDoublePoints doublePoints;
    private ModOvercharge overchargeMod;
    private ModRustedWings rustedWings;
    private ModConfusion confusion;
    private ModSnail snail;

    public Dictionary<AbsModifierEffect.ModType, int> modifierCounter;

    public int Order { get; private set; }
    public GameObject hitRadius;
    public GameObject hitParticles;
    public GameObject defaultHitParticles;

    private PlayerConfig config;


    public float overheatMax;
    public float maxShootOverheat;
    public float heatAdd;
    public float heatRemove;
    private Overheat _overheat;

    [SerializeField]
    private Crosshair crosshairPrefab;
    public Crosshair activeCrosshair;

    private void Awake()
    {
        modifierCounter = new Dictionary<AbsModifierEffect.ModType, int>();
        GameManager.roundOverObservers += ReportEndOfRound;
        scoreController = new PlayerScoreController();
        activeCrosshair = Instantiate(crosshairPrefab, new Vector3(0,0,0), Quaternion.identity);
        if (config != null)
        {
            activeCrosshair.ChangeSpriteColor(config.crossHairColor);
            hitParticles = config.stunParticles;
        } else
        {
            Order = PlayerData.activePlayers.Count;
            hitParticles = defaultHitParticles;
        }
        modifiedShotRadius = originalShotRadius;
        _overheat = new Overheat(maxShootOverheat, overheatMax, heatRemove, heatAdd);
    }

    /// <summary>
    /// What happens with movement inputs
    /// </summary>
    /// /// <param name="movementData">Vector2 that holds movement direction information</param>
    public void HandleMovement(Vector2 movementData)
    {

        activeCrosshair.SetMovementDelta(movementData);
    }

    /// <summary>
    /// Set the configuration for this player controller
    /// </summary>
    /// <param name="pc">The PlayerConfiguration object that informs this controller</param>
    /// <param name="controllerState">ControllerState enum value that indicates what state the controller is in</param>
    public void SetConfig(PlayerConfig pc, ControllerState controllerState)
    {
        Order = pc.playerIndex;
        config = pc;
        if (activeCrosshair != null)
        {
            activeCrosshair.SetCrosshairSprite(pc.crosshairSprite);
            activeCrosshair.ChangeSpriteColor(pc.crossHairColor);

            if (pc.stunParticles != null)
            {
                defaultHitParticles = pc.stunParticles;
                hitParticles = pc.stunParticles;
            }

            //Set initials under crosshair only in Competitive and Defense mode
            if (GameManager.Instance.gameModeType == EGameMode.Competitive || GameManager.Instance.gameModeType == EGameMode.Defense)
            {
                //Set initials under crosshair to AAA if initials haven't been changed
                if (config.initials == null)
                {
                    switch (Order)
                    {
                        case 0:
                            config.initials = "AAA";
                            break;
                        case 1:
                            config.initials = "BBB";
                            break;
                        case 2:
                            config.initials = "CCC";
                            break;
                        case 3:
                            config.initials = "DDD";
                            break;
                    }

                    PlayerData.activePlayers[Order].initials = config.initials;
                }

                activeCrosshair.gameObject.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = config.initials;
            } else
            {
                if (config.initials == null)
                {
                    config.initials = "AAA";
                }
            }
        }
        currentState = controllerState;
    }

    /// <summary>
    /// Logic for handling the fire input message when received from the PlayerInputWrapper
    /// </summary>
    public void HandleFire()
    {
        // Play the shoot sound and animation if the game is not paused 
        if (Time.timeScale > 0 && activeCrosshair.gameObject.activeInHierarchy)
        {

            // Overheat interferes here
            if (!_overheat.CanShoot())
            {
                return;
            }

            // SoundManager.Instance.PlaySoundContinuous(shootSound);
            SoundManager.Instance.PlaySoundContPitched(shootSound, 0.8f, 1.2f);
            GameObject hr = Instantiate(hitRadius, activeCrosshair.transform.position, Quaternion.identity);
            hr.transform.localScale *= modifiedShotRadius / originalShotRadius;
            EndAndDie ed = hr.GetComponent<EndAndDie>();
            ed.radius = modifiedShotRadius;

            GameObject hitVFX = Instantiate(hitParticles, activeCrosshair.transform.position, Quaternion.identity);
            Destroy(hitVFX, 0.6f);

            // Leave this here, or it messes up shot/hit recording order
            scoreController.AddShot();
        }

        
        // Relay a shot information message to the Input Manager which acts as a publisher
        _overheat.AddHeat();
        ShotInformation s = new(activeCrosshair.transform.position, this, modifiedShotRadius != originalShotRadius);
        InputManager.PlayerShot(s);

        if (_overheat.IsVenting())
        {
            activeCrosshair.EnableVentEffects();
        }
    }

    private void Update()
    {
        _overheat.ReduceHeat(Time.deltaTime);
        activeCrosshair.overheatUI.fillAmount = _overheat.GetHeatProportion();
        if (!_overheat.IsVenting())
        {
            activeCrosshair.StopVentEffects();
        }
    }

    /// <summary>
    /// Recenter the cursor when the event to do so comes from the PlayerInputWrapper.
    /// This is legacy code - meant to support Joycons.
    /// </summary>
    public void RecenterCursor()
    {
        activeCrosshair.Center();
    }

    /// <summary>
    /// Emit a pause event for the UI Manager to handle.
    /// </summary>
    public void EmitPause()
    {
        // The JoinScreen uses this event to ready up the player
        if (currentState == ControllerState.JoinScreen)
        {
            pjm.joinPanelContainer.transform.GetChild(Order).GetComponent<PlayerJoinPanel>().ReadyUp();
            return;
        }

        if (GameManager.Instance.UIManager.onboardingUI != null && GameManager.Instance.UIManager.onboardingUI.onboardingPanel.activeInHierarchy)
            return;

        UIManager.PlayerPause(Order);
    }

    /// <summary>
    /// Emit a cancel event
    /// </summary>
    public void EmitCancel()
    {
        GameManager.Instance.UIManager.CancelMenu();
        if (currentState == ControllerState.JoinScreen)
        {
            pjm.BackOut(Order);
        }
    }

    private void OnDisable()
    {
        GameManager.roundOverObservers -= ReportEndOfRound;
    }

    /// <summary>
    /// Send the PointsManager bonus point information at the end of every round of gameplay
    /// </summary>
    public void ReportEndOfRound()
    {
        GameManager.Instance.PointsManager.AddBonusPoints(Order, scoreController.GetAccuracy());
    }

    /// <summary>
    /// Exposes the total shots fired managed by the scoreController
    /// </summary>
    /// <returns>Total Shots Fired</returns>
    public int GetShotsFired()
    {
        return scoreController.ShotsFired;
    }


    /// <summary>
    /// Exposes the total shots landed managed by the scoreController
    /// </summary>
    /// <returns>Total shots landed</returns>
    public int GetShotsLanded()
    {
        return scoreController.ShotsLanded;
    }


    /// <summary>
    /// Set which object is the join manager.
    /// </summary>
    /// <param name="manager">A PlayerJoinManager object</param>
    public void SetJoinManager(PlayerJoinManager manager)
    {
        pjm = manager;
    }

    /// <summary>
    /// Get the Crosshair sprite image
    /// </summary>
    public Sprite GetCrosshairSprite()
    {
        return crosshairPrefab.GetCrosshairSprite();
    }


   
    /// <summary>
    /// Expand this player's hit radius
    /// </summary>
    public void ExpandRadius()
    {
        modifiedShotRadius = originalShotRadius * 2.0f;
        _overheat.OverchargeHeatAdd();
    }

    /// <summary>
    /// Reset this player's hit radius
    /// </summary>
    public void ResetRadius()
    {
        modifiedShotRadius = originalShotRadius;
        overchargeMod = null;
        _overheat.ResetHeadAdd();
    }

    /// <summary>
    /// Get this player modified radius value
    /// </summary>
    /// <returns></returns>
    public float GetShotRadius()
    {
        return modifiedShotRadius;
    }

    /// <summary>
    /// Test that the player currently has a type of modifier active
    /// </summary>
    /// <param name="type">The type to test</param>
    /// <returns>Whether or not the type tested is active</returns>
    public bool HasMod(AbsModifierEffect.ModType type)
    {
        switch (type)
        {
            case AbsModifierEffect.ModType.Overcharge:
                return overchargeMod != null;
            case AbsModifierEffect.ModType.DoublePoints:
                return doublePoints != null;
            case AbsModifierEffect.ModType.RustedWings:
                return rustedWings != null;
            case AbsModifierEffect.ModType.Confusion:
                return confusion != null;
            case AbsModifierEffect.ModType.Snail:
                return snail != null;
            default:
                return false;
        }
    }

    /// <summary>
    /// Add modifier to the count for data tracking
    /// </summary>
    /// <param name="type">The type of modifier to add</param>
    public void AddModToCount(AbsModifierEffect.ModType type)
    {
        if (!modifierCounter.ContainsKey(type))
        {
            modifierCounter[type] = 0;
        }
        modifierCounter[type] += 1;
    }

    /// <summary>
    /// Set a modifier in this player controller
    /// </summary>
    /// <param name="type">The type of modifier being set</param>
    /// <param name="effect">The effect object as a reference </param>
    public void SetMod(AbsModifierEffect.ModType type, AbsModifierEffect effect)
    {
        AddModToCount(type);
        switch (type)
        {
            case AbsModifierEffect.ModType.Overcharge:
                overchargeMod = (ModOvercharge) effect;
                break;
            case AbsModifierEffect.ModType.DoublePoints:
                doublePoints = (ModDoublePoints) effect;
                break;
            case AbsModifierEffect.ModType.RustedWings:
                rustedWings = (ModRustedWings) effect;
                break;
            case AbsModifierEffect.ModType.Confusion:
                confusion = (ModConfusion) effect;
                break;
            case AbsModifierEffect.ModType.Snail:
                snail = (ModSnail) effect;
                break;
        }
    }

    /// <summary>
    /// Extend an owned mod's duration
    /// </summary>
    /// <param name="type">Which mod to extend</param>
    /// <param name="duration">By how much</param>
    public void ExtendModDuration(AbsModifierEffect.ModType type, float duration)
    {
        switch (type)
        {
            case AbsModifierEffect.ModType.Overcharge:
                overchargeMod.AddDuration(duration);
                break;
            case AbsModifierEffect.ModType.DoublePoints:
                doublePoints.AddDuration(duration);
                break;
            case AbsModifierEffect.ModType.RustedWings:
                rustedWings.AddDuration(duration);
                break;
            case AbsModifierEffect.ModType.Confusion:
                confusion.AddDuration(duration);
                break;
            case AbsModifierEffect.ModType.Snail:
                snail.AddDuration(duration);
                break;
        }
    }

    /// <summary>
    /// Remove a reference to a mod, usually because it has expired
    /// </summary>
    /// <param name="type">Which mod to de-reference</param>
    public void RemoveMod(AbsModifierEffect.ModType type)
    {
        switch (type)
        {
            case AbsModifierEffect.ModType.Overcharge:
                overchargeMod = null;
                break;
            case AbsModifierEffect.ModType.DoublePoints:
                doublePoints = null;
                break;
            case AbsModifierEffect.ModType.RustedWings:
                rustedWings = null;
                break;
            case AbsModifierEffect.ModType.Confusion:
                confusion = null;
                break;
            case AbsModifierEffect.ModType.Snail:
                snail = null;
                break;
        }
    }

    public void SetOverheatParameters(OverheatParameters ovh)
    {
        if (ovh == null) return;
        _overheat = new Overheat(ovh.maxShootOverheat, ovh.overheatMax, ovh.heatRemove, ovh.heatAdd);
        maxShootOverheat = ovh.maxShootOverheat;
        overheatMax = ovh.overheatMax;
        heatRemove = ovh.heatRemove;
        heatAdd = ovh.heatAdd;
    }
}

public struct ShotInformation
{
    public Vector3 location;
    public PlayerController player;
    public bool isRadiusCheck;
    public ShotInformation(Vector3 l, PlayerController p, bool radiusCheck)
    {
        location = l;
        player = p;
        isRadiusCheck = radiusCheck;
    }
}