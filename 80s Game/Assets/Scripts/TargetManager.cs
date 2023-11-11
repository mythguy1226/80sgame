using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetManager : MonoBehaviour
{
    // Fields for all targets and spawn locations
    public static Target[] targets;
    public List<Vector3> spawnLocations;

    // Default values
    public int firstRoundTargetCount = 5;
    public int currentRound = 1;
    public int currentRoundSize;
    public int numStuns = 0;
    public bool gameOver = false;
    int maxTargetsOnScreen = 8;
    public int numRounds = 10;

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
    public void UpdateRoundParameters()
    {
        // Reset stuns and update round counter
        currentRound++;
        numStuns = 0;

        // Update round size and arena max
        currentRoundSize += 2;
        maxTargetsOnScreen += 1;
        if (maxTargetsOnScreen >= targets.Length)
        {
            maxTargetsOnScreen = targets.Length - 2;
        }
    }

    // Method for starting the next round
    public void StartNextRound()
    {
        // Check list isn't empty
        if (targets.Length > 0)
        {
            // Iterate through and spawn the next set of targets
            for (int i = 0; i < currentRoundSize; i++)
            {
                int targetIndex = GetNextAvailableTarget();
                if (targetIndex >= 0)
                {
                    SpawnTarget(targetIndex);

                    // Check if screen is now full
                    if (GetAllTargetsOnScreen().Length == maxTargetsOnScreen)
                        return;
                }
            }
        }
    }

    // Method for spawning more targets if needed
    public void SpawnMoreTargets()
    {
        // Check if stuns are needed
        int neededStuns = currentRoundSize - numStuns;
        if (neededStuns > 0)
        {
            if (GetAllTargetsOnScreen().Length < neededStuns)
            {
                // Check that screen isn't full
                if (GetAllTargetsOnScreen().Length < maxTargetsOnScreen)
                {
                    // Spawn a target
                    int targetIndex = GetNextAvailableTarget();
                    if (targetIndex >= 0)
                    {
                        SpawnTarget(targetIndex);
                    }
                }
            }
        }
    }

    // Method for spawning a target
    void SpawnTarget(int targetIndex)
    {
        // Get the spawn point randomly and teleport the target to that point
        Vector3 spawnPoint = spawnLocations[Random.Range(0, spawnLocations.Count - 1)];
        targets[targetIndex].transform.position = spawnPoint;

        // Update on-screen status
        targets[targetIndex].isOnScreen = true;
        targets[targetIndex].currentState = TargetStates.Moving;
        targets[targetIndex].GetComponent<AnimationHandler>().ResetAnimation();
        targets[targetIndex].GetComponent<CircleCollider2D>().isTrigger = false;
        targets[targetIndex].SetFleeTimer();
    }

    // Method for getting all targets currently on screen
    public Target[] GetAllTargetsOnScreen()
    {
        // Init counter
        List<Target> tempTargetList = new List<Target>();

        // Iterate through all targets and check if they're on screen
        foreach(Target target in targets)
        {
            // Add to counter if target is on screen
            if (target.isOnScreen)
                tempTargetList.Add(target);
        }

        // Init array for transfer
        Target[] targetsOnScreen = new Target[tempTargetList.Count];

        // Iterate through the temp list and add to target array
        for (int i = 0; i < tempTargetList.Count; i++)
        {
            targetsOnScreen[i] = tempTargetList[i];
        }

        return targetsOnScreen;
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
