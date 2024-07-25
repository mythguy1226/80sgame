using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

public class PlayerJoinPanel : MonoBehaviour
{
    public TextMeshProUGUI playerHeader;

    //Crosshair Elements
    public Image crosshairPreview;
    public Button crosshairRightArrow;
    public Button crosshairLeftArrow;

    //Color Elements
    public GameObject colorSettings;
    public TMP_Dropdown colorDropdown;
    public GameObject colorSettingsOption;
    public List<Slider> colorSliders;
    public List<TextMeshProUGUI> sliderLabels;

    //Crosshair sprites that can be used
    public List<Sprite> crosshairSprites;
    private int crosshairIndex = 0;

    //Variables for Stun Effect Customization
    public GameObject stunPanel;
    public GameObject stunSettingsOption;
    public TextMeshProUGUI stunText;
    public List<GameObject> stunParticlePreviews;
    public List<GameObject> stunPrefabs;
    public Button stunLeftArrow;
    public Button stunRightArrow;
    private int stunIndex = 0;

    //List of elements for setting player initials
    public List<TextMeshProUGUI> initials;
    public List<Button> initialUpButtons;
    public List<Button> initialDownButtons;

    //Variables to keep track of which letter to display for each initial
    private int initialOne = 65;
    private int initialTwo = 65;
    private int initialThree = 65;

    //Variables for readying up
    public GameObject playerSettings;
    public GameObject readyUpPrompt;
    public bool playerReady;
    private GameObject lastSelected;
    
    //Variables for Player Profiles
    public GameObject profilePanel;
    public GameObject overwriteProfilePanel;
    public TMP_Dropdown profileDropdown;
    public GameObject profilesButton;
    public GameObject saveProfileButton;
    public GameObject confirmOverwrite;
    public TextMeshProUGUI profileSavedMessage;
    public TextMeshProUGUI overwriteMessage;
    private FileInfo[] profiles;

    public EventSystem eventSystem;
    private int player;
    private PlayerJoinManager pjm;
    private string filePath = Application.dataPath + "/PlayerData/PlayerProfiles";

    void Start()
    {
        //Set the player root for the panel's event system to the appropriate join panel
        eventSystem.GetComponent<MultiplayerEventSystem>().playerRoot = this.gameObject;
        
        //Set button events for the crosshair arrow buttons
        crosshairLeftArrow.onClick.AddListener(() => ChangeCrosshairSprite(false));
        crosshairRightArrow.onClick.AddListener(() => ChangeCrosshairSprite(true));

        //Set button events for stun arrow buttons
        stunLeftArrow.onClick.AddListener(() => ChangeStunParticle(false));
        stunRightArrow.onClick.AddListener(() => ChangeStunParticle(true));

        //Set the text for the crosshair indicator under the preview
        TextMeshProUGUI crosshairText = crosshairPreview.gameObject.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
        crosshairText.text = (crosshairIndex + 1) + "/" + crosshairSprites.Count;

        stunText.text = (stunIndex + 1) + "/" + stunParticlePreviews.Count;
        
        //Set the button events for the initial buttons
        for (int i = 0; i < initials.Count; i++)
        {
            int iCopy = i;
            initialUpButtons[i].onClick.AddListener(() => ChangeInitial(false, iCopy));
            initialDownButtons[i].onClick.AddListener(() => ChangeInitial(true, iCopy));
        }

        //Set preview crosshair to default color for that player
        colorSliders[0].value = PlayerData.defaultColors[player].r * 255;
        colorSliders[1].value = PlayerData.defaultColors[player].g * 255;
        colorSliders[2].value = PlayerData.defaultColors[player].b * 255;

        //Turn off the corsshairs
        PauseScreenBehavior.Instance.ToggleCrosshairs(false);
    }


    //Method for changing the crosshair sprite for the player
    public void ChangeCrosshairSprite(bool forward)
    {   
        //Increase or decrease the index based on the passed in parameter
        if (forward) crosshairIndex++;
        else crosshairIndex--;

        //Clamp the index to the size of the available crosshairs
        if (crosshairIndex == crosshairSprites.Count)
        {
            crosshairIndex = 0;
        }

        else if (crosshairIndex == -1)
        {
            crosshairIndex = crosshairSprites.Count - 1;
        }

        //Change the sprite of the crosshair preview
        crosshairPreview.sprite = crosshairSprites[crosshairIndex];

        //Update the text and player config to match
        TextMeshProUGUI crosshairText = crosshairPreview.gameObject.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
        crosshairText.text = (crosshairIndex + 1) + "/" + crosshairSprites.Count;
        UpdatePlayerConfig();
    }

