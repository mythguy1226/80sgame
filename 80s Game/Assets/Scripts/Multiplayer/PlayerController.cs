using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
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
        Order = PlayerData.activePlayers.Count;
        activeCrosshair = Instantiate(crosshairPrefab, new Vector3(0,0,0), Quaternion.identity);
        activeCrosshair.ChangeSpriteColor(config.crossHairColor);
    }

    public void HandleMovement(Vector2 movementData)
    {

        activeCrosshair.SetMovementDelta(movementData);
    }

    public void SetConfig(PlayerConfig pc)
    {
        Order = pc.playerIndex;
        config = pc;
        if (activeCrosshair != null)
        {
            activeCrosshair.ChangeSpriteColor(pc.crossHairColor);
        }
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
            SoundManager.Instance.PlaySoundContinuous(shootSound, 0.5f);
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
        UIManager.PlayerPause();
    }

    public void ReportEndOfRound()
    {
        GameManager.Instance.PointsManager.AddBonusPoints(Order, scoreController.GetAccuracy());
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