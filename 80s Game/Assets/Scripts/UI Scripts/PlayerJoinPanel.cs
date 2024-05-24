using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerJoinPanel : MonoBehaviour
{
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

    void Start()
    {
        crosshairLeftArrow.onClick.AddListener(() => ChangeCrosshairSprite(false));
        crosshairRightArrow.onClick.AddListener(() => ChangeCrosshairSprite(true));

        TextMeshProUGUI crosshairText = crosshairPreview.gameObject.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
        crosshairText.text = (crosshairIndex + 1) + "/" + crosshairSprites.Count;
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
}
