using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerScoreController
{
    // This class encapsulates the accuracy-keeping responsibility for each player object.
    private int _shotsFired;

    public int ShotsFired { get => _shotsFired; }

    /// <summary>
    /// Returns all shots that have recorded targets
    /// </summary>
    public int ShotsLanded { get
        {
            return shotHitsMap.Count(shot => shot.Value.Count > 0);
        }
    }

    public int pointsMod = 1;

    // Stores shots and sets of their related hits
    private Dictionary<int, HashSet<Target>> shotHitsMap;

    public PlayerScoreController()
    {
        _shotsFired = 0;
        shotHitsMap = new Dictionary<int, HashSet<Target>>();
    }

    // Record keeping functions for managing accuracy
    /// <summary>
    /// Adds a new Target set for each shot made
    /// </summary>
    public void AddShot()
    {
        shotHitsMap[_shotsFired++] = new HashSet<Target>();
    }

    // Subject to race conditions with AddShot
    /// <summary>
    /// Called by a Modifier whenever shot to balance out the
    /// extra AddShot call made by getting modifiers.
    /// </summary>
    public void AdjustForModShot()
    {
        shotHitsMap.Remove(--_shotsFired);
    }

    // Subject to race conditions with AddShot
    /// <summary>
    /// Adds targets to a given shot's target set
    /// </summary>
    /// <param name="target"></param>
    public void AddHit(Target target)
    {
        // Runs after each AddShot, so adds target to set of _shotsFired - 1
        shotHitsMap[_shotsFired - 1].Add(target);
    }

    public float GetAccuracy()
    {
        if (_shotsFired == 0)
        {
            return 0.0f;
        }
        return ShotsLanded / (float)_shotsFired;
    }
}