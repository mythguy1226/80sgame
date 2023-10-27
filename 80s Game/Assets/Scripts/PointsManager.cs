using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointsManager : MonoBehaviour
{
    // Public property
    public int Points {  get; private set; }

    // Start is called before the first frame update
    void Awake()
    {
        Points = 0;
    }

    /// <summary>
    /// Adds a number of points
    /// </summary>
    /// <param name="numPoints"></param>
    /// <returns>The new point total</returns>
    public int AddPoints(int numPoints)
    {
        Points += numPoints;
        return Points;
    }

    /// <summary>
    /// Adds bonus points based off of an accuracy level
    /// </summary>
    /// <param name="accuracy"></param>
    /// <returns>The new point total</returns>
    public int AddBonusPoints(float accuracy)
    {
        // Bonus points are in the amount of (1
        int numBonusPoints = Mathf.RoundToInt(Points * accuracy);
        return AddPoints(numBonusPoints);
    }
}
