using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

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

    private void Awake()
    {
        onboardingUI = canvas.GetComponent<OnboardingUI>();
        if (onboardingUI)
        {
            onboardingUI.SetManager(this);
        }
        pauseScreenUI = canvas.GetComponent<PauseScreenBehavior>();
        gameOverUI = canvas.GetComponent<GameOverBehavior>();
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
}