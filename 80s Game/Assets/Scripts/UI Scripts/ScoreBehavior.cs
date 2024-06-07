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
    private float accuracy = 100;

    private int playerOnePoints = 0;
    private int playerTwoPoints = 0;
    private int leadingPlayer;

    public List<TextMeshProUGUI> playerNames;

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

        //Set the player names for the scores in Competitive mode
        if (GameManager.Instance.gameModeType == EGameMode.Competitive)
        {   
            //Loop through all active player names
            for (int i =0 ; i <= playerNames.Count - 1; i++)
            {
                //Set default name if no initials set
                if (PlayerData.activePlayers[i].initials == null)
                {
                    playerNames[i].text = "AAA";
                }

                //Otherwise, used saved initials
                else
                {
                    playerNames[i].text = PlayerData.activePlayers[i].initials;
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Get the score from the PointsManager
        int score = GameManager.Instance.PointsManager.maxScore;
        int currentRoundNum = GameManager.Instance.ActiveGameMode.CurrentRound;
        int maxNumOfRounds = GameManager.Instance.ActiveGameMode.NumRounds;

        //Classic mode final score 
        if (GameManager.Instance.gameModeType == EGameMode.Classic)
        {
            //Display the final score and accuracy values
            finalScoreText = "Final Score: " + score + "\n\nAccuracy: " + accuracy + "%";
            finalScoreTextObject.SetText(finalScoreText);

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

        //Competitive mode final scores
        else if (GameManager.Instance.gameModeType == EGameMode.Competitive)
        {
            //Show which player has won the game
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

            //Additionally show each player's final scores
            finalScoreText += "\n\n\nFINAL SCORES:\n\nPlayer 1: " + playerOnePoints + "\nPlayer 2: " + playerTwoPoints;
            finalScoreTextObject.SetText(finalScoreText);
        }

        //Update Round Indicator
        roundIndicatorTextObject.SetText(roundText + currentRoundNum + "/" + maxNumOfRounds);
    }

    //Update scores for the players in competitive mode
    public void UpdateScores(int player)
    {
        //Update the appropraite score text with the proper player's score
        if (textScores[player] != null)
        {
            textScores[player].text = GameManager.Instance.PointsManager.TotalPointsByPlayer[player].ToString();
        }
        
        //Keep track of player 1's score
        if(GameManager.Instance.PointsManager.TotalPointsByPlayer.ContainsKey(0))
        {
            playerOnePoints = GameManager.Instance.PointsManager.TotalPointsByPlayer[0];
        }

        //Keep track of player 2's score
        if (GameManager.Instance.PointsManager.TotalPointsByPlayer.ContainsKey(1))
        {
            playerTwoPoints = GameManager.Instance.PointsManager.TotalPointsByPlayer[1];
        }   

        //Update which player is winning based on the scores
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


    }
    
    //Keep track of the accuracy
    public void UpdateAccuracy(float newAccuracy)
    {
        accuracy = Mathf.RoundToInt(newAccuracy * 100);
    }
}
