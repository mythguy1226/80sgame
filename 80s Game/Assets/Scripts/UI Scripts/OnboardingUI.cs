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
    public GameObject controllerDiagram;
    [SerializeField] AudioClip gameStartTheme;
    private UIManager manager;
    private bool controllerConnected = false;


    // Start is called before the first frame update
    void Start()
    {
        //Pause game when onboarding panel is activated
        Time.timeScale = 0.0f;

        foreach (PlayerConfig pc in PlayerData.activePlayers)
        {
            if (pc.controlScheme != "KnM")
            {
                controllerConnected = true;
            }
        }

        if (controllerConnected)
        {
            controllerDiagram.SetActive(true);
        }

        else
        {
            mouseDiagram.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
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
