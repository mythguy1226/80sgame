using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class PointsManager : MonoBehaviour
{
    // Public property
    public int Points { get; private set; }
    public int RoundPoints { get; private set; }
    private string filePath = Application.dataPath + "/PlayerData/scores.txt";
    private int numTopScores = 6;

    // Start is called before the first frame update
    void Awake()
    {
        Points = 0;
        RoundPoints = 0;
    }

    /// <summary>
    /// Adds a number of points
    /// </summary>
    /// <param name="numPoints"></param>
    /// <returns>The new point total</returns>
    public int AddRoundPoints(int numPoints)
    {
        RoundPoints += numPoints;
        return RoundPoints;
    }

    /// <summary>
    /// Adds bonus points based off of an accuracy level
    /// </summary>
    /// <param name="accuracy"></param>
    /// <returns>The new round point total</returns>
    public int AddBonusPoints(float accuracy)
    {
        int numBonusPoints = Mathf.RoundToInt(RoundPoints * accuracy);
        return AddRoundPoints(numBonusPoints);
    }

    /// <summary>
    /// Totals up all the points in a round to the game points field
    /// </summary>
    /// <returns>The new point total</returns>
    public int AddTotal()
    {
        Points += RoundPoints;
        RoundPoints = 0;
        return Points;
    }

    /// <summary>
    /// Loads the scores from "scores.txt"
    /// </summary>
    /// <returns>A list of integers parsed from the data in "scores.txt"</returns>
    public List<int> LoadScores()
    {
        if(File.Exists(filePath))
        {
            // Read all lines from the file
            string[] lines = File.ReadAllLines(filePath);

            List<int> scores = new List<int>();
            // parse each top line and add it to the top scores
            foreach(string line in lines)
            {
                if (int.TryParse(line, out int score))
                {
                    scores.Add(score);
                }
                else
                {
                    Debug.LogWarning("Failed to parse score: " + line);
                }
            }

            // convert list to array
            return scores;
        }
        else
        {
            Debug.LogWarning("Scores file not found!");
            return new List<int> { };
        }
    }

    /// <summary>
    /// Saves the current overal score to "scores.txt"
    /// </summary>
    public void SaveScore()
    {
        string dirPath = Path.GetDirectoryName(filePath);
        // Create the file if it doesn't exist
        if(!Directory.Exists(dirPath))
        {
            Directory.CreateDirectory(dirPath);
        }

        // Write to the file
        using(StreamWriter writer = new StreamWriter(filePath, true))
        {
            writer.WriteLine(Points);
        }

        // Print the new set of scores
        LoadScores().ForEach(score => Debug.Log(score));
    }
}
