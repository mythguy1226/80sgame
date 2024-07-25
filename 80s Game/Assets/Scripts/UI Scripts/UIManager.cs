using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;
//using UnityEngine.WSA;

public class UIManager : MonoBehaviour
{

    public static Action<int> pauseEvent;
    public enum UIType
    {
        None,
        Onboarding,
        Pause,
        GameOver
    }


    public UIType activeUI;
    public Canvas canvas;

    public List<GameObject> modifierContainers;
    public PostProcessVolume postProcessVolume;

    private OnboardingUI onboardingUI;
    private PauseScreenBehavior pauseScreenUI;
    private GameOverBehavior gameOverUI;
    public TitleScreenBehavior titleScreenUI;
    public ScoreBehavior scoreBehavior;
    public BackgroundCustomization backgroundUI;

    public List<Sprite> classicModeBackgrounds;
    public List<Sprite> defenseModeBackgrounds;

    public List<GameObject> gamemodeCards;

    public GameObject achievementNotifPrefab;
    private Queue<GameObject> achievementNotifs;
    private bool notifPlaying = false;

    public SpriteRenderer background;
    private bool backgroundChanged = false;

    private void Awake()
    {
        onboardingUI = canvas.GetComponent<OnboardingUI>();
        if (onboardingUI)
        {
            onboardingUI.SetManager(this);
        }
        pauseScreenUI = canvas.GetComponent<PauseScreenBehavior>();
        gameOverUI = canvas.GetComponent<GameOverBehavior>();
        scoreBehavior = canvas.GetComponent<ScoreBehavior>();
        titleScreenUI = canvas.GetComponent<TitleScreenBehavior>();
        achievementNotifs = new Queue<GameObject>();
    }

    private void Start()
    {
        SetBackground();
    }

    private void Update()
    {
        if (achievementNotifs != null)
        {
            if(achievementNotifs.Count > 0 && !notifPlaying)
            {
                Instantiate(achievementNotifs.Dequeue(), canvas.transform);
                notifPlaying = true;
            }        
        }
    }

    public void GetFireInput(Vector3 screenPosition)
    {
        switch (activeUI)
        {
            case UIType.Onboarding:
                onboardingUI.CloseOnboarding();
                break;
            case UIType.Pause:
                // Test each element and see wtf?
                break;
            case UIType.GameOver:
                break;
            default:
                return;
        }
    }

    public static void PlayerPause(int player)
    {
        pauseEvent?.Invoke(player);
    }

    //Create status effect UI for the appropriate modifier and proper player
    public GameObject CreateModifierUI(GameObject uiPrefab, int player)
    {
        return Instantiate(uiPrefab, modifierContainers[player].transform);
    }

    private void SetBackground()
    {
        if (background == null)
        {
            return;
        }
        
        //Set background to defense if defense mode is selected
        if (GameModeData.activeGameMode == EGameMode.Defense)
        {
            background.sprite = GameManager.Instance.UIManager.defenseModeBackgrounds[PlayerPrefs.GetInt("DefenseBackground")];
        }

        else if (GameModeData.activeGameMode == EGameMode.Classic)
        {
            background.sprite = GameManager.Instance.UIManager.classicModeBackgrounds[PlayerPrefs.GetInt("ClassicBackground")];
        }

        else if (GameModeData.activeGameMode == EGameMode.Competitive)
        {
            background.sprite = GameManager.Instance.UIManager.classicModeBackgrounds[PlayerPrefs.GetInt("CompetitiveBackground")];
        }
    }

    public void BackgroundCycle(Vector2 movement)
    {
        if (!backgroundChanged)
        {
            for(int i = 0; i < gamemodeCards.Count; i++)
            {
                if (EventSystem.current.currentSelectedGameObject == gamemodeCards[i])
                {
                    if (movement.y >= 0.7f)
                    {
                        backgroundUI.PreviousBackground(i + 1);
                        backgroundChanged = true;
                    }

                    else if (movement.y <= -0.7f)
                    {
                        backgroundUI.NextBackground(i + 1);
                        backgroundChanged = true;
                    }
                }
            }
        }

        if (movement == Vector2.zero)
        {
            backgroundChanged = false;
        }
    }

    public void CancelMenu()
    {
        if (titleScreenUI == null)
        {
            return; 
        }
        
        if (titleScreenUI.achievementsPanel.activeInHierarchy)
        {
            titleScreenUI.ToggleAchievementsPanel();
        }

        else if (titleScreenUI.gamemodePanel.activeInHierarchy)
        {
            titleScreenUI.ToggleGamemodeSelection();
        }
    }

    public void ShowAchievementNotification(AchievementData achievement)
    {
        achievementNotifPrefab.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = achievement.nameText;

        achievementNotifs.Enqueue(achievementNotifPrefab);
        //Instantiate(achievementNotifs[0], canvas.transform);
    }

    public void ClearNotification()
    {
        notifPlaying = false;
    }
}