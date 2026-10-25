using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameFileCard : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI fileName;
    [SerializeField]
    Image background;
    [SerializeField]
    TextMeshProUGUI progress;

    [SerializeField]
    GameObject mainText;

    [SerializeField]
    GameObject typer;

    [SerializeField]
    GameObject typerButton;

    [SerializeField]
    List<Button> upButtons;

    [SerializeField]
    List<Button> downButtons;

    [SerializeField]
    List<TextMeshProUGUI> initials;

    [SerializeField]
    GameObject fadeInScreen;

    int index;

    //Variables to keep track of which letter to display for each initial
    private int initialOne = 65;
    private int initialTwo = 65;
    private int initialThree = 65;


    public void SetUp(int index, GameFileData data)
    {
        this.index = index;
        if (data.playerAvatarLocation == -1)
        {
            SetUpEmpty();
            return;
        }

        fileName.text = data.owner;
        progress.text = CalculateProgress(data.levelClearInformation).ToString()+"%";
        background.sprite = GameManager.Instance.UIManager.levelSprites[data.playerAvatarLocation];
    }

    private void ShowTyper()
    {
        typer.SetActive(true);
        mainText.SetActive(false);

        for (int i = 0; i < upButtons.Count; i++) {
            int iCopy = i;
            upButtons[i].onClick.AddListener(() => ChangeInitial(false, iCopy));
            downButtons[i].onClick.AddListener(()=> ChangeInitial(true, iCopy));
        }


        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(typerButton);
    }

    public void ChooseFile()
    {
        if (fileName.text == "NEW")
        {
            ShowTyper();
            return;
        }

        // Load the campaign
        LoadCampaign();
    }

    public void LoadCampaign()
    {
        StartCoroutine(FadeOutSubroutine());
    }

    private int UpdateInitialNumber(bool increment, int currentValue)
    {
        if (increment)
        {
            if (currentValue == 90)
            {
                return 65;
            }
            return Mathf.Clamp(++currentValue, 65, 90);
        } else
        {
            if (currentValue == 65)
            {
                return 90;
            }
            return Mathf.Clamp(--currentValue, 65, 90);
        }
    }

    private void ChangeInitial(bool increment, int initialIndex)
    {
        char tempChar;
        switch (initialIndex) {
            case 0:
                initialOne = UpdateInitialNumber(increment, initialOne);
                tempChar = (char)initialOne;
                initials[initialIndex].text = tempChar.ToString();
                break;
            case 1:
                initialTwo = UpdateInitialNumber(increment, initialTwo);
                tempChar = (char)initialTwo;
                initials[initialIndex].text = tempChar.ToString();
                break;
            case 2:
                initialThree = UpdateInitialNumber(increment, initialThree);
                tempChar = (char)initialThree;
                initials[initialIndex].text = tempChar.ToString();
                break;
        }
    }

    private void SetUpEmpty()
    {
        fileName.text = "NEW";
        background.color = new Color(0, 0, 0, 0);
        progress.text = "";
    }

    private float CalculateProgress(LevelClearInformation[] clearInformation)
    {
        int count = clearInformation.Length;
        int incomplete = 0;
        for (int i = 0; i < count; i++)
        {
            if (!clearInformation[i].cleared)
            {
                incomplete++;
            }
        }
        return Mathf.Floor((incomplete / (float)count) * 100);
    }

    IEnumerator FadeOutSubroutine()
    {
        
        fadeInScreen.SetActive(true);
        Animator a = fadeInScreen.GetComponent<Animator>();
        Destroy(a);
        CanvasGroup im = fadeInScreen.GetComponent<CanvasGroup>();
        im.alpha = 0f;
        float duration = 0.5f;
        float current = 0.0f;
        while(current < duration)
        {
            im.alpha = (current / duration);
            current += Time.deltaTime;
            yield return null;
        }
        //im.color = new Color(0, 0, 0, 255f);
        im.alpha = 1;
        // To do: Set up information to load


        SceneManager.LoadScene(6);
    }
}