using System.Collections;
using System.Collections.Generic;
using RetroTVFX;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

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
    public Slider crtCurvature;
    public Button saveAndQuit;
    public Button applyButton;

    public GameObject settingsPanel;
    public GameObject cancelPanel;
    public GameObject settingsButton;
    public List<TextMeshProUGUI> settingsLabels;

    float sensitivityValue;

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
        settingsLabels[0].text = musicVolumeLabel.ToString();
    }

    //Change SFX Volume
    public void ChangeSFXVolume()
    {
        SoundManager.Instance.sfxVolume = sfxVolumeSlider.value;
        float sfxVolumeLabel = Mathf.RoundToInt(sfxVolumeSlider.value * 100);
        settingsLabels[3].text = sfxVolumeLabel.ToString();
    }

    //Change the sensitivity for the player that paused the game 
    public void ChangeSensitivity()
    {
        sensitivityValue = sensitivitySlider.value / 4;
        PlayerData.activePlayers[playerIndex].sensitivity = new Vector2(sensitivityValue, sensitivityValue);
        float sensitivityLabel = sensitivitySlider.value / 4;
        settingsLabels[1].text = sensitivityLabel.ToString("F2");
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
        settingsLabels[2].text = crtCurvatureLabel.ToString();
    }

    //Toggle the menu on and off
    public void ToggleSettingsPanel()
    {
        settingsPanel.SetActive(!settingsPanel.activeInHierarchy);
        cancelPanel.SetActive(false);
        
        //Get which player paused the game to determine who is changing settings
        GetPlayerReference(PauseScreenBehavior.Instance.playerIndex + 1);
        LoadSettings();

        //Select the proper UI element for navigation
        if (settingsPanel.activeInHierarchy)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(musicVolumeSlider.gameObject);
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
                EventSystem.current.SetSelectedGameObject(null);
                EventSystem.current.SetSelectedGameObject(saveAndQuit.gameObject);
            }
            else
            {
                EventSystem.current.SetSelectedGameObject(null);
                EventSystem.current.SetSelectedGameObject(applyButton.gameObject);
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
        int crtOn = PlayerPrefs.GetInt("CRTOn", 1);
        float curvature = PlayerPrefs.GetFloat("CRTCurvature",0.2f);
        float sensitivity = PlayerPrefs.GetFloat("Sensitivity",20);

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

        //Set curvature of the CRT effect
        crtEffect.Curvature = curvature;
        crtCurvature.value = curvature;
    }
}
