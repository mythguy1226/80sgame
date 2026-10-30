using System.Collections;
using UnityEngine;

public class CampaignGameManager : MonoBehaviour
{
    AbsGameMode activeGameMode;

    [SerializeField]
    CanvasGroup fadeScreen;

    LevelDataSO dataSpec;


    private void OnEnable()
    {
        dataSpec = CampaignSave.ActiveLevel;
        if (dataSpec == null)
        {
            //Testing fallback
        }
        StartCoroutine(FadeIn());   
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