    public void ChangeStunParticle(bool forward)
    {   
        stunParticlePreviews[stunIndex].SetActive(false);

        //Increase or decrease the index based on the passed in parameter
        if (forward) stunIndex++;
        else stunIndex--;

        //Clamp the index to the size of the available crosshairs
        if (stunIndex == stunParticlePreviews.Count)
        {
            stunIndex = 0;
        }

        else if (stunIndex == -1)
        {
            stunIndex = stunParticlePreviews.Count - 1;
        }

        //Change the sprite of the crosshair preview
        stunParticlePreviews[stunIndex].SetActive(true);

        //Update the text and player config to match
        stunText.text = (stunIndex + 1) + "/" + stunParticlePreviews.Count;
        UpdatePlayerConfig();
    }
    
    //Method for toggling the panel to change crosshair color settings
    public void ToggleColorSettings()
    {
        if (stunPanel.activeInHierarchy)   
        {
            ToggleStunSettings();
        }
        
        //Set the panel on or off
        colorSettings.SetActive(!colorSettings.activeInHierarchy);

        //Select the appropriate UI element for navigation based on if the panel is active or not
        if (colorSettings.activeInHierarchy)
        {
            eventSystem.SetSelectedGameObject(null);
            eventSystem.SetSelectedGameObject(colorDropdown.gameObject);
        }

        else
        {
            eventSystem.SetSelectedGameObject(null);
            eventSystem.SetSelectedGameObject(colorSettingsOption);
        }
    }

    public void ToggleStunSettings()
    {
        stunPanel.SetActive(!stunPanel.activeInHierarchy);

        //Select the appropriate UI element for navigation based on if the panel is active or not
        if (stunPanel.activeInHierarchy)
        {
            eventSystem.SetSelectedGameObject(null);
            eventSystem.SetSelectedGameObject(stunRightArrow.gameObject);
        }

        else
        {
            eventSystem.SetSelectedGameObject(null);
            eventSystem.SetSelectedGameObject(stunSettingsOption);
        }
    }

    //Method for changing the crosshair color
    public void ChangeCrosshairColor()
    {
        //Set the color of the preview based on each of the RGB slider values
        crosshairPreview.color = new Color(colorSliders[0].value / 255, colorSliders[1].value / 255, colorSliders[2].value / 255);

        //Update the text next to the sliders to match the value
        for (int i = 0; i < sliderLabels.Count; i++)
        {
            sliderLabels[i].text = colorSliders[i].value.ToString();
        }

        UpdatePlayerConfig();
    }

    //Method for changing the crosshair color preset
    public void ChangeCrosshairPreset()
    {
        //Create a new color
        Color newColor;

        //Get the color based off of the dropdown option's text
        if(ColorUtility.TryParseHtmlString(colorDropdown.captionText.text, out newColor))
        {
            //Set the sliders to match the new color from the preset
            colorSliders[0].value = newColor.r * 255;
            colorSliders[1].value = newColor.g * 255;
            colorSliders[2].value = newColor.b * 255;
        }

        //Fix Green slider being set to 128 when using Green preset
        if (colorDropdown.captionText.text == "Green")
        {
            colorSliders[1].value *= 2;
        }

        //Change the crosshair color based off of the new slider values
        ChangeCrosshairColor();
        UpdatePlayerConfig();
    }

    //Method for readying up
    public void ReadyUp()
    {   
        //Toggle whether the player is ready
        playerReady = !playerReady;

        //Show that the player is ready
        if (playerReady)
        {
            readyUpPrompt.SetActive(true);
            playerSettings.SetActive(false);

            lastSelected = eventSystem.currentSelectedGameObject;

            eventSystem.SetSelectedGameObject(null);
            eventSystem.SetSelectedGameObject(readyUpPrompt.transform.GetChild(1).gameObject);
        }
        
        //Show settings if player unreadies
        else
        {
            readyUpPrompt.SetActive(false);
            playerSettings.SetActive(true);

            eventSystem.SetSelectedGameObject(null);

            if (lastSelected != null)
            {
                eventSystem.SetSelectedGameObject(lastSelected);
            }
        }
        pjm.SetPlayerReady(player, playerReady);
    }
    
    //Method for changing the initials
    private void ChangeInitial(bool increase, int initialIndex)
    {
        char tempChar;

        //Change the proper initial based on the passed in index
        switch(initialIndex)
        {
            //First initial
            case 0:
                initialOne = UpdateInitialNumber(increase, initialOne);
                tempChar = (char)initialOne;
                initials[initialIndex].text = tempChar.ToString();
                break;

            //Second initial
            case 1:
                initialTwo = UpdateInitialNumber(increase, initialTwo);
                tempChar = (char)initialTwo;
                initials[initialIndex].text = tempChar.ToString();
                break;

            //Third initial
            case 2:
                initialThree = UpdateInitialNumber(increase, initialThree);
                tempChar = (char)initialThree;
                initials[initialIndex].text = tempChar.ToString();
                break;
        }

        UpdatePlayerConfig();
    }

