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

    private List<int> selectedColors;

    public List<Image> recolorPreviewSprites;
    public Image previewImage;

    public void ChangeSelectedBat(int batIndex)
    {
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
