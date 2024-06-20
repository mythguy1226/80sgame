using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


//Class that handles behavior for UI elements on the game's title screen
public class TitleScreenBehavior : MonoBehaviour
{
    public GameObject onboardingContinueButton;
    public GameObject onboardingPanel;
    public GameObject gamemodePanel;
    public AudioClip buttonClickSound;
    public AudioClip titleScreenMusic;

    //UI Elements for selecting game modes
    public TextMeshProUGUI gamemodeName;
    public TextMeshProUGUI gamemodeDescription;
    public GameObject startButton;
    public List<Button> gamemodeOptions;

    private int gamemodeSelected = 1;

    void Start()
    {
        SoundManager.Instance.SetMusicToLoop(titleScreenMusic);

        //Set up Gamemode Selection Buttons
        gamemodeOptions[0].onClick.AddListener(() => SelectGamemode(1));
        gamemodeOptions[1].onClick.AddListener(() => SelectGamemode(2));
        gamemodeOptions[2].onClick.AddListener(() => SelectGamemode(3));
    }

    void Update()
    {
        //Change gamemode based on which game mode is selected
        switch(gamemodeSelected)
        {
            case 1:
                GameModeData.activeGameMode = EGameMode.Classic;
                break;
            case 2:
                GameModeData.activeGameMode = EGameMode.Competitive;
                break;
            case 3:
                GameModeData.activeGameMode = EGameMode.Defense;
                break;
        }
    }

    //Loads the scene with index 1 within the game's build settings (Should be the Join Scene)
    //Used within the StartGame button element
    public void StartGame()
    {
        SoundManager.Instance.PlaySoundContinuous(buttonClickSound);
        SoundManager.Instance.StopMusicLoop();
        PlayerData.Reset();
        SceneManager.LoadScene(1);
    }

    //Exits the application
    //Used within the ExitGame button element
    public void ExitGame()
    {
        SoundManager.Instance.PlaySoundContinuous(buttonClickSound);
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }

    //Closes the onboarding panel
    public void CloseOnboarding()
    {
        onboardingPanel.SetActive(false);
    }

    //Toggle the game mode selection panel on or off
    public void ToggleGamemodeSelection()
    {
        gamemodePanel.SetActive(!gamemodePanel.activeInHierarchy);

        //Select the proper UI element for navigation
        if (gamemodePanel.activeInHierarchy)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(gamemodeOptions[0].gameObject);
        }
        else
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(startButton);
        }
    }

    //Select Gamemode and start the game
    public void SelectGamemode(int gamemodeIndex)
    {
        gamemodeSelected = gamemodeIndex;
        StartGame();
    }
}
