using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.EventSystems;
using Unity.VisualScripting;

public class GameOverBehavior : MonoBehaviour
{
    //Fields for UI
    public GameObject gameOverUI;
    public GameObject gameUI;
    public GameObject summaryScreen;
    public GameObject highScoreLeaderboard;
    public GameObject continueButton;
    public GameObject restartButton;
    public TMP_Text leaderboardText;
    public AudioClip buttonClickSound;

    private bool gameOverTransition = true;
    
    // Update is called once per frame
    void Update()
    {
        //set UI based on if the game is over or not
        gameOverUI.SetActive(GameManager.Instance.ActiveGameMode.GameOver);
        if (gameOverUI.activeInHierarchy)
        {
            if (gameOverTransition)
            {
                EventSystem.current.SetSelectedGameObject(null);
                EventSystem.current.SetSelectedGameObject(continueButton);

                this.gameObject.GetComponent<PauseScreenBehavior>().ToggleCrosshairs(false);
                Cursor.visible = true;

                gameOverTransition = false;
            }
            Time.timeScale = 0f;
            gameUI.SetActive(false);
        }
    }

    //Change game over panel when the player continues from the summary screen
    public void ContinueToLeaderboard()
    {
        // Load the high scores and set leaderboard text
        PointsManager pMngr = GameManager.Instance.PointsManager;
        List<PointsManager.UserRecord> records = pMngr.LoadRecords();
        string scores = "";
        for(int i = records.Count - 1; i >= 0; i--)
        {
            // Formatting for high score text
            scores += $"{records[i].initials}\t{records[i].score}\n";
            scores += "\n";
        }
        leaderboardText.text = scores;

        summaryScreen.SetActive(false);
        highScoreLeaderboard.SetActive(true);

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(restartButton);

        SoundManager.Instance.PlaySoundInterrupt(buttonClickSound);
    }

    //Restart the game by reloading the scene
    public void RestartGame()
    {
        SoundManager.Instance.PlaySoundContinuous(buttonClickSound);
        GameManager.Instance.PointsManager.SaveScore();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
