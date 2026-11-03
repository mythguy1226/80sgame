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

    LevelDataSO dataSpec;

    private void OnEnable()
    {
        dataSpec = CampaignSave.ActiveLevel;
        if (dataSpec == null)
        {
            //Same Scene testing fallback
            dataSpec = debugFallback;
        }

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
        activeGameMode = GameModeFactory.MakeCampaignGameMode(dataSpec.GetLevelType());
        activeGameMode.SetupMode(dataSpec);
        activeGameMode.onGameEnd += HandleGameEnd;
        background.sprite = dataSpec.GetLevelBackground();

        // Test to see if onboarding needs to appear

        //
    }

    private void HandleGameEnd()
    {

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