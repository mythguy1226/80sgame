using System;
using System.Collections;
using UnityEngine;

public class CampaignGameManager : MonoBehaviour
{
    public static Action roundOverObservers;

    AbsCampaignMode activeGameMode;

    [SerializeField]
    SpriteRenderer background;

    [SerializeField]
    CanvasGroup fadeScreen;

    [SerializeField]
    LevelDataSO debugFallback;

    [SerializeField]
    MusicTrack roundEndTheme;


    UIManager uiManager;
    LevelDataSO dataSpec;

    private void OnEnable()
    {
        dataSpec = CampaignSave.ActiveLevel;
        if (dataSpec == null)
        {
            //Same Scene testing fallback
            dataSpec = debugFallback;
        }
        uiManager = GetComponent<UIManager>();
        SetupGameMode();
        StartCoroutine(FadeIn());   
    }

    public void StartLevel()
    {
        if (dataSpec.LevelHasTutorial)
        {
            // Show Tutorial Screen
            return;
        }
        StartTargets();
    }

    public void StartTargets()
    {
        activeGameMode.StartGame();
    }


    private void SetupGameMode()
    {
        TargetManager targetManagerReference = GetComponent<TargetManager>();
        activeGameMode = GameModeFactory.MakeCampaignGameMode(dataSpec.GetLevelType());
        activeGameMode.SetupMode(dataSpec, targetManagerReference);
        activeGameMode.onGameEnd += HandleGameEnd;
        activeGameMode.onRoundEnd += HandleRoundEnd;
        background.sprite = dataSpec.GetLevelBackground();
    }

    private void EvaluateStart()
    {
        if (dataSpec.LevelHasTutorial)
        {
            // Show the tutorial
        } else
        {
            activeGameMode.StartGame();
        }
    }

    private void HandleGameEnd()
    {

    }

    private void HandleRoundEnd()
    {
        SoundManager.Instance.PlaySoundContinuous(roundEndTheme.Clip);
        uiManager.scoreBehavior.ShowNewRoundText();

        float delay = roundEndTheme.Clip.length - (float)roundEndTheme.EndOffset;
        StartCoroutine(activeGameMode.DelayNextRound(delay));
    }


    IEnumerator FadeIn()
    {
        float duration = 0.5f;
        float current = 0f;

        while(current < duration)
        {
            fadeScreen.alpha = 1 - (current / duration);
            current += Time.deltaTime;
            yield return null;
        }
        fadeScreen.alpha = 0;
        fadeScreen.gameObject.SetActive(false);
        EvaluateStart();
    }

    IEnumerator FadeOut()
    {
        fadeScreen.gameObject.SetActive(true);
        float duration = 0.5f;
        float current = 0f;

        while (current < duration)
        {
            fadeScreen.alpha = (current / duration);
            current += Time.deltaTime;
            yield return null;
        }
        fadeScreen.alpha = 1;
    }
}