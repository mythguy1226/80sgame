using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetManager : MonoBehaviour
{
    // Fields for all targets and spawn locations
    Target[] targets;
    public List<Vector3> spawnLocations;

    // Default values
    public int firstRoundTargetCount = 5;
    int currentRound = 1;
    int currentRoundSize;

    // Start is called before the first frame update
    void Start()
    {
        // Get all targets currently in the scene
        targets = GameObject.FindObjectsOfType<Target>();

        // Begin the first round and set the round size tracker
        // to be equal to the first round's size
        StartFirstRound();
        currentRoundSize = firstRoundTargetCount;
    }

    // Method for spawning first round of targets
    void StartFirstRound()
    {
        // Check list isn't empty
        if(targets.Length > 0)
        {
            // Iterate through and spawn the first set of targets
            for(int i = 0; i < firstRoundTargetCount; i++)
            {
                int targetIndex = GetNextAvailableTarget();
                if(targetIndex >= 0)
                {
                    SpawnTarget(targetIndex);
                }
            }
        }
    }

    // Method for updating round parameters
    void UpdateRoundParameters()
    {
        // TO-DO
        // When we get to future rounds
    }

    // Method for starting the next round
    void StartNextRound()
    {
        // TO-DO
        // When we get to future rounds
    }

    // Method for spawning more targets if needed
    void SpawnMoreTargets()
    {
        // TO-DO
        // When we get to future rounds
    }

    // Method for spawning a target
    void SpawnTarget(int targetIndex)
    {
        // Get the spawn point randomly and teleport the target to that point
        Vector3 spawnPoint = spawnLocations[Random.Range(0, spawnLocations.Count - 1)];
        targets[targetIndex].transform.position = spawnPoint;

        // Update on-screen status
        targets[targetIndex].isOnScreen = true;
    }

    // Method for getting all targets currently on screen
    void GetAllTargetsOnScreen()
    {
        // TO-DO
        // When we get to future rounds
    }

    // Method for getting the next available target
    int GetNextAvailableTarget()
    {
        // Iterate through the targets until you
        // find one that isn't already on screen
        for(int i = 0; i < targets.Length; i++)
        {
            // Check if on screen
            if(!targets[i].isOnScreen)
            {
                return i;
            }
        }

        return -1;
    }
}
