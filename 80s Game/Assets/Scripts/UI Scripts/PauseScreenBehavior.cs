using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseScreenBehavior : MonoBehaviour
{
    public static PauseScreenBehavior Instance { get; private set; }

    public bool isPaused;
    public GameObject pauseScreen;
    public GameObject continueButton;
    public GameObject gameUIElements;
    public GameObject introCutscene;
    public AudioClip buttonClickSound;

    public Crosshair[] crosshairs;
    public Image howToPlayPrompt;
    public List<Sprite> howToPlayIcons;

    public int playerIndex;

    public void Awake()
    {
        UIManager.pauseEvent += PauseGame;
    }

    private void OnDisable()
    {
        UIManager.pauseEvent -= PauseGame;
    }

    void Start()
    {
        // Check if the static reference matches the script instance
        if(Instance != null && Instance != this)
        {
            // If not, then the script is a duplicate and can delete itself
            Destroy(this);
        }

        else
        {
            Instance = this;
        }

        GetAllCrosshairs();
    }

    // Update is called once per frame
    void Update()
    {
   
    }

    public void PauseGame(int player)
    {
        if (GameManager.Instance.UIManager.activeUI != UIManager.UIType.None){
            return;
        }
        

        //Show Gamemode descriptions if on the Title Screen
        if (this.gameObject.GetComponent<TitleScreenBehavior>() != null)
        {            
            if (introCutscene != null)
            {
                introCutscene.GetComponent<CutsceneManager>().SkipCutscene();
            }

            return;
        }

        if(player >= 0)
        {
            playerIndex = player;
        }

        if (pauseScreen != null)
        {
            //GameManager.Instance.InputManager.ResetRumble();
            isPaused = !isPaused;

            ToggleCrosshairs(!isPaused);

            if (isPaused == true)
            {
                //Sets time scale to 0 so game pauses
                Time.timeScale = 0f;

                //Enable pause screen and onboarding info (except the button to close onboarding)
                pauseScreen.SetActive(true);
                gameUIElements.SetActive(false);

                EventSystem.current.SetSelectedGameObject(null);
                EventSystem.current.SetSelectedGameObject(continueButton);

                SoundManager.Instance.PlaySoundContinuous(buttonClickSound);

                switch (PlayerData.activePlayers[playerIndex].controlScheme)
                {
                    case "PS4":
                        howToPlayPrompt.sprite = howToPlayIcons[0];
                        break;
                    case "xbox":
                        howToPlayPrompt.sprite = howToPlayIcons[1];
                        break;
                    case "KnM":
                        howToPlayPrompt.sprite = howToPlayIcons[2];
                        break;
                }
            }

            else
            {
                //Sets time scale to 1 so game unpauses
                Time.timeScale = 1f;

                pauseScreen.SetActive(false);
                //onboardingPanel.SetActive(false);
                gameUIElements.SetActive(true);
                this.gameObject.GetComponent<SettingsManager>().settingsPanel.SetActive(false);

                SoundManager.Instance.PlaySoundContinuous(buttonClickSound);
                LookingGlassUI lookingGlassUI = FindObjectOfType<LookingGlassUI>();

                if (lookingGlassUI != null)
                {
                    lookingGlassUI.Hide();
                }
            }
        }
    }

    public void HowToPlay()
    {
        if (this.gameObject.GetComponent<TitleScreenBehavior>() != null)
        {
            if (this.gameObject.GetComponent<TitleScreenBehavior>().gamemodePanel.activeInHierarchy)
            {
                foreach (Button gamemode in this.gameObject.GetComponent<TitleScreenBehavior>().gamemodeOptions)
                {
                    GameObject gamemodeDescription = gamemode.gameObject.transform.GetChild(2).gameObject;
                    gamemodeDescription.SetActive(!gamemodeDescription.activeInHierarchy);
                }
                return;
            }
        }

        if (GameManager.Instance.UIManager.onboardingUI.backOutPanel.activeInHierarchy)
            return;

        GameManager.Instance.UIManager.onboardingUI.onboardingPanel.SetActive(!GameManager.Instance.UIManager.onboardingUI.onboardingPanel.activeInHierarchy);
        pauseScreen.SetActive(!pauseScreen.activeInHierarchy);

        if (pauseScreen.activeInHierarchy)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject (continueButton);
        }
    }

    public void QuitGame()
    {
        Time.timeScale = 1f;

        if (GameManager.Instance.ActiveGameMode.ModeType == EGameMode.Defense)
        {
            AbsGameMode gameMode = GameManager.Instance.ActiveGameMode;
            AchievementManager.TestEndGameAchievements(gameMode.ModeType, gameMode.CurrentRound, 0);
        }
        GameManager.Instance.SteamInterface.UpdateSteamServer();
        SceneManager.LoadScene(2);
        SoundManager.Instance.StopAllAudio();
        SoundManager.Instance.PlaySoundContinuous(buttonClickSound);
    }

    //Toggle crosshairs and mouse cursor based on the passed in boolean
    public void ToggleCrosshairs(bool toggle)
    {
        GetAllCrosshairs();
        foreach(Crosshair c in crosshairs)
        {
            c.gameObject.SetActive(toggle);
        }

        if (toggle == false)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    //Fill the array of crosshairs with all crosshair objects
    public void GetAllCrosshairs()
    {
        crosshairs = FindObjectsOfType(typeof(Crosshair), includeInactive:true) as Crosshair[];
    }

    public void ContinueGame()
    {
        PauseGame(0);
    }
}
