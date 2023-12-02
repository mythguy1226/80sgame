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
    private string initials = "ABC";
    public HighScoreInitials initialsTracker;

    // Start is called before the first frame update
    void Awake()
    {
        Points = 0;
        RoundPoints = 0;
    }

    // Add directly to total points
    public int AddPoints(int numPoints)
    {
        Points += numPoints;
        return Points;
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
        Points += RoundPoints;
        RoundPoints = 0;
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

    public class UserRecord
    {
        public string initials;
        public int score;

        public UserRecord(string intitials, int score)
        {
            this.initials = intitials;
            this.score = score;
        }
    }

    /// <summary>
    /// Loads the scores from "scores.txt"
    /// </summary>
    /// <returns>A list of integers parsed from the data in "scores.txt"</returns>
    public List<UserRecord> LoadRecords()
    {
        if(File.Exists(filePath))
        {
            // Read all lines from the file
            string[] lines = File.ReadAllLines(filePath);

            List<UserRecord> records = new List<UserRecord>();
            // parse each top line and add it to the top scores
            foreach(string line in lines)
            {
                string[] data = line.Split(":");
                if (int.TryParse(data[1], out int score))
                {
                    records.Add(new UserRecord(data[0], score));
                }
                else
                {
                    Debug.LogWarning("Failed to parse score: " + line);
                }
            }

            return records;
        }
        else
        {
            Debug.LogWarning("Scores file not found!");
            return new List<UserRecord> { };
        }
    }

    /// <summary>
    /// Saves the current overal score to "scores.txt"
    /// </summary>
    public void SaveScore()
    {
        // Get initials from tracker
        initials = initialsTracker.initialOneText.text + initialsTracker.initialTwoText.text + initialsTracker.initialThreeText.text;

        // Load the saves file
        List<UserRecord> records = LoadRecords();

        // Sort the leaderboard based on score
        records.Sort((record1, record2) => record1.score.CompareTo(record2.score));

        // If file contains scores,
        // iterate through the records to compare new score
        // If the score is higher, insert it into the records list
        if(records.Count > 0)
        {
            for(int i = 0; i < records.Count; i++)
            {
                // Check if new score beats a score
                if(Points > records[i].score)
                {
                    // Remove score being replaced and insert new score
                    if(records.Count == numTopScores)
                        records.RemoveAt(i);
                    records.Insert(i, new UserRecord(initials, Points));
                    break;
                }
                else if(records.Count < numTopScores) // Just add if leaderboard isnt full
                {
                    records.Insert(i, new UserRecord(initials, Points));
                    break;
                }
            }
        }
        else // Adding the first record
        {
            records.Add(new UserRecord(initials, Points));
        }


        // Get the file directory
        string dirPath = Path.GetDirectoryName(filePath);

        // Create the file if it doesn't exist
        if (!Directory.Exists(dirPath))
        {
            Directory.CreateDirectory(dirPath);
        }

        // Sort the leaderboard based on score
        records.Sort((record1, record2) => record1.score.CompareTo(record2.score));

        // String of scores to add to the file
        string scores = "";
        for(int i = 0; i < numTopScores; i++)
        {
            // Early break if num scores doesnt hit the max
            if (i > records.Count - 1)
                break;

            // Stringify each record
            scores += $"{records[i].initials}:{records[i].score}";

            // If note last recorded score, add new line
            if(i < numTopScores - 1)
            {
                scores += "\n";
            }

            // Write text to the file
            File.WriteAllText(filePath, scores);
        }
    }
}
