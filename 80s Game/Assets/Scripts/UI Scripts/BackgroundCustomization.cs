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
                selectedClassicBackground = ChangeBackground(selectedClassicBackground, GameManager.Instance.UIManager.classicModeBackgrounds, true);
                classicBackgroundPreview.sprite = GameManager.Instance.UIManager.classicModeBackgrounds[selectedClassicBackground];
                if (BackgroundLocked(1))
                {
                    classicBackgroundPreview.gameObject.transform.GetChild(1).gameObject.SetActive(true);
                    return;
                }
                else
                {
                    classicBackgroundPreview.gameObject.transform.GetChild(1).gameObject.SetActive(false);
                }

                PlayerPrefs.SetInt("ClassicBackground", selectedClassicBackground);
                break;

            case 2:
                selectedCompetitiveBackground = ChangeBackground(selectedCompetitiveBackground, GameManager.Instance.UIManager.classicModeBackgrounds, true);
                competitiveBackgroundPreview.sprite = GameManager.Instance.UIManager.classicModeBackgrounds[selectedCompetitiveBackground];
                if (BackgroundLocked(2))
                {
                    competitiveBackgroundPreview.gameObject.transform.GetChild(4).gameObject.SetActive(true);
                    return;
                }
                else
                {
                    competitiveBackgroundPreview.gameObject.transform.GetChild(4).gameObject.SetActive(false);
                }
                
                PlayerPrefs.SetInt("CompetitiveBackground", selectedCompetitiveBackground);
                break;

            case 3:
                selectedDefenseBackground = ChangeBackground(selectedDefenseBackground, GameManager.Instance.UIManager.defenseModeBackgrounds, true);
                defenseBackgroundPreview.sprite = GameManager.Instance.UIManager.defenseModeBackgrounds[selectedDefenseBackground];
                if (BackgroundLocked(3))
                {
                    defenseBackgroundPreview.gameObject.transform.GetChild(1).gameObject.SetActive(true);
                    return;
                }
                else
                {
                    defenseBackgroundPreview.gameObject.transform.GetChild(1).gameObject.SetActive(false);
                }

                PlayerPrefs.SetInt("DefenseBackground", selectedDefenseBackground);
                break;
        }
    }

    public void PreviousBackground(int gamemode)
    {
        switch(gamemode)
        {
            case 1:
                selectedClassicBackground = ChangeBackground(selectedClassicBackground, GameManager.Instance.UIManager.classicModeBackgrounds, false);
                classicBackgroundPreview.sprite = GameManager.Instance.UIManager.classicModeBackgrounds[selectedClassicBackground];
                if (BackgroundLocked(1))
                {
                    classicBackgroundPreview.gameObject.transform.GetChild(1).gameObject.SetActive(true);
                    return;
                }
                else
                {
                    classicBackgroundPreview.gameObject.transform.GetChild(1).gameObject.SetActive(false);
                }

                PlayerPrefs.SetInt("ClassicBackground", selectedClassicBackground);
                break;

            case 2:
                selectedCompetitiveBackground = ChangeBackground(selectedCompetitiveBackground, GameManager.Instance.UIManager.classicModeBackgrounds, false);
                competitiveBackgroundPreview.sprite = GameManager.Instance.UIManager.classicModeBackgrounds[selectedCompetitiveBackground];
                if (BackgroundLocked(2))
                {
                    competitiveBackgroundPreview.gameObject.transform.GetChild(4).gameObject.SetActive(true);
                    return;
                }
                else
                {
                    competitiveBackgroundPreview.gameObject.transform.GetChild(4).gameObject.SetActive(false);
                }

                PlayerPrefs.SetInt("CompetitiveBackground", selectedCompetitiveBackground);
                break;

            case 3:
                selectedDefenseBackground = ChangeBackground(selectedDefenseBackground, GameManager.Instance.UIManager.defenseModeBackgrounds, false);
                defenseBackgroundPreview.sprite = GameManager.Instance.UIManager.defenseModeBackgrounds[selectedDefenseBackground];
                if (BackgroundLocked(3))
                {
                    defenseBackgroundPreview.gameObject.transform.GetChild(1).gameObject.SetActive(true);
                    return;
                }
                else
                {
                    defenseBackgroundPreview.gameObject.transform.GetChild(1).gameObject.SetActive(false);
                }
                
                PlayerPrefs.SetInt("DefenseBackground", selectedDefenseBackground);
                break;
        }
    }

    private int ChangeBackground(int selectedBackground, List<Sprite> backgrounds, bool increase)
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

    public bool BackgroundLocked(int gamemode)
    {
        switch (gamemode)
        {
            case 1:
                switch(selectedClassicBackground)
                {
                    case 0:
                        return false;
                    case 1:
                        return !AchievementManager.GetAchievementByKey("classic-1").isUnlocked();
                    case 2:
                        return !AchievementManager.GetAchievementByKey("classic-2").isUnlocked();
                    case 3:
                        return !AchievementManager.GetAchievementByKey("classic-3").isUnlocked();
                    default:
                        return true;
                }
            case 2:
                switch(selectedCompetitiveBackground)
                {
                    case 0:
                        return false;
                    case 1:
                        return !AchievementManager.GetAchievementByKey("classic-1").isUnlocked();
                    case 2:
                        return !AchievementManager.GetAchievementByKey("classic-2").isUnlocked();
                    case 3:
                        return !AchievementManager.GetAchievementByKey("classic-3").isUnlocked();
                    default:
                        return true;
                }
            case 3:
                switch(selectedDefenseBackground)
                {
                    case 0:
                        return false;
                    case 1:
                        return !AchievementManager.GetAchievementByKey("def-1").isUnlocked();
                    case 2:
                        return !AchievementManager.GetAchievementByKey("def-2").isUnlocked();
                    case 3:
                        return !AchievementManager.GetAchievementByKey("def-3").isUnlocked();
                    default:
                        return true;
                }
            default:
                return true;
        }
    }
}
