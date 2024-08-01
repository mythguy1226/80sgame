using System;
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
    public GameObject achievementsButton;
    public GameObject achievementsPanel;
    public GameObject achievementsList;
    public GameObject achievementRewardPrefab;
    public AudioClip buttonClickSound;
    public MusicTrack titleScreenMusic;

    //UI Elements for selecting game modes
    public TextMeshProUGUI gamemodeName;
    public TextMeshProUGUI gamemodeDescription;
    public GameObject startButton;
    public List<Button> gamemodeOptions;
    public Image helpInputPrompt;
    public List<Sprite> controllerHelpInputs;

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
        SoundManager.Instance.StopAllAudio();
        SoundManager.Instance.PlaySoundContinuous(buttonClickSound);
        PlayerData.Reset();
        SceneManager.LoadScene(1);
    }

    //Exits the application
    //Used within the ExitGame button element
    public void ExitGame()
    {
        SoundManager.Instance.StopAllAudio();
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

            //Change the prompt icon for the Gamemode help button to the proper control scheme
            switch (PlayerData.activePlayers[0].controlScheme)
            {
                case "PS4":
                    helpInputPrompt.sprite = controllerHelpInputs[0];
                    break;
                case "xbox":
                    helpInputPrompt.sprite = controllerHelpInputs[1];
                    break;
            }
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

    //Toggles the Achievements Screen on and off
    public void ToggleAchievementsPanel()
    {
        achievementsPanel.SetActive(!achievementsPanel.activeInHierarchy);

        if (achievementsPanel.activeInHierarchy)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(achievementsList.transform.GetChild(0).gameObject);
        }

        else
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(achievementsButton);
        }
    }
    
    //Creates a screen showing achievement rewards
    public void CreateAchievementRewards()
    {
        //Get Next AchievementData from the rewards list
        AchievementData unlockedAchievement = AchievementManager.GetNextReward();

        if (unlockedAchievement != null)
        {
            //Set reward type and reward thumbnails
            achievementRewardPrefab.GetComponent<AchievementReward>().rewardType = unlockedAchievement.rewardText;
            achievementRewardPrefab.GetComponent<AchievementReward>().rewardThumbnails = unlockedAchievement.rewardSprites;

            Instantiate(achievementRewardPrefab, GameManager.Instance.UIManager.canvas.transform);
        }   

        //If there are no rewards left, select the start button
        else
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(startButton);
        }
    }
}
