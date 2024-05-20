using System.Collections;
using System.Collections.Generic;
using RetroTVFX;
using UnityEditor.Timeline.Actions;
using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    //References to scripts that the settings will affect
    public InputManager inputManager;
    public CRTEffect crtEffect;

    public Slider volumeSlider;
    public Slider sensitivitySlider;
    public Toggle crtToggle;
    public Slider crtCurvature;

    public GameObject crtCurvatureOption;

    public GameObject settingsPanel;

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
        
        inputManager.sensitivity = sensitivity;
        sensitivitySlider.value = sensitivity;

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

        crtEffect.Curvature = curvature;
        crtCurvature.value = curvature;
    }

    // Update is called once per frame
    void Update()
    {
        SoundManager.Instance.volume = volumeSlider.value;

        crtCurvatureOption.SetActive(crtToggle.isOn);
        crtEffect.enabled = crtToggle.isOn;

        if (crtEffect.enabled)
        {
            crtEffect.Curvature = crtCurvature.value;
        }
    }

    public void ToggleSettingsPanel()
    {
        settingsPanel.SetActive(!settingsPanel.activeInHierarchy);
    }   

    public void ApplySettings()
    {
        PlayerPrefs.SetFloat("Volume", volumeSlider.value);
        PlayerPrefs.SetFloat("Sensitivity", sensitivitySlider.value);
        PlayerPrefs.SetInt("CRTOn", System.Convert.ToInt32(crtToggle.isOn));
        PlayerPrefs.SetFloat("CRTCurvature", crtCurvature.value);

        ToggleSettingsPanel();
    }
}
