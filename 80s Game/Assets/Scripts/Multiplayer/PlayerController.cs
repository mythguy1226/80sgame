using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{

    public AudioClip shootSound;
    public int score;
    public float accuracy;
    public int Order { get; private set; }

    private PlayerConfig config;

    [SerializeField]
    private Crosshair crosshairPrefab;
    private Crosshair activeCrosshair;

    private void Awake()
    {
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
        ShotInformation s = new(activeCrosshair.transform.position, Order);
        InputManager.PlayerShot(s);
    }

    public void RecenterCursor()
    {
        activeCrosshair.Center();
    }
}
