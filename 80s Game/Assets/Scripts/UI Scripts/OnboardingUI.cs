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
    [SerializeField] AudioClip gameLoopIntro;
    [SerializeField] AudioClip gameLoopBGM;
    private UIManager manager;
    private bool controllerConnected = false;
    private bool playedBGM = false;


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

        // Start coroutine for playing BGM
        StartCoroutine(DelayPersistentBGMIntro(gameStartTheme.length));
    }

    public void SetManager(UIManager reference)
    {
        manager = reference;
    }

    /// <summary>
    /// Coroutine used for delaying play of BGM intro
    /// </summary>
    IEnumerator DelayPersistentBGMIntro(float delay)
    {
        yield return new WaitForSeconds(delay);
        SoundManager.Instance.PlaySoundContinuous(gameLoopIntro);
        if(!playedBGM)
        {
            StartCoroutine(DelayPersistentBGM(gameLoopIntro.length));
            playedBGM = true;
        }
    }

    /// <summary>
    /// Coroutine used for delaying play of persistent BGM
    /// </summary>
    IEnumerator DelayPersistentBGM(float delay)
    {
        yield return new WaitForSeconds(delay);
        SoundManager.Instance.SetMusicToLoop(gameLoopBGM);
    }
}
