using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class BatCustomizationManager : MonoBehaviour
{
    public List<Sprite> mk1Colors;
    public List<Sprite> unstableColors;
    public List<Sprite> modColors;
    public List<Sprite> lowBonusColors;
    public List<Sprite> highBonusColors;

    private int[] selectedColors;

    public List<Image> recolorPreviewSprites;
    public Image previewImage;

    private float modBatScale = 1.2f;
    private float batScale = 1.6f;

    private void Start()
    {
        selectedColors = new int[5];
        for (int i = 0; i < selectedColors.Length; i++)
        {
            selectedColors[i] = 0;
        }
    }

    public void ChangeSelectedBat(int batIndex)
    {
        foreach(Image recolor in recolorPreviewSprites)
        {
            recolor.rectTransform.transform.localScale = new Vector3(batScale, batScale, 1);
            recolor.rectTransform.localPosition = new Vector3(0, -26, 0);
        }

        switch (batIndex)
        {
            case 0:
                previewImage.sprite = mk1Colors[selectedColors[0]];
                for (int i = 0; i < mk1Colors.Count; i++)
                {
                    recolorPreviewSprites[i].sprite = mk1Colors[i];
                }
                break;
            case 1:
                previewImage.sprite = unstableColors[selectedColors[1]];
                for (int i = 0; i < unstableColors.Count; i++)
                {
                    recolorPreviewSprites[i].sprite = unstableColors[i];
                }
                break;
            case 2:
                previewImage.sprite = modColors[selectedColors[2]];
                for (int i = 0; i < modColors.Count; i++)
                {
                    recolorPreviewSprites[i].sprite = modColors[i];
                    recolorPreviewSprites[i].rectTransform.transform.localScale = new Vector3(modBatScale, modBatScale, 1);
                    recolorPreviewSprites[i].rectTransform.localPosition = new Vector3(0, -10, 0);
                }
                break;
            case 3:
                previewImage.sprite = lowBonusColors[selectedColors[3]];
                for (int i = 0; i < lowBonusColors.Count; i++)
                {
                    recolorPreviewSprites[i].sprite = lowBonusColors[i];
                }
                break;
            case 4:
                previewImage.sprite = highBonusColors[selectedColors[4]];
                for (int i = 0; i < highBonusColors.Count; i++)
                {
                    recolorPreviewSprites[i].sprite = highBonusColors[i];
                }
                break;
        }
    }
}
