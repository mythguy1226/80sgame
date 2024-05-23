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
    public Button saveAndQuit;
    public Button applyButton;

    public GameObject settingsPanel;
    public GameObject cancelPanel;
    public GameObject settingsButton;
    public List<TextMeshProUGUI> settingsLabels;

    float mouseSensitivityX;
    float mouseSensitivityY;
    float gamepadSensitivityX;
    float gamepadSensitivityY;

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

        LoadSettings();
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
        mouseSensitivityX = mouseXSlider.value / 400;
        playerInputWrapper.mouseSensitivity.x = mouseSensitivityX;

        float sensitivityLabel = mouseXSlider.value / 4;
        settingsLabels[1].text = sensitivityLabel.ToString("F2");
    }

    public void ChangeMouseSensitivityY()
    {
        mouseSensitivityY = mouseYSlider.value / 400;
        playerInputWrapper.mouseSensitivity.y = mouseSensitivityY;        

        float sensitivityLabel = mouseYSlider.value / 4;
        settingsLabels[2].text = sensitivityLabel.ToString("F2");
    }

    public void ChangeGamepadSensitivityX()
    {
        gamepadSensitivityX = gamepadXSlider.value / 40;
        playerInputWrapper.controllerSensitivity.x = gamepadSensitivityX;

        float sensitivityLabel = gamepadXSlider.value / 4;
        settingsLabels[3].text = sensitivityLabel.ToString("F2");
    }

    public void ChangeGamepadSensitivityY()
    {
        gamepadSensitivityY = gamepadYSlider.value / 40;
        playerInputWrapper.controllerSensitivity.y = gamepadSensitivityY;

        float sensitivityLabel = gamepadYSlider.value / 4;
        settingsLabels[4].text = sensitivityLabel.ToString("F2");
    }

    public void ToggleCRTEffect()
    {
        crtEffect.enabled = crtToggle.isOn;
    }

    public void ChangeCRTCurvature()
    {
        crtEffect.Curvature = crtCurvature.value;
        float crtCurvatureLabel = Mathf.RoundToInt(crtCurvature.value * 500);
        settingsLabels[5].text = crtCurvatureLabel.ToString();
    }

    //Toggle the menu on and off
    public void ToggleSettingsPanel()
    {
        settingsPanel.SetActive(!settingsPanel.activeInHierarchy);
        cancelPanel.SetActive(false);
        
        LoadSettings();

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
        PlayerPrefs.SetFloat("MouseXSensitivity", mouseSensitivityX);
        PlayerPrefs.SetFloat("MouseYSensitivity", mouseSensitivityY);
        PlayerPrefs.SetFloat("GamepadXSensitivity", gamepadSensitivityX);
        PlayerPrefs.SetFloat("GamepadYSensitivity", gamepadSensitivityY);
        PlayerPrefs.SetInt("CRTOn", System.Convert.ToInt32(crtToggle.isOn));
        PlayerPrefs.SetFloat("CRTCurvature", crtCurvature.value);

        playerInputWrapper.SetSensitivity(playerInputWrapper.controllerInput);

        ToggleSettingsPanel();
    }

    public void CancelSettings()
    {
        if (settingsPanel.activeInHierarchy)
        {
            cancelPanel.SetActive(!cancelPanel.activeInHierarchy);

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

    public void GetPlayerReference(int playerNumber = 1)
    {
        //Get Reference to PlayerInputWrapper
        playerInputWrapper = PlayerData.activePlayers[playerNumber - 1].GetComponent<PlayerInputWrapper>();
    }

    private void LoadSettings()
    {
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
        mouseXSlider.value = mouseX * 400;
        mouseYSlider.value = mouseY * 400;
        gamepadXSlider.value = gamepadX * 40;
        gamepadYSlider.value = gamepadY * 40;

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
