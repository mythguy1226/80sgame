using System.Collections;
using System.Collections.Generic;
using RetroTVFX;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance { get; private set; }

    //References to scripts that the settings will affect
    public PlayerInputWrapper playerInputWrapper;
    public CRTEffect crtEffect;

    //References to UI Elements
    public Slider volumeSlider;
    public Slider mouseXSlider;
    public Slider mouseYSlider;
    public Slider gamepadXSlider;
    public Slider gamepadYSlider;
    public Toggle crtToggle;
    public Slider crtCurvature;


    public GameObject crtCurvatureOption;
    public GameObject settingsPanel;
    public GameObject settingsButton;
    public List<TextMeshProUGUI> settingsLabels;

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

        GetPlayerReference();
        this.GetComponent<PauseScreenBehavior>().ToggleCrosshairs(false);

        //Load in settings from PlayerPrefs
        float volume = PlayerPrefs.GetFloat("Volume");
        int crtOn = PlayerPrefs.GetInt("CRTOn");
        float curvature = PlayerPrefs.GetFloat("CRTCurvature");
        float mouseX = PlayerPrefs.GetFloat("MouseXSensitivity");
        float mouseY = PlayerPrefs.GetFloat("MouseYSensitivity");
        float gamepadX = PlayerPrefs.GetFloat("GamepadXSensitivity");
        float gamepadY = PlayerPrefs.GetFloat("GamepadYSensitivity");

        //Set Volume settings
        SoundManager.Instance.volume = volume;
        volumeSlider.value = volume;

        //Set Sensitivity sliders
        mouseXSlider.value = mouseX;
        mouseYSlider.value = mouseY;
        gamepadXSlider.value = gamepadX;
        gamepadYSlider.value = gamepadY;

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

    //Change Volume
    public void ChangeVolume()
    {
        SoundManager.Instance.volume = volumeSlider.value;
        float volumeLabel = Mathf.RoundToInt(volumeSlider.value * 100);
        settingsLabels[0].text = volumeLabel.ToString();
    }

    public void ChangeMouseSensitivityX()
    {
        playerInputWrapper.mouseSensitivity.x = mouseXSlider.value;
        playerInputWrapper.SetSensitivity(false);

        float sensitivityLabel = mouseXSlider.value * 50;
        settingsLabels[1].text = sensitivityLabel.ToString("F2");
    }

    public void ChangeMouseSensitivityY()
    {
        playerInputWrapper.mouseSensitivity.y = mouseYSlider.value;
        playerInputWrapper.SetSensitivity(false);

        float sensitivityLabel = mouseYSlider.value * 50;
        settingsLabels[2].text = sensitivityLabel.ToString("F2");
    }

    public void ChangeGamepadSensitivityX()
    {
        playerInputWrapper.controllerSensitivity.x = gamepadXSlider.value;
        playerInputWrapper.SetSensitivity(true);

        float sensitivityLabel = gamepadXSlider.value * 5;
        settingsLabels[3].text = sensitivityLabel.ToString("F2");
    }

    public void ChangeGamepadSensitivityY()
    {
        playerInputWrapper.controllerSensitivity.y = gamepadYSlider.value;
        playerInputWrapper.SetSensitivity(true);

        float sensitivityLabel = gamepadYSlider.value * 5;
        settingsLabels[4].text = sensitivityLabel.ToString("F2");
    }

    public void ToggleCRTEffect()
    {
        crtEffect.enabled = crtToggle.isOn;
        crtCurvatureOption.SetActive(crtToggle.isOn);
    }

    public void ChangeCRTCurvature()
    {
        if (crtEffect.enabled)
        {
            crtEffect.Curvature = crtCurvature.value;
            float crtCurvatureLabel = Mathf.RoundToInt(crtCurvature.value * 500);
            settingsLabels[5].text = crtCurvatureLabel.ToString();
        }
    }

    //Toggle the menu on and off
    public void ToggleSettingsPanel()
    {
        settingsPanel.SetActive(!settingsPanel.activeInHierarchy);

        if (settingsPanel.activeInHierarchy)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(volumeSlider.gameObject);
        }

        else
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(settingsButton.gameObject);
        }
    }   

    //Save the settings, then close the menu
    public void ApplySettings()
    {
        PlayerPrefs.SetFloat("Volume", volumeSlider.value);
        PlayerPrefs.SetFloat("MouseXSensitivity", mouseXSlider.value);
        PlayerPrefs.SetFloat("MouseYSensitivity", mouseYSlider.value);
        PlayerPrefs.SetFloat("GamepadXSensitivity", gamepadXSlider.value);
        PlayerPrefs.SetFloat("GamepadYSensitivity", gamepadYSlider.value);
        PlayerPrefs.SetInt("CRTOn", System.Convert.ToInt32(crtToggle.isOn));
        PlayerPrefs.SetFloat("CRTCurvature", crtCurvature.value);

        ToggleSettingsPanel();
    }

    public void GetPlayerReference(int playerNumber = 1)
    {
        //Get Reference to PlayerInputWrapper
        playerInputWrapper = PlayerData.activePlayers[playerNumber - 1].GetComponent<PlayerInputWrapper>();
    }
}
