using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseScreenBehavior : MonoBehaviour
{
    public bool isPaused;
    public GameObject pauseScreen;
    public GameObject gameUIElements;
    public GameObject onboardingPanel;
    public GameObject onboardingCloseButton;
    public AudioClip buttonClickSound;

    // Update is called once per frame
    void Update()
    {
        //Pause game if escape key is pressed
        if (Input.GetKeyDown("escape") && (!onboardingPanel.activeInHierarchy || pauseScreen.activeInHierarchy) && !GameManager.Instance.TargetManager.gameOver)
        {
            PauseGame();
        }

        //If the onboarding panel is active when escape is pressed, close it and start the game
        else if (Input.GetKeyDown("escape") && onboardingPanel.activeInHierarchy && !pauseScreen.activeInHierarchy)
        {
            onboardingPanel.SetActive(false);
            gameUIElements.SetActive(true);

            Time.timeScale = 1.0f;
        }
    }

    public void PauseGame()
    {
        //GameManager.Instance.InputManager.ResetRumble();
        isPaused = !isPaused;
        if (isPaused == true)
        {
            //Sets time scale to 0 so game pauses
            Time.timeScale = 0f;

            //Enable pause screen and onboarding info (except the button to close onboarding)
            pauseScreen.SetActive(true);
            onboardingPanel.SetActive(true);
            onboardingCloseButton.SetActive(false);
            gameUIElements.SetActive(false);

            SoundManager.Instance.PlaySoundContinuous(buttonClickSound);
        }

        else
        {
            //Sets time scale to 1 so game unpauses
            Time.timeScale = 1f;

            pauseScreen.SetActive(false);
            onboardingPanel.SetActive(false);
            gameUIElements.SetActive(true);

            SoundManager.Instance.PlaySoundContinuous(buttonClickSound);
        }
    }

    public void QuitGame()
    {
        GameManager.Instance.PointsManager.SaveScore();
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);

        SoundManager.Instance.PlaySoundContinuous(buttonClickSound);
    }
}
