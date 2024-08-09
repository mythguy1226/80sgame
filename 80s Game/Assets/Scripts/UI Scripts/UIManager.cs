using System;
using System.Collections.Generic;
using TMPro;
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

    public OnboardingUI onboardingUI;
    private PauseScreenBehavior pauseScreenUI;
    private GameOverBehavior gameOverUI;
    public TitleScreenBehavior titleScreenUI;
    public ScoreBehavior scoreBehavior;
    public BackgroundCustomization backgroundUI;
    public AchievementsUI achievementsUI;

    public List<Sprite> classicModeBackgrounds;
    public List<Sprite> defenseModeBackgrounds;

    public List<GameObject> gamemodeCards;

    public GameObject achievementNotifPrefab;
    private Queue<AchievementNotificationData> achievementNotifs;
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
        achievementNotifs = new Queue<AchievementNotificationData>();
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
                AchievementNotificationData data = achievementNotifs.Dequeue();
                GameObject clone = Instantiate(achievementNotifPrefab, canvas.transform);
                clone.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = data.text;
                clone.transform.GetChild(3).GetComponent<Image>().sprite = data.image;
                notifPlaying = true;
            }        
        }
    }

    public void GetFireInput(Vector3 screenPosition)
    {
        switch (activeUI)
        {
            case UIType.Onboarding:
                //onboardingUI.CloseOnboarding();
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
        
        //Set background for defense mode
        if (GameModeData.activeGameMode == EGameMode.Defense)
        {
            background.sprite = GameManager.Instance.UIManager.defenseModeBackgrounds[PlayerPrefs.GetInt("DefenseBackground")];
        }

        //Set background for classic mode
        else if (GameModeData.activeGameMode == EGameMode.Classic)
        {
            background.sprite = GameManager.Instance.UIManager.classicModeBackgrounds[PlayerPrefs.GetInt("ClassicBackground")];
        }

        //Set background for competitive mode
        else if (GameModeData.activeGameMode == EGameMode.Competitive)
        {
            background.sprite = GameManager.Instance.UIManager.classicModeBackgrounds[PlayerPrefs.GetInt("CompetitiveBackground")];
        }
    }

    //Cycle through the different backgrounds that can be customized
    public void BackgroundCycle(Vector2 movement)
    {
        //Only change the background once if the user holds up or down on the stick/d-pad
        if (!backgroundChanged)
        {
            //Check if any of the gamemode cards are selected
            for(int i = 0; i < gamemodeCards.Count; i++)
            {
                if (EventSystem.current.currentSelectedGameObject == gamemodeCards[i])
                {
                    //If it is selected and the user presses up, go to the previous background
                    if (movement.y >= 0.7f)
                    {
                        backgroundUI.PreviousBackground(i + 1);
                        backgroundChanged = true;
                    }

                    //If the user presses down, go to the next background
                    else if (movement.y <= -0.7f)
                    {
                        backgroundUI.NextBackground(i + 1);
                        backgroundChanged = true;
                    }
                }
            }
        }

        //Allow the background to be changed again once there is no movement from the user
        if (movement == Vector2.zero)
        {
            backgroundChanged = false;
        }
    }

    //Cancel out of a menu on the title screen when pressing the cancel input
    public void CancelMenu()
    {
        if (titleScreenUI == null)
        {
            return; 
        }
        
        //Cancel out of achievements screen if it's active
        if (titleScreenUI.achievementsPanel.activeInHierarchy)
        {
            titleScreenUI.ToggleAchievementsPanel();
        }

        //Cancel out of gamemode screen if it's active
        else if (titleScreenUI.gamemodePanel.activeInHierarchy)
        {
            titleScreenUI.ToggleGamemodeSelection();
        }

        //Cancel out of credits screen if it's active
        else if (titleScreenUI.creditsScreen.activeInHierarchy)
        {
            titleScreenUI.ToggleCredisPanel();
        }
    }

    //Show an achievement notification when it is unlocked
    public void EnqueueAchievementNotification(AchievementData achievement)
    {
        AchievementNotificationData notificationData = new AchievementNotificationData(achievement.nameText, achievement.image);
        achievementNotifs.Enqueue(notificationData);
    }

    public void ClearNotification()
    {
        notifPlaying = false;
    }

    public void LoadAchievements(List<AchievementData> achievements)
    {
        if (achievementsUI != null)
        {
            achievementsUI.CreateAchievements(achievements);
        }
    }
}