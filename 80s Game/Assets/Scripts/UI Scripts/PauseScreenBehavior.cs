using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseScreenBehavior : MonoBehaviour
{
    public bool isPaused;
    public GameObject PauseScreen;
    public GameObject GameUIElements;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("escape"))
        {
            PauseGame();
        }
    }

    public void PauseGame()
    {
        isPaused = !isPaused;
        if (isPaused == true)
        {
            PauseScreen.SetActive(true);
            GameUIElements.SetActive(false);

            //Sets time scale to 0 so game pauses
            Time.timeScale = 0f;
        }

        else
        {
            PauseScreen.SetActive(false);
            GameUIElements.SetActive(true);

            //Sets time scale to 1 so game unpauses
            Time.timeScale = 1f;
        }
    }

    public void QuitGame()
    {
        SceneManager.LoadScene(0);
    }
}
