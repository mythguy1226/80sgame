using System.Collections;
using System.Collections.Generic;
using RetroTVFX;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.Rendering.PostProcessing;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance { get; private set; }

    //References to scripts that the settings will affect
    public int playerIndex;
    public CRTEffect crtEffect;

    //References to UI Elements
    public Slider musicVolumeSlider;
    public Slider sfxVolumeSlider;
    public Slider sensitivitySlider;
    public Toggle crtToggle;
    public Toggle bloomToggle;
    public Slider crtCurvature;
    public Button saveAndQuit;
    public Button applyButton;
    public GameObject controlsNextMapping;

    public GameObject settingsPanel;
    public GameObject cancelPanel;
    public GameObject settingsButton;
    public List<TextMeshProUGUI> settingsLabels;

    public List<GameObject> tabs;
    /*public List<Image> tabPrompts;
    public List<Sprite> controllerTabPrompts;*/
    private int tabIndex = 0;

    public List<GameObject> controlMappings;
    private int mappingIndex = 0;

    private float sensitivityValue;

    private GameObject lastSelected;

    // Start is called before the first frame update
    void Start()
    {
        // Check if the static reference matches the script instance
        if(Instance != null && Instance != this)
        {
            // If not, then the script is a duplicate and can delete itself
            Destroy(this);
        }

        else
        {
            Instance = this;
        }

        //Turn the crosshairs off
        this.GetComponent<PauseScreenBehavior>().ToggleCrosshairs(false);
        playerIndex = -1;

        //Load in the settings from PlayerPrefs
        LoadSettings();
    }

    //Change *Music Volume
    public void ChangeMusicVolume()
    {
        SoundManager.Instance.MusicVolume = musicVolumeSlider.value;
        float musicVolumeLabel = Mathf.RoundToInt(musicVolumeSlider.value * 100);
        settingsLabels[1].text = musicVolumeLabel.ToString();
    }

    //Change SFX Volume
    public void ChangeSFXVolume()
    {
        SoundManager.Instance.sfxVolume = sfxVolumeSlider.value;
        float sfxVolumeLabel = Mathf.RoundToInt(sfxVolumeSlider.value * 100);
        settingsLabels[0].text = sfxVolumeLabel.ToString();
    }

    //Change the sensitivity for the player that paused the game 
    public void ChangeSensitivity()
    {
        sensitivityValue = sensitivitySlider.value / 4;
        PlayerData.activePlayers[playerIndex].sensitivity = new Vector2(sensitivityValue, sensitivityValue);
        float sensitivityLabel = sensitivitySlider.value / 4;
        settingsLabels[2].text = sensitivityLabel.ToString("F2");
    }

    public void ToggleBloom()
    {
        GameManager.Instance.UIManager.postProcessVolume.profile.GetSetting<Bloom>().active = bloomToggle.isOn;
    }

    //Toggle the CRT effect on and off
    public void ToggleCRTEffect()
    {
        crtEffect.enabled = crtToggle.isOn;
    }

    //Change the intensity of the curve for the CRT effect
    public void ChangeCRTCurvature()
    {
        crtEffect.Curvature = crtCurvature.value;
        float crtCurvatureLabel = Mathf.RoundToInt(crtCurvature.value * 500);
        settingsLabels[3].text = crtCurvatureLabel.ToString();
    }

    //Toggle the menu on and off
    public void ToggleSettingsPanel()
    {
        //Switch tab back to Settings
        PreviousTab();

        //Toggle panels
        settingsPanel.SetActive(!settingsPanel.activeInHierarchy);
        cancelPanel.SetActive(false);
        
        //Get which player paused the game to determine who is changing settings
        GetPlayerReference(PauseScreenBehavior.Instance.playerIndex + 1);
        LoadSettings();

        //Select the proper UI element for navigation
        if (settingsPanel.activeInHierarchy)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(sfxVolumeSlider.gameObject);

            //Change input prompts for changing tabs based on the control scheme of the player who paused
            /*switch (PlayerData.activePlayers[playerIndex].controlScheme)
            {
                case "PS4":
                    tabPrompts[0].sprite = controllerTabPrompts[0];
                    tabPrompts[1].sprite = controllerTabPrompts[1];
                    break;
                case "xbox":
                    tabPrompts[0].sprite = controllerTabPrompts[2];
                    tabPrompts[1].sprite = controllerTabPrompts[3];
                    break;
                case "KnM":
                    tabPrompts[0].sprite = controllerTabPrompts[4];
                    tabPrompts[1].sprite = controllerTabPrompts[5];
                    break;
            }*/
        }

        else
        {
            EventSystem.current.SetSelectedGameObject(null);
            
            if(settingsButton != null)
            {
                EventSystem.current.SetSelectedGameObject(settingsButton.gameObject);
            }
        }
    }   

    //Save the settings, then close the menu
    public void ApplySettings()
    {
        PlayerPrefs.SetFloat("MusicVolume", musicVolumeSlider.value);
        PlayerPrefs.SetFloat("SFXVolume", sfxVolumeSlider.value);
        PlayerPrefs.SetFloat("Sensitivity", sensitivityValue);
        PlayerPrefs.SetInt("Bloom", System.Convert.ToInt32(bloomToggle.isOn));
        PlayerPrefs.SetInt("CRTOn", System.Convert.ToInt32(crtToggle.isOn));
        PlayerPrefs.SetFloat("CRTCurvature", crtCurvature.value);

        ToggleSettingsPanel();
    }

    //Prompt the player when they cancel out of the settings menu
    public void CancelSettings()
    {
        if (settingsPanel.activeInHierarchy)
        {   
            //Toggle the panel on or off
            cancelPanel.SetActive(!cancelPanel.activeInHierarchy);

            //Select the appropriate UI element for navigation
            if (cancelPanel.activeInHierarchy)
            {
                lastSelected = EventSystem.current.currentSelectedGameObject;

                EventSystem.current.SetSelectedGameObject(null);
                EventSystem.current.SetSelectedGameObject(saveAndQuit.gameObject);
            }
            else
            {
                EventSystem.current.SetSelectedGameObject(null);
                EventSystem.current.SetSelectedGameObject(lastSelected);
            }
        }
    }

    //Change the player reference (For changing sensitivity values)
    public void GetPlayerReference(int playerNumber = 1)
    {
        playerIndex = playerNumber - 1;
    }

    //Load the settings saved in PlayerPrefs
    private void LoadSettings()
    {
        //Load in settings from PlayerPrefs
        float musicVolume = PlayerPrefs.GetFloat("MusicVolume", 1.0f);
        float sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1.0f);
        int bloomOn = PlayerPrefs.GetInt("Bloom", 1);
        int crtOn = PlayerPrefs.GetInt("CRTOn", 1);
        float curvature = PlayerPrefs.GetFloat("CRTCurvature",0.2f);
        float sensitivity = PlayerPrefs.GetFloat("Sensitivity",3.25f);

        //Overwrite sensitivity with the current player reference (i.e. show player 2's sensitivity if they pause the game)
        if (playerIndex >= 0)
        {
            sensitivity = PlayerData.activePlayers[playerIndex].sensitivity.x;
            
            //Set Sensitivity sliders
            sensitivitySlider.value = sensitivity * 4;
        }       

        //Set Volume settings
        SoundManager.Instance.MusicVolume = musicVolume;
        musicVolumeSlider.value = musicVolume;

        SoundManager.Instance.sfxVolume = sfxVolume;
        sfxVolumeSlider.value = sfxVolume;

        //Set CRT effect based on settings
        if (crtOn == 1)
        {
            crtEffect.enabled = true;
            crtToggle.isOn = true;
        }

        else
        {
            crtEffect.enabled = false;
            crtToggle.isOn = false;
        }

        if (bloomOn == 1)
        {
            GameManager.Instance.UIManager.postProcessVolume.profile.GetSetting<Bloom>().active = true;
            bloomToggle.isOn = true;
        }

        else
        {
            GameManager.Instance.UIManager.postProcessVolume.profile.GetSetting<Bloom>().active = false;
            bloomToggle.isOn = false;
        }

        //Set curvature of the CRT effect
        crtEffect.Curvature = curvature;
        crtCurvature.value = curvature;
    }

    //Switch to the next tab in the settings
    public void NextTab()
    {
        //Return if on the last tab
        if (tabIndex == (tabs.Count - 1))
        {
            return;
        }

        if (settingsPanel.activeInHierarchy)
        {
            //Disable currently active tab
            tabs[tabIndex].SetActive(false);

            //Move to the next tab in the list
            tabIndex++;
            tabIndex = Mathf.Clamp(tabIndex, 0, tabs.Count - 1);

            //Active the next tab object
            tabs[tabIndex].SetActive(true);
            
            //Select the correct UI element based on which tab is now open
            switch(tabIndex)
            {
                case 0:
                    EventSystem.current.SetSelectedGameObject(null);
                    EventSystem.current.SetSelectedGameObject(sfxVolumeSlider.gameObject);
                    break;
                case 1:
                    EventSystem.current.SetSelectedGameObject(null);
                    EventSystem.current.SetSelectedGameObject(controlsNextMapping);
                    break;
            }
        }
    }

    //Switch to the previous tab in the menu
    public void PreviousTab()
    {
        //Return if on the first tab
        if (tabIndex == 0)
        {
            return;
        }

        if (settingsPanel.activeInHierarchy)
        {
            //Disable currently active tab
            tabs[tabIndex].SetActive(false);

            //Go the previous tab in the list
            tabIndex--;
            tabIndex = Mathf.Clamp(tabIndex, 0, tabs.Count - 1);

            //Activate the previous tab object
            tabs[tabIndex].SetActive(true);

            //Select the proper UI element based on the newly activated tab
            switch(tabIndex)
            {
                case 0:
                    EventSystem.current.SetSelectedGameObject(null);
                    EventSystem.current.SetSelectedGameObject(sfxVolumeSlider.gameObject);
                    break;
                case 1:
                    EventSystem.current.SetSelectedGameObject(null);
                    EventSystem.current.SetSelectedGameObject(controlsNextMapping);
                    break;
            }
        }
    }

    //Switch to the next mapping on the controls tab
    public void NextMapping()
    {
        if (settingsPanel.activeInHierarchy)
        {
            //Disable currently active mapping image
            controlMappings[mappingIndex].SetActive(false);

            //Go to the next mapping in the list and activate it
            mappingIndex++;
            mappingIndex = Mathf.Clamp(mappingIndex, 0, controlMappings.Count - 1);
            controlMappings[mappingIndex].SetActive(true);

            //Change the text to match the mapping shown
            switch(mappingIndex)
            {
                case 0:
                    controlsNextMapping.transform.parent.GetComponent<TextMeshProUGUI>().text = "Mouse & Keyboard";
                    break;
                case 1:
                    controlsNextMapping.transform.parent.GetComponent<TextMeshProUGUI>().text = "Gamepad 1";
                    break;
                case 2:
                    controlsNextMapping.transform.parent.GetComponent<TextMeshProUGUI>().text = "Gamepad 2";
                    break;
            }
        }
    }

    //Switch to the previous mapping in the controls menu
    public void PreviousMapping()
    {
        if (settingsPanel.activeInHierarchy)
        {
            //Disable currently active mapping
            controlMappings[mappingIndex].SetActive(false);

            //Go to the previous mapping in the list and display it
            mappingIndex--;
            mappingIndex = Mathf.Clamp(mappingIndex, 0, controlMappings.Count - 1);
            controlMappings[mappingIndex].SetActive(true);

            //Update the text to match the shown mapping
            switch(mappingIndex)
            {
                case 0:
                    controlsNextMapping.transform.parent.GetComponent<TextMeshProUGUI>().text = "Mouse & Keyboard";
                    break;
                case 1:
                    controlsNextMapping.transform.parent.GetComponent<TextMeshProUGUI>().text = "Gamepad 1";
                    break;
                case 2:
                    controlsNextMapping.transform.parent.GetComponent<TextMeshProUGUI>().text = "Gamepad 2";
                    break;
            }
        }
    }
}
