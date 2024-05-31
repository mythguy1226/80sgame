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
    public Slider volumeSlider;
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
        this.GetComponent<PauseScreenBehavior>().ToggleCrosshairs(false);
        playerIndex = -1;

        LoadSettings();
    }

    //Change Volume
    public void ChangeVolume()
    {
        SoundManager.Instance.Volume = volumeSlider.value;
        float volumeLabel = Mathf.RoundToInt(volumeSlider.value * 100);
        settingsLabels[0].text = volumeLabel.ToString();
    }

    public void ChangeSensitivity()
    {
        sensitivityValue = sensitivitySlider.value / 4;
        PlayerData.activePlayers[playerIndex].sensitivity = new Vector2(sensitivityValue, sensitivityValue);
        float sensitivityLabel = sensitivitySlider.value / 4;
        settingsLabels[1].text = sensitivityLabel.ToString("F2");
    }

    public void ToggleCRTEffect()
    {
        crtEffect.enabled = crtToggle.isOn;
    }

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
        
        GetPlayerReference(PauseScreenBehavior.Instance.playerIndex + 1);
        LoadSettings();

        if (settingsPanel.activeInHierarchy)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(volumeSlider.gameObject);
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
        PlayerPrefs.SetFloat("Volume", volumeSlider.value);
        PlayerPrefs.SetFloat("Sensitivity", sensitivityValue);
        PlayerPrefs.SetInt("CRTOn", System.Convert.ToInt32(crtToggle.isOn));
        PlayerPrefs.SetFloat("CRTCurvature", crtCurvature.value);

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
        playerIndex = playerNumber - 1;
    }

    private void LoadSettings()
    {
        //Load in settings from PlayerPrefs
        float volume = PlayerPrefs.GetFloat("Volume", 1.0f);
        int crtOn = PlayerPrefs.GetInt("CRTOn", 1);
        float curvature = PlayerPrefs.GetFloat("CRTCurvature",0.2f);
        float sensitivity = PlayerPrefs.GetFloat("Sensitivity",20);
        
        Debug.Log(playerIndex);

        if (playerIndex >= 0)
        {
            sensitivity = PlayerData.activePlayers[playerIndex].sensitivity.x;
            
            //Set Sensitivity sliders
            sensitivitySlider.value = sensitivity * 4;
        }       

        //Set Volume settings
        SoundManager.Instance.Volume = volume;
        volumeSlider.value = volume;

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
