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
    public List<Sprite> playstationInputPrompts;
    public List<Sprite> xboxInputPrompts;
    public List<Sprite> keyboardInputPrompts;

    private GameObject lastSelected = null;
    private int backOutPlayerRef;
    private bool controllerConnected;
    private bool sceneTransition = false;
    private float defaultSensitivity = 2.5f;
    private float mouseDefaultSensitivity = 3.25f;

    private void Awake()
    {
        joinStatus = new Dictionary<int, bool>();
    }

    private void OnPlayerJoined(PlayerInput playerInput)
    {
        if (PlayerData.activePlayers.Count == 4)
        {
            return;
        }
        // This event is called by the Player Input Manager component when a player joins
        // The parameter passed is the PlayerInput component the instantiated Player object has.

        //First, we make sure any players joining are set to _not_ be ready, so moving to the gameplay scene can't happen by accident
        joinStatus.Add(playerInput.playerIndex, false);

        // We then instantiate a Player Panel to allow players to set their preferences for gameplay and alter the control scheme accordingly.
        GameObject newPlayerPanel = Instantiate(joinPanelPrefab, joinPanelContainer.transform);
        playerInput.SwitchCurrentActionMap("UI");

        // To avoid the 0-sensitivity bug, we begin by defining a sensitivity value that might get overriden later
        // Sensitivity values change by input, so we begin by detecting input type
        string currentControlScheme = playerInput.currentControlScheme;
        float defaultDeviceSensitivity = defaultSensitivity;
        if(currentControlScheme == "KnM")
        {
            defaultDeviceSensitivity = mouseDefaultSensitivity;
        }
        
        // We then create a data object that will persist player's information from the join scene to the gameplay scene
        PlayerConfig config = new PlayerConfig(playerInput.playerIndex, PlayerData.defaultColors[playerInput.playerIndex], new Vector2(2.5f,2.5f));
        config.controlScheme = playerInput.currentControlScheme;
        config.device = playerInput.devices[0];

        // We get the PlayerController object of the player who has joined to bind it to this manager and make sure no awkardness happens
        // with UI controls. We also set its PlayerConfig.
        PlayerController pc = playerInput.gameObject.GetComponent<PlayerController>();
        config.crosshairSprite = pc.GetCrosshairSprite();

        // Set back sensitivity override if this is player one
        if(playerInput.playerIndex == 0)
        {
            config.sensitivity = new Vector2(PlayerPrefs.GetFloat("Sensitivity", defaultDeviceSensitivity), PlayerPrefs.GetFloat("Sensitivity", defaultDeviceSensitivity));
        } else
        {
            config.sensitivity = new Vector2(defaultSensitivity, defaultSensitivity);
        }
            
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

        //Change Prompt Tray Icons based on which control scheme Player 1 uses
        if (pc.Order == 0)
        {
            switch (currentControlScheme)
            {
                case "KnM":
                    ChangePromptTray(keyboardInputPrompts, false);
                    break;
                case "PS4":
                    ChangePromptTray(playstationInputPrompts, true);
                    break;
                case "xbox":
                    ChangePromptTray(xboxInputPrompts, true);
                    break;
            } 
        }

        //Set color preset based on default color
        switch(pc.Order)
        {
            case 1: 
                newPlayerPanel.GetComponent<PlayerJoinPanel>().colorDropdown.value = 1;
                break;
            case 2: 
                newPlayerPanel.GetComponent<PlayerJoinPanel>().colorDropdown.value = 2;
                break;
            case 3: 
                newPlayerPanel.GetComponent<PlayerJoinPanel>().colorDropdown.value = 5;
                break;
        }

        //Set initials to default values for each player
        newPlayerPanel.GetComponent<PlayerJoinPanel>().SetDefaultInitials(pc.Order);

        //Disable auto scroll for dropdowns if using mouse and keyboard
        if (playerInput.currentControlScheme == "KnM")
        {
            newPlayerPanel.GetComponent<PlayerJoinPanel>().colorDropdown.gameObject.transform.GetChild(3).GetComponent<ScrollRectEnsureVisible>().enabled = false;
            newPlayerPanel.GetComponent<PlayerJoinPanel>().profileDropdown.gameObject.transform.GetChild(3).GetComponent<ScrollRectEnsureVisible>().enabled = false;
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

        foreach(KeyValuePair<int, bool> kvp in joinStatus)
        {
            if (!kvp.Value)
            {
                //Someone is not ready
                return;
            }
        }

        //Launch Game when everyone is ready
        LaunchGameMode();
    }

    // This function launches the corresponding game mode that has been loaded by selection in the previous scene.
    public void LaunchGameMode()
    {
        //Don't launch competitive with only 1 player
        if (GameModeData.activeGameMode == EGameMode.Competitive && PlayerData.activePlayers.Count == 1)
        {
            return;
        }

        if (!sceneTransition)
        {
            // Everyone is ready
            SceneManager.LoadScene(GameModeData.GameModeToSceneIndex());
            sceneTransition = true;
        }
    }

    public void BackOut(int playerIndex)
    {
        PlayerJoinPanel backOutPlayer = joinPanelContainer.transform.GetChild(playerIndex).GetComponent<PlayerJoinPanel>();

        //Close color settings if player backs out with it open
        if (backOutPlayer.colorSettings.activeInHierarchy)
        {
            backOutPlayer.ToggleColorSettings();
            return;
        }

        //Close profile panel if player backs out with it open
        else if(backOutPlayer.profilePanel.activeInHierarchy)
        {
            backOutPlayer.ToggleProfilePanel();
            return;
        }

        else if (backOutPlayer.stunPanel.activeInHierarchy)
        {
            backOutPlayer.ToggleStunSettings();
            return;
        }


        //Toggle the back out panel
        backOutPanel.SetActive(!backOutPanel.activeInHierarchy);

        //If the panel is active
        if (backOutPanel.activeInHierarchy)
        {
            //Loop through all the join panels and keep track of the index
            int childIndex = 0;
            foreach (Transform child in joinPanelContainer.transform)
            {
                //If the panel matches the player who clicked the exit button
                if (childIndex == playerIndex)
                {
                    //Set player ref
                    backOutPlayerRef = childIndex;

                    //Change player root for their event system and set their last selected game object
                    MultiplayerEventSystem eventSystem = child.GetChild(0).GetComponent<MultiplayerEventSystem>();
                    lastSelected = eventSystem.currentSelectedGameObject;
                    eventSystem.playerRoot = backOutPanel;

                    //Set the new selected object to the exit button on the back out panel
                    eventSystem.SetSelectedGameObject(null);
                    eventSystem.SetSelectedGameObject(backOutExit);
                }

                //If the player didn't hit the exit button
                else
                {   
                    //Disable their inputs but deactivating their event system
                    child.GetChild(0).gameObject.SetActive(false);
                }

                childIndex++;
            }
        }

        //If the back out panel isn't active, close it
        else
        {
            CloseBackOutPanel();
        }
    }

    public void CloseBackOutPanel()
    {
        //Turn off the panel
        backOutPanel.SetActive(false);

        int childIndex = 0;

        //Loop through all the join panels
        foreach (Transform child in joinPanelContainer.transform)
        {
            //If the panel matches the player that has control of the back out panel
            if (childIndex == backOutPlayerRef)
            {           
                //Set their player root back to their join panel     
                MultiplayerEventSystem eventSystem = child.GetChild(0).GetComponent<MultiplayerEventSystem>();
                eventSystem.playerRoot = child.gameObject;

                //Set their selected object back to what it was when they hit exit
                eventSystem.SetSelectedGameObject(null);
                eventSystem.SetSelectedGameObject(lastSelected);
            }

            //If the panel isn't for the player that hit exit, reactivate their controls
            else
            {
                child.GetChild(0).gameObject.SetActive(true);
            }

            childIndex++;
        }
    }

    public void ExitToTitle()
    {
        SceneManager.LoadScene(2);
    }

    //Change the prompt tray icons based on the based in sprite list and whether a controller is used or not
    private void ChangePromptTray (List<Sprite> promptSprites, bool controller)
    {
        for (int i = 0; i < promptTrayIcons.Count; i++)
        {
            promptTrayIcons[i].sprite = promptSprites[i];
            controllerConnected = controller;
        }
    }
}