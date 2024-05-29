using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using TMPro;

[RequireComponent(typeof(PlayerInputManager))]
public class PlayerJoinManager : MonoBehaviour
{
    public TextMeshProUGUI joinPrompt;
    Dictionary<int, bool> joinStatus;

    private void Awake()
    {
        joinStatus = new Dictionary<int, bool>();
    }

    public GameObject joinPanelPrefab;
    public GameObject joinPanelContainer;

    private void OnPlayerJoined(PlayerInput playerInput)
    {
        joinStatus.Add(playerInput.playerIndex, false);
        GameObject newPlayerPanel = Instantiate(joinPanelPrefab, joinPanelContainer.transform);
        playerInput.SwitchCurrentActionMap("UI");
        
        PlayerConfig config = new PlayerConfig(playerInput.playerIndex, PlayerData.defaultColors[playerInput.playerIndex], Vector2.one);
        config.controlScheme = playerInput.currentControlScheme;
        config.device = playerInput.devices[0];
        PlayerController pc = playerInput.gameObject.GetComponent<PlayerController>();
        config.crosshairSprite = pc.GetCrosshairSprite();
        PlayerData.activePlayers.Add(config);
        pc.SetConfig(config, PlayerController.ControllerState.JoinScreen);
        pc.SetJoinManager(this);
        pc.GetComponent<PlayerInput>().uiInputModule = newPlayerPanel.transform.GetChild(0).GetComponent<InputSystemUIInputModule>();
        
        
        newPlayerPanel.GetComponent<PlayerJoinPanel>().UpdatePlayerNumber(pc.Order + 1);
        newPlayerPanel.GetComponent<PlayerJoinPanel>().SetManager(this);

        joinPrompt.text = "Press Start when ready";
        joinPrompt.rectTransform.SetLocalPositionAndRotation(new Vector3(0, -460, 0), Quaternion.identity);
    }

    public void SetPlayerReady(int player, bool isReady)
    {
        joinStatus[player] = isReady;
    }

    public void LaunchGameMode()
    {
        foreach(KeyValuePair<int, bool> kvp in joinStatus)
        {
            if (!kvp.Value)
            {
                //Someone is not ready
                return;
            }
        }

        // Everyone is ready
        SceneManager.LoadScene(3);
    }
}