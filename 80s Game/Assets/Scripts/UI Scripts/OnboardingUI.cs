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
    [SerializeField] MusicTrack gameStartTheme;
    [SerializeField] MusicTrack gameLoopIntro;
    [SerializeField] MusicTrack gameLoopBGM;
    private UIManager manager;
    private bool controllerConnected = false;
    private bool playedBGM = false;

    // Start is called before the first frame update
    void Start()
    {
        // gameLoopBGM is a large audio file that isn't being preloaded
        // for some reason, so loading the data here prevents a lag spike
        // during early gameplay when it is first played
        gameLoopBGM.Clip.LoadAudioData();

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
        //SoundManager.Instance.PlayNonloopMusic(gameStartTheme);
        onboardingPanel.SetActive(false);
        gameUIElements.SetActive(true);

        PauseScreenBehavior.Instance.ToggleCrosshairs(true);

        Time.timeScale = 1.0f;
        manager.activeUI = UIManager.UIType.None;

        // Start coroutine for playing BGM
        StartCoroutine(DelayPersistentBGMIntro());
    }

    public void SetManager(UIManager reference)
    {
        manager = reference;
    }

    /// <summary>
    /// Coroutine used for delaying play of BGM intro
    /// </summary>
    IEnumerator DelayPersistentBGMIntro()
    {
        //yield return new WaitForSeconds(gameStartTheme.Clip.length - (float)(gameStartTheme.EndOffset + gameLoopIntro.StartOffset));
        yield return new WaitForSeconds(0);
        SoundManager.Instance.PlayNonloopMusic(gameLoopIntro);
        if(!playedBGM)
        {
            //StartCoroutine(DelayPersistentBGM());
            SoundManager.Instance.SetMusicToLoop(gameLoopBGM, gameLoopIntro.Clip.length - (gameLoopIntro.StartOffset + gameLoopIntro.EndOffset));
            playedBGM = true;
        }
    }

    /// <summary>
    /// Coroutine used for delaying play of persistent BGM.
    /// QP: I dummied this out as SoundManager has inbuilt functionality for scheduling the loop playback.
    /// </summary>
    IEnumerator DelayPersistentBGM()
    {
        yield return new WaitForSeconds(gameLoopIntro.Clip.length - (float)(gameLoopIntro.StartOffset + gameLoopIntro.EndOffset));
        SoundManager.Instance.SetMusicToLoop(gameLoopBGM, gameLoopIntro.EndOffset);
    }
}
