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
    int totalStuns = 0;
    int numBonusBats = 0;
    public bool gameOver = false;
    int maxTargetsOnScreen = 8;
    public int numRounds = 10;

    // Speed values
    float minSpeed = 3.0f;
    float maxSpeed = 3.5f;

    // Scale values
    float minScale = 2.8f;
    float maxScale = 3.5f;

    // Start is called before the first frame update
    void Start()
    {
        // Get all targets currently in the scene
        targets = GameObject.FindObjectsOfType<Target>();

        // Begin the first round and set the round size tracker
        // to be equal to the first round's size
        currentRoundSize = firstRoundTargetCount;
        Invoke("StartFirstRound", 0.1f);
    }

    // Method for spawning first round of targets
    void StartFirstRound()
    {
        // Update initial target speeds
        UpdateRoundSpeeds();

        // Switch initial target scales
        UpdateRoundScaling();

        // Check list isn't empty
        if (targets.Length > 0)
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

        // Update min and max target speeds
        minSpeed += 0.1f;
        if (minSpeed >= 3.5f)
            minSpeed = 3.5f;

        maxSpeed += 0.1f;
        if (maxSpeed >= 4.0f)
            maxSpeed = 4.0f;
        

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
        Target target = targets[targetIndex];
        // Get the spawn point randomly and teleport the target to that point
        Vector3 spawnPoint = spawnLocations[Random.Range(0, spawnLocations.Count - 1)];
        target.transform.position = spawnPoint;

        // Start particles on spawn
        if (target.GetComponentInChildren<ParticleSystem>())
        {
            target.GetComponentInChildren<ParticleSystem>().Play();
        }

        // Update on-screen status
        target.FSM.bIsActive = true;
        target.FSM.TransitionToState(BatStateMachine.BatStates.Moving);
        target.GetComponent<AnimationHandler>().ResetAnimation();
        target.GetComponent<PolygonCollider2D>().isTrigger = false;
        target.FSM.SetFleeTimer();
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
            if (target.FSM.bIsActive)
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
            if(!targets[i].FSM.bIsActive)
            {
                // Default bat spawning
                if (targets[i].FSM.pointValue == 1000.0f)
                {
                    // Ensure there are no bonus bats to be spawned
                    if (numBonusBats == 0)
                        return i;
                    else
                        continue;
                }
                else // Bonus bat spawning
                {
                    // Ensure there are bonus bats to be spawned
                    if (numBonusBats > 0)
                    {
                        numBonusBats--;
                        return i;
                    }
                    else
                        continue;
                }
            }
        }

        return -1;
    }

    // Method used for updating values on bat death
    public void OnTargetDeath(AbsBaseState<BatStateMachine.BatStates> targetState)
    {
        // Update number of stuns
        numStuns++;
        totalStuns++;

        // Add a bonus bat to spawn every 3 stuns
        if (totalStuns % 3 == 0)
            numBonusBats++;

        // Compare number of stuns needed vs round size
        if (numStuns >= currentRoundSize)
        {
            // Only add points if target didn't flee
            if (targetState.StateKey != BatStateMachine.BatStates.Fleeing)
            {
                // Add bonus points
                PointsManager pointsManager = GameManager.Instance.PointsManager;
                pointsManager.AddBonusPoints(GameManager.Instance.HitsManager.Accuracy);
            }

            // Take into account the round cap
            if (currentRound == numRounds)
            {
                gameOver = true;
                return;
            }

            // Update params
            UpdateRoundParameters();

            // Update all target speeds once new round has started
            UpdateRoundSpeeds();

            // Switch up target scaling
            UpdateRoundScaling();

            // Begin the next round
            StartNextRound();
        }
        else
        {
            // Try spawning another target
            SpawnMoreTargets();
        }
    }

    // Method for updating target speeds to be within range of round mins and maxes
    void UpdateRoundSpeeds()
    {
        // Set target speeds to be in a random range
        // between the min and max values
        foreach(Target target in targets)
        {
            if(target != null)
                target.FSM.UpdateSpeed(Random.Range(minSpeed, maxSpeed));
        }
    }

    // Method for changing bat scaling
    void UpdateRoundScaling()
    {
        // Set target transform scales
        // to be between min and max values
        foreach(Target target in targets)
        {
            // Get random scale value
            float newScale = Random.Range(minScale, maxScale);

            // Update target local scale
            target.gameObject.transform.localScale = new Vector3(newScale, newScale, newScale);
        }
    }
}
