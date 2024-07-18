using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;

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
    public ScoreBehavior scoreBehavior;

    public List<Sprite> classicModeBackgrounds;
    public List<Sprite> defenseModeBackgrounds;

    public SpriteRenderer background;

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
    }

    private void Start()
    {
        SetBackground();
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
            background.gameObject.transform.localScale = new Vector3(6.5f, 6.5f, 1f);
        }

        else if (GameModeData.activeGameMode == EGameMode.Classic)
        {
            background.sprite = GameManager.Instance.UIManager.classicModeBackgrounds[PlayerPrefs.GetInt("ClassicBackground")];
            background.gameObject.transform.localScale = new Vector3(6.5f, 6.5f, 1f);
        }

        else if (GameModeData.activeGameMode == EGameMode.Competitive)
        {
            background.sprite = GameManager.Instance.UIManager.classicModeBackgrounds[PlayerPrefs.GetInt("CompetitiveBackground")];
            background.gameObject.transform.localScale = new Vector3(6.5f, 6.5f, 1f);
        }
    }
}