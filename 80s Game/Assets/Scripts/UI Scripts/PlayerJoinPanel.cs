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

    public List<Sprite> crosshairSprites;
    private int crosshairIndex = 0;

    public List<TextMeshProUGUI> initials;
    public List<Button> initialUpButtons;
    public List<Button> initialDownButtons;

    private int initialOne = 65;
    private int initialTwo = 65;
    private int initialThree = 65;

    public TextMeshProUGUI readyIndicator;
    public bool playerReady;

    public EventSystem eventSystem;
    private int player;
    private PlayerJoinManager pjm;

    void Start()
    {
        eventSystem.GetComponent<MultiplayerEventSystem>().playerRoot = this.gameObject;
        
        crosshairLeftArrow.onClick.AddListener(() => ChangeCrosshairSprite(false));
        crosshairRightArrow.onClick.AddListener(() => ChangeCrosshairSprite(true));

        TextMeshProUGUI crosshairText = crosshairPreview.gameObject.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
        crosshairText.text = (crosshairIndex + 1) + "/" + crosshairSprites.Count;
        for (int i = 0; i < initials.Count; i++)
        {
            int iCopy = i;
            initialUpButtons[i].onClick.AddListener(() => ChangeInitial(true, iCopy));
            initialDownButtons[i].onClick.AddListener(() => ChangeInitial(false, iCopy));
        }

        PauseScreenBehavior.Instance.ToggleCrosshairs(false);

    }

    public void ChangeCrosshairSprite(bool forward)
    {   
        if (forward) crosshairIndex++;
        else crosshairIndex--;

        crosshairIndex = Mathf.Clamp(crosshairIndex, 0, crosshairSprites.Count - 1);

        crosshairPreview.sprite = crosshairSprites[crosshairIndex];

        TextMeshProUGUI crosshairText = crosshairPreview.gameObject.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
        crosshairText.text = (crosshairIndex + 1) + "/" + crosshairSprites.Count;
    }

    public void ToggleColorSettings()
    {
        colorSettings.SetActive(!colorSettings.activeInHierarchy);

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

    public void ChangeCrosshairColor()
    {
        crosshairPreview.color = new Color(colorSliders[0].value / 255, colorSliders[1].value / 255, colorSliders[2].value / 255);
        for (int i = 0; i < sliderLabels.Count; i++)
        {
            sliderLabels[i].text = colorSliders[i].value.ToString();
        }
    }

    public void ChangeCrosshairPreset()
    {
        Color newColor;
        if(ColorUtility.TryParseHtmlString(colorDropdown.captionText.text, out newColor))
        {
            colorSliders[0].value = newColor.r * 255;
            colorSliders[1].value = newColor.g * 255;
            colorSliders[2].value = newColor.b * 255;
        }

        ChangeCrosshairColor();
        
    }

    public void ReadyUp()
    {
        playerReady = !playerReady;

        if (playerReady)
        {
            readyIndicator.color = Color.green;
        }
        
        else
        {
            readyIndicator.color = Color.red;
        }
        pjm.SetPlayerReady(player, playerReady);
    }

    private void ChangeInitial(bool increase, int initialIndex)
    {
        char tempChar;
        switch(initialIndex)
        {
            case 0:
                initialOne = UpdateInitialNumber(increase, initialOne);
                tempChar = (char)initialOne;
                initials[initialIndex].text = tempChar.ToString();
                break;

            case 1:
                initialTwo = UpdateInitialNumber(increase, initialTwo);
                tempChar = (char)initialTwo;
                initials[initialIndex].text = tempChar.ToString();
                break;

            case 2:
                initialThree = UpdateInitialNumber(increase, initialThree);
                tempChar = (char)initialThree;
                initials[initialIndex].text = tempChar.ToString();
                break;
        }
    }

    private int UpdateInitialNumber(bool increase, int initialNum)
    {
        if (increase)
        {
            if (initialNum == 90)
            {
                return 65;
            }

            return Mathf.Clamp(++initialNum, 65, 90);
        }

        else
        {
            if (initialNum == 65)
            {
                return 90;
            }

            return Mathf.Clamp(--initialNum, 65, 90);
        }
    }

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
