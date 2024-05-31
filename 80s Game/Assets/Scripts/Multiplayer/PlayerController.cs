using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
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

    public void HandleMovement(Vector2 movementData)
    {

        activeCrosshair.SetMovementDelta(movementData);
    }

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

    public void HandleFire()
    {

        if (GameManager.Instance.UIManager.activeUI != UIManager.UIType.None)
        {
            GameManager.Instance.UIManager.GetFireInput(activeCrosshair.PositionToScreen());
            return;
        }

        if (Time.timeScale > 0)
        {
            SoundManager.Instance.PlaySoundContinuous(shootSound);
        }

        ShotInformation s = new(activeCrosshair.transform.position, this);
        InputManager.PlayerShot(s);
        scoreController.AddShot();
    }

    public void RecenterCursor()
    {
        activeCrosshair.Center();
    }

    public void EmitPause()
    {

        if (currentState == ControllerState.JoinScreen)
        {
            pjm.LaunchGameMode();
            return;
        }
        UIManager.PlayerPause(Order);
    }

    public void ReportEndOfRound()
    {
        GameManager.Instance.PointsManager.AddBonusPoints(Order, scoreController.GetAccuracy());
    }

    public void SetJoinManager(PlayerJoinManager manager)
    {
        pjm = manager;
    }

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