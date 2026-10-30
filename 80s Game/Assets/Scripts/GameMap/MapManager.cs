using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapManager : MonoBehaviour
{
    bool showingUI;
    Dictionary<(int,int), MapNode> nodes;
    public static MapManager Instance { get; private set; }
    MapNode activeNode;

    [SerializeField]
    GameObject fadeInScreen;


    private void Awake()
    {
        nodes = new Dictionary<(int, int), MapNode>();
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        } else
        {
            Instance = this;
            
        }
    }

    private void OnEnable()
    {
        PlayerAvatar.submitReceived += HandleSubmit;
        StartCoroutine(RunFadeIn());
    }

    private void OnDisable()
    {
        PlayerAvatar.submitReceived -= HandleSubmit;
    }

    private void OnDestroy()
    {
        PlayerAvatar.submitReceived -= HandleSubmit;
    }

    public void RegisterNode(int x, int y, MapNode node)
    {
        nodes.Add((x,y), node);
    }

    private void HandleSubmit()
    {
        if (showingUI)
        {
            CampaignSave.ActiveLevel = activeNode.GetLevelData();
            StartCoroutine(RunFadeOut());
        }
    }


    IEnumerator RunFadeIn()
    {
        CanvasGroup im = fadeInScreen.GetComponent<CanvasGroup>();
        im.alpha = 0f;

        // Set up the scene before fading in

        float duration = 0.5f;
        float current = 0.0f;
        while (current < duration)
        {
            im.alpha = 1-(current / duration);
            current += Time.deltaTime;
            yield return null;
        }
        im.alpha = 0;
        fadeInScreen.SetActive(false);
    }

    IEnumerator RunFadeOut()
    {
        fadeInScreen.SetActive(true);
        CanvasGroup im = fadeInScreen.GetComponent<CanvasGroup>();
        im.alpha = 0f;
        float duration = 0.5f;
        float current = 0.0f;
        while (current < duration)
        {
            im.alpha = (current / duration);
            current += Time.deltaTime;
            yield return null;
        }
        //im.color = new Color(0, 0, 0, 255f);
        im.alpha = 1;

        SceneManager.LoadScene(7);
    }


}