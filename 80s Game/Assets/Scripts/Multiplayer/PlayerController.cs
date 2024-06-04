using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

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
    
    public int Order { get; private set; }

    private PlayerConfig config;

    [SerializeField]
    private Crosshair crosshairPrefab;
    private Crosshair activeCrosshair;

    private void Awake()
    {
        GameManager.roundOverObservers += ReportEndOfRound;
        scoreController = new PlayerScoreController();
        activeCrosshair = Instantiate(crosshairPrefab, new Vector3(0,0,0), Quaternion.identity);
        if (config != null)
        {
            activeCrosshair.ChangeSpriteColor(config.crossHairColor);
            
        } else
        {
            Order = PlayerData.activePlayers.Count;
        }
        
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
        }
        currentState = controllerState;
    }

    /// <summary>
    /// Logic for handling the fire input message when received from the PlayerInputWrapper
    /// </summary>
    public void HandleFire()
    {
        // If the onboarding screen is on, dismiss it
        if (GameManager.Instance.UIManager.activeUI != UIManager.UIType.None)
        {
            GameManager.Instance.UIManager.GetFireInput(activeCrosshair.PositionToScreen());
            return;
        }

        // Play the shoot sound if the game is not paused
        if (Time.timeScale > 0)
        {
            SoundManager.Instance.PlaySoundContinuous(shootSound);
        }

        // Relay a shot information message to the Input Manager which acts as a publisher
        ShotInformation s = new(activeCrosshair.transform.position, this);
        InputManager.PlayerShot(s);
        scoreController.AddShot();
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
        // The JoinScreen uses this event to launch the game if all players are ready
        if (currentState == ControllerState.JoinScreen)
        {
            pjm.LaunchGameMode();
            return;
        }
        UIManager.PlayerPause(Order);
    }

    public void EmitCancel()
    {
        if (currentState == ControllerState.JoinScreen)
        {
            pjm.BackOut();
        }
    }

    /// <summary>
    /// Send the PointsManager bonus point information at the end of every round of gameplay
    /// </summary>
    public void ReportEndOfRound()
    {
        GameManager.Instance.PointsManager.AddBonusPoints(Order, scoreController.GetAccuracy());
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

}

public struct ShotInformation
{
    public Vector3 location;
    public PlayerController player;
    public ShotInformation(Vector3 l, PlayerController p)
    {
        location = l;
        player = p;
    }
}