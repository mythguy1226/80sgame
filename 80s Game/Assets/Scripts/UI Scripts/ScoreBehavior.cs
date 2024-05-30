using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SocialPlatforms.Impl;

public class ScoreBehavior : MonoBehaviour
{
    //References to Text Objects
    public List<TextMeshProUGUI> textScores;
    public TMP_Text highScoreTextObject;
    public TMP_Text finalScoreTextObject;
    public TMP_Text roundIndicatorTextObject;

    //Strings for text minus the score values
    private string highScoreText = "High Score:\n";
    private string finalScoreText = "Final Score: ";
    private string roundText = "Round\n";

    //Placeholder high score (Replace with File IO stuff)
    private int highScore = 20000;

    private int playerOnePoints = 0;
    private int playerTwoPoints = 0;
    private int leadingPlayer;

    // Start is called before the first frame update
    void Start()
    {
        // Get the current high score
        PointsManager pMngr = GameManager.Instance.PointsManager;
        List<PointsManager.UserRecord> records = pMngr.LoadRecords();

        // Set high score text
        if (records.Count > 0)
            highScore = records[records.Count - 1].score;
        else
            highScore = 0;
    }

    // Update is called once per frame
    void Update()
    {
        //Get the score from the PointsManager
        int score = GameManager.Instance.PointsManager.maxScore;
        int currentRoundNum = GameManager.Instance.ActiveGameMode.CurrentRound;
        int maxNumOfRounds = GameManager.Instance.ActiveGameMode.NumRounds;

        if (GameManager.Instance.gameModeType == EGameMode.Classic)
        {
            finalScoreTextObject.SetText(finalScoreText + score);

            //If you beat the high score, list the current score under high score as well
            if (score > highScore)
            {
                highScoreTextObject.SetText(highScoreText + score);
            }

            //Otherwise use the set high score value
            else
            {
                highScoreTextObject.SetText(highScoreText + highScore);
            }
        }

        else if (GameManager.Instance.gameModeType == EGameMode.Competitive)
        {
            switch (leadingPlayer)
            {
                case 1:
                    finalScoreText = "Player 1 Wins!";
                    break;
                case 2:
                    finalScoreText = "Player 2 Wins!";
                    break;
                case 3:
                    finalScoreText = "Tie!";
                    break;
            }

            finalScoreText += "\n\n\nFINAL SCORES:\n\nPlayer 1: " + playerOnePoints + "\nPlayer 2: " + playerTwoPoints;
            finalScoreTextObject.SetText(finalScoreText);
        }

        //Update Round Indicator
        roundIndicatorTextObject.SetText(roundText + currentRoundNum + "/" + maxNumOfRounds);
    }

    public void UpdateScores(int player)
    {
        Debug.Log(player);
        textScores[player].text = GameManager.Instance.PointsManager.TotalPointsByPlayer[player].ToString();

        if(GameManager.Instance.PointsManager.TotalPointsByPlayer.ContainsKey(0))
        {
            playerOnePoints = GameManager.Instance.PointsManager.TotalPointsByPlayer[0];
        }

        if (GameManager.Instance.PointsManager.TotalPointsByPlayer.ContainsKey(1))
        {
            playerTwoPoints = GameManager.Instance.PointsManager.TotalPointsByPlayer[1];
        }   

        if (playerOnePoints > playerTwoPoints)
        {
            leadingPlayer = 1;
        }
        else if(playerOnePoints < playerTwoPoints)
        {
            leadingPlayer = 2;
        }
        else
        {
            leadingPlayer = 3;
        }

        Debug.Log(leadingPlayer);
    }
}
