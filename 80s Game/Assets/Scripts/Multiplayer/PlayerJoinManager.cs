using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.EventSystems;

[RequireComponent(typeof(PlayerInputManager))]
public class PlayerJoinManager : MonoBehaviour
{
    // This class handles interactions in the Join Screen to register new players

    public TextMeshProUGUI joinPrompt;
    Dictionary<int, bool> joinStatus;
    public GameObject joinPanelPrefab;
    public GameObject joinPanelContainer;
    public GameObject backOutPanel;
    public GameObject backOutExit;
    public GameObject promptTray;

    public List<Image> promptTrayIcons;
    public List<Sprite> controllerInputPrompts;
    public List<Sprite> keyboardInputPrompts;

    private GameObject lastSelected = null;
    private int backOutPlayerRef;


    private void Awake()
    {
        joinStatus = new Dictionary<int, bool>();
    }

    private void OnPlayerJoined(PlayerInput playerInput)
    {
        // This event is called by the Player Input Manager component when a player joins
        // The parameter passed is the PlayerInput component the instantiated Player object has.

        //First, we make sure any players joining are set to _not_ be ready, so moving to the gameplay scene can't happen by accident
        joinStatus.Add(playerInput.playerIndex, false);

        // We then instantiate a Player Panel to allow players to set their preferences for gameplay and alter the control scheme accordingly.
        GameObject newPlayerPanel = Instantiate(joinPanelPrefab, joinPanelContainer.transform);
        playerInput.SwitchCurrentActionMap("UI");
        
        // We then create a data object that will persist player's information from the join scene to the gameplay scene
        PlayerConfig config = new PlayerConfig(playerInput.playerIndex, PlayerData.defaultColors[playerInput.playerIndex], new Vector2(2.5f,2.5f));
        config.controlScheme = playerInput.currentControlScheme;
        config.device = playerInput.devices[0];

        // We get the PlayerController object of the player who has joined to bind it to this manager and make sure no awkardness happens
        // with UI controls. We also set its PlayerConfig.
        PlayerController pc = playerInput.gameObject.GetComponent<PlayerController>();
        config.crosshairSprite = pc.GetCrosshairSprite();
        PlayerData.activePlayers.Add(config);
        pc.SetConfig(config, PlayerController.ControllerState.JoinScreen);
        pc.SetJoinManager(this);
        pc.GetComponent<PlayerInput>().uiInputModule = newPlayerPanel.transform.GetChild(0).GetComponent<InputSystemUIInputModule>();
        
        // We set the active player information in the PlayerPanel to ensure that readiness properly communicates to this manager
        newPlayerPanel.GetComponent<PlayerJoinPanel>().UpdatePlayerNumber(pc.Order + 1);
        newPlayerPanel.GetComponent<PlayerJoinPanel>().SetManager(this);

        // We also update any onscreen UI the new state of having at least one joined player on screen.
        joinPrompt.gameObject.SetActive(false);
        promptTray.gameObject.SetActive(true);

        if (pc.Order == 0)
        {
            if (playerInput.currentControlScheme == "KnM")
            {
                for (int i = 0; i < promptTrayIcons.Count; i++)
                {
                    promptTrayIcons[i].sprite = keyboardInputPrompts[i];
                }
            }

            else
            {
                for (int i = 0; i < promptTrayIcons.Count; i++)
                {
                    promptTrayIcons[i].sprite = controllerInputPrompts[i];
                }
            }
        }
        //joinPrompt.text = "Press Start when ready";
        //joinPrompt.rectTransform.SetLocalPositionAndRotation(new Vector3(0, -460, 0), Quaternion.identity);

        // In classic mode we only want one player to join, so we disable future joining.
        if (GameModeData.activeGameMode == EGameMode.Classic)
        {
            PlayerInputManager.instance.DisableJoining();        
        }
    }

    // This function sets a player as ready in the dictionary tracked by this manager
    public void SetPlayerReady(int player, bool isReady)
    {
        joinStatus[player] = isReady;
    }

    // This function launches the corresponding game mode that has been loaded by selection in the previous scene.
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
        SceneManager.LoadScene(GameModeData.GameModeToSceneIndex());
    }

    public void BackOut(int playerIndex)
    {
        backOutPanel.SetActive(!backOutPanel.activeInHierarchy);
        //TogglePanelControls();
        if (backOutPanel.activeInHierarchy)
        {
            int childIndex = 0;
            foreach (Transform child in joinPanelContainer.transform)
            {
                if (childIndex == playerIndex)
                {
                    backOutPlayerRef = childIndex;
                    MultiplayerEventSystem eventSystem = child.GetChild(0).GetComponent<MultiplayerEventSystem>();
                    lastSelected = eventSystem.currentSelectedGameObject;

                    Debug.Log(lastSelected);
                    eventSystem.playerRoot = backOutPanel;
                    eventSystem.SetSelectedGameObject(null);
                    eventSystem.SetSelectedGameObject(backOutExit);
                }

                else
                {
                    child.GetChild(0).gameObject.SetActive(false);
                }

                childIndex++;
            }
        }

        else
        {
            CloseBackOutPanel();
        }
    }

    public void CloseBackOutPanel()
    {
        backOutPanel.SetActive(false);

        int childIndex = 0;
            foreach (Transform child in joinPanelContainer.transform)
            {
                if (childIndex == backOutPlayerRef)
                {                
                    MultiplayerEventSystem eventSystem = child.GetChild(0).GetComponent<MultiplayerEventSystem>();
                    eventSystem.playerRoot = child.gameObject;
                    eventSystem.SetSelectedGameObject(null);
                    eventSystem.SetSelectedGameObject(lastSelected);
                }

                else
                {
                    child.GetChild(0).gameObject.SetActive(true);
                }

                childIndex++;
            }
    }

    public void ExitToTitle()
    {
        SceneManager.LoadScene(0);
    }

    public void TogglePanelControls()
    {
        
    }
}