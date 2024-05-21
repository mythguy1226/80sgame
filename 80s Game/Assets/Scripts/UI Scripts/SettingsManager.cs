using System.Collections;
using System.Collections.Generic;
using RetroTVFX;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class SettingsManager : MonoBehaviour
{
    //References to scripts that the settings will affect
    public InputManager inputManager;
    public CRTEffect crtEffect;

    //References to UI Elements
    public Slider volumeSlider;
    public Slider sensitivitySlider;
    public Toggle crtToggle;
    public Slider crtCurvature;

    //Reference to curvature option and menu panel
    public GameObject crtCurvatureOption;

    public GameObject settingsPanel;
    public GameObject settingsButton;

    public List<TextMeshProUGUI> settingsLabels;

    //Fields for each of the settings
    private float volume;
    private float sensitivity;
    private int crtOn = 1;
    private float curvature;

    // Start is called before the first frame update
    void Start()
    {
        //Load in settings from PlayerPrefs
        volume = PlayerPrefs.GetFloat("Volume");
        sensitivity = PlayerPrefs.GetFloat("Sensitivity");
        crtOn = PlayerPrefs.GetInt("CRTOn");
        curvature = PlayerPrefs.GetFloat("CRTCurvature");

        //Set Volume settings
        SoundManager.Instance.volume = volume;
        volumeSlider.value = volume;
        
        //Set sensitivity settings
        inputManager.sensitivity = sensitivity;
        sensitivitySlider.value = sensitivity;

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

    public void ChangeSensitivity()
    {
        inputManager.sensitivity = sensitivitySlider.value;
        float sensitivityLabel = Mathf.RoundToInt(sensitivitySlider.value * 10);
        settingsLabels[1].text = sensitivityLabel.ToString();
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
            settingsLabels[2].text = crtCurvatureLabel.ToString();
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
        PlayerPrefs.SetFloat("Sensitivity", sensitivitySlider.value);
        PlayerPrefs.SetInt("CRTOn", System.Convert.ToInt32(crtToggle.isOn));
        PlayerPrefs.SetFloat("CRTCurvature", crtCurvature.value);

        ToggleSettingsPanel();
    }
}
