using System.Collections;
using System.Collections.Generic;
using UnityEditor.Timeline.Actions;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    

    public AudioClip shootSound;
    public int score;
    public float accuracy;
    public int Order { get; private set; }

    [SerializeField]
    private Crosshair crosshairPrefab;
    private Crosshair activeCrosshair;

    private void Awake()
    {
        PlayerData.activePlayers.Add(this);
        Order = PlayerData.activePlayers.Count;
        activeCrosshair = Instantiate(crosshairPrefab, new Vector3(0,0,0), Quaternion.identity);
    }

    public void HandleMovement(Vector2 movementData)
    {

        activeCrosshair.SetMovementDelta(movementData);
    }

    public void HandleFire()
    {
        Debug.Log("Fire");
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
}
