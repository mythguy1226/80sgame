using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;


//Class that handles behavior for UI elements on the game's title screen
public class TitleScreenBehavior : MonoBehaviour
{
    public GameObject onboardingContinueButton;
    public GameObject onboardingPanel;
    public GameObject gamemodePanel;
    public AudioClip buttonClickSound;
    public AudioClip titleScreenMusic;

    public TextMeshProUGUI gamemodeName;
    public TextMeshProUGUI gamemodeDescription;
    public GameObject startButton;
    public GameObject nextGamemodeButton;

    private int gamemodeSelected = 1;

    void Start()
    {
        SoundManager.Instance.SetMusicToLoop(titleScreenMusic);
    }

    void Update()
    {


        switch(gamemodeSelected)
        {
            case 1:
                gamemodeName.text = "Classic";
                gamemodeDescription.text = "The Classic Bat Bots experience.\n\nPlay through several rounds and stun as many Bat Bots as possible.\n\nTry to achieve the highest score!";
                GameModeData.activeGameMode = EGameMode.Classic;
                break;
            case 2:
                gamemodeName.text = "Competitive";
                gamemodeDescription.text = "Bat Bots with Multiplayer!\n\nPlay with up to 2 players and compete to see who can get the highest score!\n\nThis mode features new bat bots not seen in the Classic mode!";
                GameModeData.activeGameMode = EGameMode.Competitive;
                break;
        }
    }

    //Loads the scene with index 1 within the game's build settings
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
        if (Application.isEditor)
        {
            UnityEditor.EditorApplication.isPlaying = false;
            return;
        }
        Application.Quit();
    }

    //Closes the onboarding panel
    public void CloseOnboarding()
    {
        onboardingPanel.SetActive(false);
    }

    public void ToggleGamemodeSelection()
    {
        gamemodePanel.SetActive(!gamemodePanel.activeInHierarchy);

        if (gamemodePanel.activeInHierarchy)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(nextGamemodeButton);
        }
        else
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(startButton);
        }
    }

    public void NextGameMode()
    {
        gamemodeSelected++;
        if (gamemodeSelected == 3) gamemodeSelected = 1;

        gamemodeSelected = Mathf.Clamp(gamemodeSelected, 1, 2);
    }

    public void PreviousGameMode()
    {
        gamemodeSelected--;
        if (gamemodeSelected == 0) gamemodeSelected = 2;
        
        gamemodeSelected = Mathf.Clamp(gamemodeSelected, 1, 2);
    }
}
