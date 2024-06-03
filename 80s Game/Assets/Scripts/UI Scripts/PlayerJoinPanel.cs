using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
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

    //List of elements for setting player initials
    public List<TextMeshProUGUI> initials;
    public List<Button> initialUpButtons;
    public List<Button> initialDownButtons;

    //Variables to keep track of which letter to display for each initial
    private int initialOne = 65;
    private int initialTwo = 65;
    private int initialThree = 65;

    //Variables for readying up
    public Image readyIndicator;
    public Sprite notReady;
    public Sprite isReady;
    public bool playerReady;

    public EventSystem eventSystem;
    private int player;
    private PlayerJoinManager pjm;

    void Start()
    {
        //Set the player root for the panel's event system to the appropriate join panel
        eventSystem.GetComponent<MultiplayerEventSystem>().playerRoot = this.gameObject;
        
        //Set button events for the crosshair arrow buttons
        crosshairLeftArrow.onClick.AddListener(() => ChangeCrosshairSprite(false));
        crosshairRightArrow.onClick.AddListener(() => ChangeCrosshairSprite(true));

        //Set the text for the crosshair indicator under the preview
        TextMeshProUGUI crosshairText = crosshairPreview.gameObject.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
        crosshairText.text = (crosshairIndex + 1) + "/" + crosshairSprites.Count;
        
        //Set the button events for the initial buttons
        for (int i = 0; i < initials.Count; i++)
        {
            int iCopy = i;
            initialUpButtons[i].onClick.AddListener(() => ChangeInitial(true, iCopy));
            initialDownButtons[i].onClick.AddListener(() => ChangeInitial(false, iCopy));
        }

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
        crosshairIndex = Mathf.Clamp(crosshairIndex, 0, crosshairSprites.Count - 1);

        //Change the sprite of the crosshair preview
        crosshairPreview.sprite = crosshairSprites[crosshairIndex];

        //Update the text and player config to match
        TextMeshProUGUI crosshairText = crosshairPreview.gameObject.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
        crosshairText.text = (crosshairIndex + 1) + "/" + crosshairSprites.Count;
        UpdatePlayerConfig();
    }
    
    //Method for toggling the panel to change crosshair color settings
    public void ToggleColorSettings()
    {
        //Set the panel on or off
        colorSettings.SetActive(!colorSettings.activeInHierarchy);

        //Select the appropriate UI element for navigation based on if the panel is active or not
        if (colorSettings.activeInHierarchy)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(colorDropdown.gameObject);
        }

        else
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(colorSettingsOption);
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

        //Change the crosshair color based off of the new slider values
        ChangeCrosshairColor();
        UpdatePlayerConfig();
    }

    //Method for readying up
    public void ReadyUp()
    {   
        //Toggle whether the player is ready
        playerReady = !playerReady;

        //Set the ready indicator to the appropriate sprite
        if (playerReady)
        {
            readyIndicator.sprite = isReady;
        }
        
        else
        {
            readyIndicator.sprite = notReady;
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

    //Method for updating the player's config values
    private void UpdatePlayerConfig()
    {
        PlayerData.activePlayers[player].initials = initials[0].text + initials[1].text + initials[2].text;
        PlayerData.activePlayers[player].crossHairColor = new Color(colorSliders[0].value / 255, colorSliders[1].value / 255, colorSliders[2].value / 255);
        PlayerData.activePlayers[player].crosshairSprite = crosshairPreview.sprite;
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
}
