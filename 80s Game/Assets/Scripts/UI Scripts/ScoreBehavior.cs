using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreBehavior : MonoBehaviour
{
    //References to Text Objects
    public TMP_Text scoreTextObject;
    public TMP_Text highScoreTextObject;
    public TMP_Text finalScoreTextObject;

    //Strings for text minus the score values
    private string scoreText = "Score:\n";
    private string highScoreText = "High Score:\n";
    private string finalScoreText = "Final Score: ";

    //Placeholder high score (Replace with File IO stuff)
    private int highScore = 20000;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Get the score from the PointsManager
        int score = GameManager.Instance.PointsManager.Points;

        //Update Score and Final Score text boxes
        scoreTextObject.SetText(scoreText + score);
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
}
