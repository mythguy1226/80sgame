using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerJoinPanel : MonoBehaviour
{
    public GameObject colorSettings;
    public TMP_Dropdown colorDropdown;
    public GameObject colorSettingsOption;
    public Image crosshairPreview;
    public List<Slider> colorSliders;
    public List<TextMeshProUGUI> sliderLabels;

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
}
