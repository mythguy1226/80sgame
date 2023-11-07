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

    // Update is called once per frame
    void Update()
    {
        //Pause game if escape key is pressed
        if (Input.GetKeyDown("escape") && !onboardingPanel.activeInHierarchy)
        {
            PauseGame();
        }

        //If the onboarding panel is active when escape is pressed, close it and start the game
        else if (Input.GetKeyDown("escape") && onboardingPanel.activeInHierarchy)
        {
            onboardingPanel.SetActive(false);
            gameUIElements.SetActive(true);

            Time.timeScale = 1.0f;
        }
    }

    public void PauseGame()
    {
        isPaused = !isPaused;
        if (isPaused == true)
        {
            pauseScreen.SetActive(true);
            gameUIElements.SetActive(false);

            //Sets time scale to 0 so game pauses
            Time.timeScale = 0f;
        }

        else
        {
            pauseScreen.SetActive(false);
            gameUIElements.SetActive(true);

            //Sets time scale to 1 so game unpauses
            Time.timeScale = 1f;
        }
    }

    public void QuitGame()
    {
        SceneManager.LoadScene(0);
        Time.timeScale = 1f;
    }
}
