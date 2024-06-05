using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.EventSystems;

public class LoadingScreen : MonoBehaviour
{
    public GameObject loadingScreen;
    public GameObject controllerDiagram;
    public GameObject mouseDiagram;
    public Image loadingBarFill;

    public GameObject startButton;
    public GameObject startButtonPrompt;
    public Sprite controllerConfirmIcon;
    public Sprite mouseConfirmIcon;

    private bool confirmSceneLoad = false;

    public void LoadScene(int sceneId, bool controller)
    {
        StartCoroutine(LoadSceneAsync(sceneId, controller));
    }

    IEnumerator LoadSceneAsync(int sceneId, bool controller)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneId);
        operation.allowSceneActivation = confirmSceneLoad;

        loadingScreen.SetActive(true);

        if (!controller)
        {
            mouseDiagram.SetActive(true);
            startButtonPrompt.GetComponent<Image>().sprite = mouseConfirmIcon;
        }

        else
        {
            controllerDiagram.SetActive(true);
            startButtonPrompt.GetComponent<Image>().sprite = controllerConfirmIcon;
        }

        while (!operation.isDone)
        {
            float progressValue = Mathf.Clamp01(operation.progress / 0.9f);

            loadingBarFill.fillAmount = progressValue;

            if (operation.progress >= 0.9f)
            {
                startButton.GetComponent<TextMeshProUGUI>().text = "Start";
                startButtonPrompt.SetActive(true);

                EventSystem.current.SetSelectedGameObject(null);
                EventSystem.current.SetSelectedGameObject(startButton);

                startButton.GetComponent<Button>().onClick.AddListener(() => operation.allowSceneActivation = true);
            }

            yield return null;
        }
    }

    public void ConfirmStart()
    {
        confirmSceneLoad = true;
    }
}
