using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackgroundCustomization : MonoBehaviour
{
    public Image classicBackgroundPreview;
    public Image competitiveBackgroundPreview;
    public Image defenseBackgroundPreview;

    private int selectedClassicBackground = 0;
    private int selectedCompetitiveBackground = 0;
    private int selectedDefenseBackground = 0;
    
    public void Start()
    {
        LoadBackgrounds();
    }

    public void NextBackground(int gamemode)
    {
        switch(gamemode)
        {
            case 1:
                selectedClassicBackground = ChangeBackground(selectedClassicBackground, GameManager.Instance.UIManager.classicModeBackgrounds, classicBackgroundPreview, true);
                classicBackgroundPreview.sprite = GameManager.Instance.UIManager.classicModeBackgrounds[selectedClassicBackground];

                PlayerPrefs.SetInt("ClassicBackground", selectedClassicBackground);
                break;

            case 2:
                selectedCompetitiveBackground = ChangeBackground(selectedCompetitiveBackground, GameManager.Instance.UIManager.classicModeBackgrounds, competitiveBackgroundPreview, true);
                competitiveBackgroundPreview.sprite = GameManager.Instance.UIManager.classicModeBackgrounds[selectedCompetitiveBackground];

                PlayerPrefs.SetInt("CompetitiveBackground", selectedCompetitiveBackground);
                break;

            case 3:
                selectedDefenseBackground = ChangeBackground(selectedDefenseBackground, GameManager.Instance.UIManager.defenseModeBackgrounds, defenseBackgroundPreview, true);
                defenseBackgroundPreview.sprite = GameManager.Instance.UIManager.defenseModeBackgrounds[selectedDefenseBackground];

                PlayerPrefs.SetInt("DefenseBackground", selectedDefenseBackground);
                break;
        }
    }

    public void PreviousBackground(int gamemode)
    {
        switch(gamemode)
        {
            case 1:
                selectedClassicBackground = ChangeBackground(selectedClassicBackground, GameManager.Instance.UIManager.classicModeBackgrounds, classicBackgroundPreview, false);
                classicBackgroundPreview.sprite = GameManager.Instance.UIManager.classicModeBackgrounds[selectedClassicBackground];

                PlayerPrefs.SetInt("ClassicBackground", selectedClassicBackground);
                break;

            case 2:
                selectedCompetitiveBackground = ChangeBackground(selectedCompetitiveBackground, GameManager.Instance.UIManager.classicModeBackgrounds, competitiveBackgroundPreview, false);
                competitiveBackgroundPreview.sprite = GameManager.Instance.UIManager.classicModeBackgrounds[selectedCompetitiveBackground];

                PlayerPrefs.SetInt("CompetitiveBackground", selectedCompetitiveBackground);
                break;

            case 3:
                selectedDefenseBackground = ChangeBackground(selectedDefenseBackground, GameManager.Instance.UIManager.defenseModeBackgrounds, defenseBackgroundPreview, false);
                defenseBackgroundPreview.sprite = GameManager.Instance.UIManager.defenseModeBackgrounds[selectedDefenseBackground];

                PlayerPrefs.SetInt("DefenseBackground", selectedDefenseBackground);
                break;
        }
    }

    private int ChangeBackground(int selectedBackground, List<Sprite> backgrounds, Image background, bool increase)
    {
        if (increase)
        {
            selectedBackground++;
            if(selectedBackground == backgrounds.Count)
            {
                selectedBackground = 0;
            }
            return selectedBackground;
        }

        else
        {
            selectedBackground--;
            if(selectedBackground == -1)
            {
                selectedBackground = backgrounds.Count - 1;
            }
            return selectedBackground;
        }
    }

    private void LoadBackgrounds()
    {
        selectedClassicBackground = PlayerPrefs.GetInt("ClassicBackground");
        classicBackgroundPreview.sprite = GameManager.Instance.UIManager.classicModeBackgrounds[selectedClassicBackground];

        selectedCompetitiveBackground = PlayerPrefs.GetInt("CompetitiveBackground");
        competitiveBackgroundPreview.sprite = GameManager.Instance.UIManager.classicModeBackgrounds[selectedCompetitiveBackground];

        selectedDefenseBackground = PlayerPrefs.GetInt("DefenseBackground");
        defenseBackgroundPreview.sprite = GameManager.Instance.UIManager.defenseModeBackgrounds[selectedDefenseBackground];
    }
}
