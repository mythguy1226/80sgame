using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.EventSystems;
using Unity.VisualScripting;
using UnityEngine.InputSystem;

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
    public List<GameObject> leaderboardScoreHighlights;
    [SerializeField] MusicTrack gameEndTheme;

    private bool gameOverTransition = true;
    
    // Update is called once per frame
    // This should be refactored - the game over screen is being set every frame
    void Update()
    {
        //set UI based on if the game is over or not
        gameOverUI.SetActive(GameManager.Instance.ActiveGameMode.GameOver);
        if (gameOverUI.activeInHierarchy)
        {
            GameManager.Instance.UIManager.activeUI = UIManager.UIType.GameOver;

            //Handle transition to game over screen
            if (gameOverTransition)
            {
                //Select continue button for non-mouse navigation
                EventSystem.current.SetSelectedGameObject(null);
                EventSystem.current.SetSelectedGameObject(continueButton);
                GameManager.Instance.PointsManager.SaveScore();
                
                //Disable crosshairs and turn mouse cursor on
                this.gameObject.GetComponent<PauseScreenBehavior>().ToggleCrosshairs(false);
                Cursor.visible = true;
                if (gameEndTheme != null)
                    SoundManager.Instance.PlayNonloopMusic(gameEndTheme);

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
        bool topSixScore = false;

        for(int i = records.Count - 1; i >= 0; i--)
        {
            // Formatting for high score text
            scores += $"{records[i].initials}\t{records[i].score}\n";
            scores += "\n";

            //Highlight score if it's in the top six
            if (records[i].score == pMngr.maxScore)
            {
                leaderboardScoreHighlights[records.Count - 1 - i].SetActive(true);
                leaderboardScoreHighlights[records.Count - 1 - i].GetComponent<TextMeshProUGUI>().text = $"{records[i].initials}\t{records[i].score}";
                topSixScore = true;
            }
        }

        //If the score is not in the top six, show it at the bottom of the leaderboard
        if (!topSixScore)
        {
            this.gameObject.GetComponent<ScoreBehavior>().bottomScoreTextObject.gameObject.SetActive(true);
        }
        leaderboardText.text = scores;

        //Transition from summary screen to high score leaderboard
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
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
