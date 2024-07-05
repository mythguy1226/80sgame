using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class OnboardingUI : MonoBehaviour
{
    //Fields for object references
    public GameObject onboardingPanel;
    public GameObject gameUIElements;
    public GameObject mouseDiagram;
    public GameObject xboxControllerDiagram;
    public GameObject playstationControllerDiagram;
    [SerializeField] AudioClip gameStartTheme;
    private UIManager manager;
    private bool controllerConnected = false;


    // Start is called before the first frame update
    void Start()
    {
        //Pause game when onboarding panel is activated
        Time.timeScale = 0.0f;

        if (PlayerData.activePlayers[0].controlScheme == "KnM")
        {
            mouseDiagram.SetActive(true);
        }

        else if (PlayerData.activePlayers[0].controlScheme == "xbox")
        {
            xboxControllerDiagram.SetActive(true);
        }

        else
        {
            playstationControllerDiagram.SetActive(true);
        }
    }

    //Close the panel, active game UI elements, and unpause the game
    public void CloseOnboarding()
    {
        SoundManager.Instance.PlayNonloopMusic(gameStartTheme);
        onboardingPanel.SetActive(false);
        gameUIElements.SetActive(true);

        PauseScreenBehavior.Instance.ToggleCrosshairs(true);

        Time.timeScale = 1.0f;
        manager.activeUI = UIManager.UIType.None;
    }

    public void SetManager(UIManager reference)
    {
        manager = reference;
    }
}
