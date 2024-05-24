using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInputManager))]
public class PlayerJoinManager : MonoBehaviour
{
    PlayerInputManager playerInputManager;

    public GameObject joinPanelPrefab;
    public GameObject joinPanelContainer;

    private void OnPlayerJoined(PlayerInput playerInput)
    {
        Instantiate(joinPanelPrefab, joinPanelContainer.transform);
        PauseScreenBehavior.Instance.ToggleCrosshairs(false);
        
        PlayerConfig config = new PlayerConfig(playerInput.playerIndex, PlayerData.defaultColors[playerInput.playerIndex], Vector2.one);
        PlayerController pc = playerInput.gameObject.GetComponent<PlayerController>();
        PlayerData.activePlayers.Add(config);
        pc.SetConfig(config);
    }
}