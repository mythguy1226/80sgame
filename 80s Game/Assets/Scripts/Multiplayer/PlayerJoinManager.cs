using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

[RequireComponent(typeof(PlayerInputManager))]
public class PlayerJoinManager : MonoBehaviour
{
    PlayerInputManager playerInputManager;

    public GameObject joinPanelPrefab;
    public GameObject joinPanelContainer;

    private void OnPlayerJoined(PlayerInput playerInput)
    {
        GameObject newPlayerPanel = Instantiate(joinPanelPrefab, joinPanelContainer.transform);
        playerInput.SwitchCurrentActionMap("UI");
        
        PlayerConfig config = new PlayerConfig(playerInput.playerIndex, PlayerData.defaultColors[playerInput.playerIndex], Vector2.one);
        PlayerController pc = playerInput.gameObject.GetComponent<PlayerController>();
        PlayerData.activePlayers.Add(config);
        pc.SetConfig(config);

        pc.GetComponent<PlayerInput>().uiInputModule = newPlayerPanel.transform.GetChild(0).GetComponent<InputSystemUIInputModule>();
        newPlayerPanel.GetComponent<PlayerJoinPanel>().UpdatePlayerNumber(pc.Order + 1);
    }
}