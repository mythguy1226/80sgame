using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameOverBehavior : MonoBehaviour
{
    //Fields for UI
    public GameObject gameOverUI;
    public GameObject gameUI;
    public GameObject summaryScreen;
    public GameObject highScoreLeaderboard;
    public TMP_Text leaderboardText;

    // Update is called once per frame
    void Update()
    {
        //set UI based on if the game is over or not
        gameOverUI.SetActive(GameManager.Instance.TargetManager.gameOver);
        if (gameOverUI.activeInHierarchy)
        {
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
    }

    //Restart the game by reloading the scene
    public void RestartGame()
    {
        GameManager.Instance.PointsManager.SaveScore();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
