using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseScreenBehavior : MonoBehaviour
{
    public static PauseScreenBehavior Instance { get; private set; }

    public bool isPaused;
    public GameObject pauseScreen;
    public GameObject continueButton;
    public GameObject gameUIElements;
    public GameObject onboardingPanel;
    public GameObject onboardingCloseButton;
    public AudioClip buttonClickSound;

    public Crosshair[] crosshairs;

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
        if (onboardingPanel == null)
        {
            return;
        }
    }

    public void PauseGame(int player)
    {
        if (GameManager.Instance.UIManager.activeUI != UIManager.UIType.None){
            return;
        }
        

        //Show Gamemode descriptions if on the Title Screen
        if (this.gameObject.GetComponent<TitleScreenBehavior>() != null && this.gameObject.GetComponent<TitleScreenBehavior>().gamemodePanel.activeInHierarchy)
        {
            foreach (Button gamemode in this.gameObject.GetComponent<TitleScreenBehavior>().gamemodeOptions)
            {
               GameObject gamemodeDescription = gamemode.gameObject.transform.GetChild(2).gameObject;
               gamemodeDescription.SetActive(!gamemodeDescription.activeInHierarchy);
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
            }

            else
            {
                //Sets time scale to 1 so game unpauses
                Time.timeScale = 1f;

                pauseScreen.SetActive(false);
                //onboardingPanel.SetActive(false);
                gameUIElements.SetActive(true);

                foreach(Crosshair c in crosshairs)
                {
                    c.gameObject.SetActive(true);
                }

                SoundManager.Instance.PlaySoundContinuous(buttonClickSound);
            }
        }
    }

    public void QuitGame()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene(0);

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
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Confined;
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
