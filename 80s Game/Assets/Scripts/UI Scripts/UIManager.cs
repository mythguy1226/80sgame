using UnityEngine;

public class UIManager : MonoBehaviour
{
    public enum UIType
    {
        None,
        Onboarding,
        Pause,
        GameOver
    }


    public UIType activeUI;
    public Canvas canvas;

    private OnboardingUI onboardingUI;
    private PauseScreenBehavior pauseScreenUI;
    private GameOverBehavior gameOverUI;

    private void Awake()
    {
        onboardingUI = canvas.GetComponent<OnboardingUI>();
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
}