    public void SetDefaultInitials(int playerNum)
    {
        switch (playerNum)
        {
            case 1:
                for (int i = 0; i < 3; i++)
                {
                    ChangeInitial(true, i);
                }
                break;
            case 2:
                for (int i = 0; i < 3; i++)
                {
                    ChangeInitial(true, i);
                    ChangeInitial(true, i);
                }
                break;
            case 3:
                for (int i = 0; i < 3; i++)
                {
                    ChangeInitial(true, i);
                    ChangeInitial(true, i);
                    ChangeInitial(true, i);
                }
                break;
        }
    }

    //Method for updating the player's config values
    private void UpdatePlayerConfig()
    {
        PlayerData.activePlayers[player].initials = initials[0].text + initials[1].text + initials[2].text;
        PlayerData.activePlayers[player].crossHairColor = new Color(colorSliders[0].value / 255, colorSliders[1].value / 255, colorSliders[2].value / 255);
        PlayerData.activePlayers[player].crosshairSprite = crosshairPreview.sprite;
        PlayerData.activePlayers[player].crossHairIndex = crosshairIndex;
        PlayerData.activePlayers[player].stunParticleIndex = stunIndex;
        PlayerData.activePlayers[player].stunParticles = stunPrefabs[stunIndex];
    }

    //Method for updating the number of the initial variables
    private int UpdateInitialNumber(bool increase, int initialNum)
    {   
        //Increase the initial number
        if (increase)
        {
            //If the player increases when the initial is Z (90), set it back to A (65)
            if (initialNum == 90)
            {
                return 65;
            }

            return Mathf.Clamp(++initialNum, 65, 90);
        }

        //Decrease the initial number
        else
        {
            //If the player decreases when the initial is A (65), set it back to Z (90)
            if (initialNum == 65)
            {
                return 90;
            }

            return Mathf.Clamp(--initialNum, 65, 90);
        }
    }

    //Update the player header with the appropriate player number
    public void UpdatePlayerNumber(int playerNum)
    {
        player = playerNum - 1;
        playerHeader.text = "Player " + playerNum;
    }

    public void SetManager(PlayerJoinManager manager)
    {
        pjm = manager;
    }

    //Saves the current settings to a profile that is stored as a local text file
    public void SaveProfile()
    {
        //Set file path
        filePath = Application.dataPath + "/PlayerData/PlayerProfiles" + "/" + PlayerData.activePlayers[player].initials + ".txt";

        // Get the file directory
        string dirPath = Path.GetDirectoryName(filePath);
        
        // Create the folder if it doesn't exist
        if (!Directory.Exists(dirPath))
        {
            Directory.CreateDirectory(dirPath);
        }

        //Write to the file if it doesn't exist
        else if (!File.Exists(filePath))
        {
            WriteProfileData(filePath);
        }
        
        //If the profile doesn't exist, prompt the user to overwrite
        else
        {
            overwriteProfilePanel.SetActive(true);

            overwriteMessage.text = "Profile Already Exists\n\n\"" + PlayerData.activePlayers[player].initials + "\"\n\nDo you want to overwrite it?";

            eventSystem.SetSelectedGameObject(null);
            eventSystem.SetSelectedGameObject(confirmOverwrite);
        }
    }

    //Writes profile data to a file at the specified path
    private void WriteProfileData(string filePath)
    {
        //Set the initials for the profile data
        string profileData = initialOne.ToString();
        profileData += "\n" + initialTwo.ToString();
        profileData += "\n" + initialThree.ToString();

        //Add crosshair color to profile data
        profileData += "\n" + colorSliders[0].value;
        profileData += "\n" + colorSliders[1].value;
        profileData += "\n" + colorSliders[2].value;

        //Add crosshair sprite index to profile data
        profileData += "\n" + PlayerData.activePlayers[player].crossHairIndex;

        //Add stun effect index to profile data
        profileData += "\n" + PlayerData.activePlayers[player].stunParticleIndex;

        //Write the data to the text file
        File.WriteAllText(filePath, profileData);

        //Inform the user that the profile was saved
        profileSavedMessage.gameObject.SetActive(true);
        profileSavedMessage.text = "\"" + PlayerData.activePlayers[player].initials + "\"\nSaved!";

        //Reload the profile options to account for the newly saved profile
        ClearProfileOptions();
        LoadProfiles();
    }

