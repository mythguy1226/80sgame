using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointsManager : MonoBehaviour
{
    // Public property
    public int Points {  get; private set; }
    public int RoundPoints { get; private set; }

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
}
