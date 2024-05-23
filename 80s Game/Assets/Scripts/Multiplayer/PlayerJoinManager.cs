using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInputManager))]
public class PlayerJoinManager : MonoBehaviour
{
    PlayerInputManager playerInputManager;

    private void OnPlayerJoined(PlayerInput playerInput)
    {
        if (playerInput.playerIndex == 0)
        {
            // Skip the first player
            return;
        }
        PlayerConfig config = new PlayerConfig(playerInput.playerIndex, PlayerData.defaultColors[playerInput.playerIndex], Vector2.one);
        PlayerController pc = playerInput.gameObject.GetComponent<PlayerController>();
        PlayerData.activePlayers.Add(config);
        pc.SetConfig(config);
    }
}