    //Overwrites a currently existing profile with new data
    public void ConfirmOverwriteProfile()
    {
        //Writes the new data to the file with the current file path
        WriteProfileData(filePath);
        CloseOverwritePanel();
    }

    //Closes the panel prompting the user to overwrite a profile
    public void CloseOverwritePanel()
    {
        overwriteProfilePanel.SetActive(false);
        eventSystem.SetSelectedGameObject(null);
        eventSystem.SetSelectedGameObject(saveProfileButton);
    }

    //Loads the profiles from the PlayerProfiles folder under PlayerData
    private void LoadProfiles()
    {
        //Get all the .txt files in the folder and store their names in a list
        DirectoryInfo dir = new DirectoryInfo(Application.dataPath + "/PlayerData/PlayerProfiles");
        profiles = dir.GetFiles("*.txt");
        List<string> profileNames = new List<string>();

        foreach (FileInfo file in profiles)
        {
            profileNames.Add(Path.GetFileNameWithoutExtension(file.Name));
        }

        //Add the list of profile names as options to the profile dropdown
        profileDropdown.AddOptions(profileNames);
    }

    //Toggles the profile panel on and off
    public void ToggleProfilePanel()
    {
        profilePanel.SetActive(!profilePanel.activeInHierarchy);

        if (profilePanel.activeInHierarchy)
        {
            //Load the profile options
            LoadProfiles();
            eventSystem.SetSelectedGameObject(null);
            eventSystem.SetSelectedGameObject(saveProfileButton);
        }

        else
        {
            //Clear the options so only default is present (reloaded when panel is opened again)
            ClearProfileOptions();
            profileSavedMessage.gameObject.SetActive(false);
            eventSystem.SetSelectedGameObject(null);
            eventSystem.SetSelectedGameObject(profilesButton);
        }
    }

    //Clears the profile options
    private void ClearProfileOptions()
    {
        //Clear options
        profileDropdown.ClearOptions();
        
        //Add default option to dropdown
        List<string> defaultOptions = new List<string>();
        defaultOptions.Add("Default");

        profileDropdown.AddOptions(defaultOptions);
    }

    //Change the player's settings when they select a profile
    public void SelectProfileOption()
    {
        //Default profile
        if (profileDropdown.captionText.text == "Default")
        {
            //Change the crosshair to the default
            crosshairIndex = 0;
            ChangeCrosshairSprite(false);
            ChangeCrosshairSprite(true);

            //Change stun particle to default
            stunParticlePreviews[stunIndex].SetActive(false);
            stunIndex = 0;
            ChangeStunParticle(false);
            ChangeStunParticle(true);

            //Change the preset to whatever the last preset selected was
            ChangeCrosshairPreset();
            
            //Change initials back to AAA
            initialOne = 66;
            ChangeInitial(false, 0);

            initialTwo = 66;
            ChangeInitial(false, 1);

            initialThree = 66;
            ChangeInitial(false, 2);
        }

        //Reads in the player settings from the profile data
        else
        {
            string filePath = null;

            //Check that the file name in the directory matches the dropdown option selected
            foreach (FileInfo file in profiles)
            {
                Debug.Log(Path.GetFileNameWithoutExtension(file.Name));
                if (Path.GetFileNameWithoutExtension(file.Name) == profileDropdown.captionText.text)
                {
                    //Set the file path
                    filePath = file.FullName;
                }
            }
            
            if (filePath != null)
            {
                //Read in the lines from the profile
                string[] lines = File.ReadAllLines(filePath);

                //Set initials then change them back and forth to update accordingly
                initialOne = int.Parse(lines[0]);
                ChangeInitial(false, 0);
                ChangeInitial(true, 0);

                initialTwo = int.Parse(lines[1]);
                ChangeInitial(false, 1);
                ChangeInitial(true, 1);

                initialThree = int.Parse(lines[2]);
                ChangeInitial(false, 2);
                ChangeInitial(true, 2);

                //Change the color of the crosshair to the saved color for the profile
                colorSliders[0].value = int.Parse(lines[3]);
                colorSliders[1].value = int.Parse(lines[4]);
                colorSliders[2].value = int.Parse(lines[5]);

                //Change the crosshair sprite to the saved index for the profile
                crosshairIndex = int.Parse(lines[6]);
                ChangeCrosshairSprite(true);
                ChangeCrosshairSprite(false);

                //Change the stun effect to the saved index for the profile
                stunParticlePreviews[stunIndex].SetActive(false);
                stunIndex = int.Parse(lines[7]);
                ChangeStunParticle(false);
                ChangeStunParticle(true);
            }
        }
    }
}
