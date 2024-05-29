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
        Debug.Log(titleScreenMusic + " should be playing right about now...");
        Debug.Log("Is music playing? " + SoundManager.Instance.IsMusicPlaying);
    }

    void Update()
    {
        //if (!SoundManager.Instance.IsMusicPlaying)
        //{
        //    SoundManager.Instance.SetMusicToLoop(titleScreenMusic);
        //    Debug.Log(titleScreenMusic + " has been executed through update instead!");
        //}
        //Disable joycon calibration onboarding if no joycons are connected
        /*if (GameManager.Instance.InputManager.joycons.Count == 0)
        {
            onboardingPanel.SetActive(false);
            onboardingContinueButton.SetActive(false);
        }

        //only allow user to continue after they recenter the cursor
        else
        {
            Joycon j = GameManager.Instance.InputManager.joycons[GameManager.Instance.InputManager.jc_ind];
            if (j.GetButtonDown(Joycon.Button.DPAD_DOWN))
            {
                onboardingContinueButton.SetActive(true);
            }
        }*/

        switch(gamemodeSelected)
        {
            case 1:
                gamemodeName.text = "Classic";
                gamemodeDescription.text = "The Classic Bat Bots experience.\n\nPlay through several rounds and stun as many Bat Bots as possible.\n\nTry to achieve the highest score!";
                break;
            case 2:
                gamemodeName.text = "Competitive";
                gamemodeDescription.text = "Bat Bots with Multiplayer!\n\nPlay with up to 2 players and compete to see who can get the highest score!\n\nThis mode features new bat bots not seen in the Classic mode!";
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
        SceneManager.LoadScene(gamemodeSelected);
    }

    //Exits the application
    //Used within the ExitGame button element
    public void ExitGame()
    {
        SoundManager.Instance.PlaySoundContinuous(buttonClickSound);
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
