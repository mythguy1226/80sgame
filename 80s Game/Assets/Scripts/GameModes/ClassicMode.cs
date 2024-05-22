using System;
using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

public class ClassicMode : AbsGameMode
{
    public int numRounds = 10;
    public int currentRound = 1;
    public int currentRoundTargetCount = 0;
    public int maxTargetsOnScreen = 8;
    public int numBonusBats = 0;
    public TargetManager targetManager;

    public ClassicMode()
    {
        // Target count for first round
        currentRoundTargetCount = 5;
        targetManager = GameManager.Instance.TargetManager;
        StartNextRound(true);
    }

    private void StartNextRound(bool isFirstRound = false)
    {
        if (!isFirstRound)
            UpdateRoundParams();

        // Iterate through and spawn the next set of targets
        for(int i = 0; i < currentRoundTargetCount; i++)
        {
            int targetIndex = GetNextAvailableBat();

            if (targetIndex == -1)
                continue;

            targetManager.SpawnTarget(targetIndex);

            // Check if screen is now full
            if (targetManager.ActiveTargets.Count == maxTargetsOnScreen)
                return;
        }
    }

    private void UpdateRoundParams()
    {
        currentRound++;
        currentRoundTargetCount += 2;
        maxTargetsOnScreen += 1;
        // Keep max targets on screen to at most two fewer than object pool
        if(maxTargetsOnScreen >= targetManager.targets.Count)
        {
            maxTargetsOnScreen = targetManager.targets.Count - 2;
        }

        targetManager.numStuns = 0;
        targetManager.UpdateTargetParams();
    }

    public override int GetNextAvailableBat()
    {
        List<Target> bats = targetManager.targets;

        // Iterate through the targets until you
        // find one that isn't already on screen
        for (int i = 0; i < bats.Count; i++)
        {
            if (bats[i].FSM.bIsActive)
                continue;

            // If default bat, return index if no bonus bats
            // Otherwise continue
            if (bats[i].FSM.IsDefault && numBonusBats == 0)
            {
                return i;
            }
            else if (numBonusBats > 0) // Bonus bat spawning
            {
                numBonusBats--;
                return i;
            }
        }

        return -1;
    }

    public override void OnTargetStun()
    {
        if(targetManager.totalStuns % 3 == 0)
            numBonusBats++;

        if(targetManager.numStuns >= currentRoundTargetCount)
        {
            // Add bonus points
            GameManager.Instance.PointsManager.AddBonusPoints(
                GameManager.Instance.HitsManager.Accuracy
            );

            // If last round completed
            if(currentRound == numRounds)
                gameOver = true;

            return;
        }

        // If not the end of a round, check if more targets can be spawned
        SpawnMoreTargets();
    }

    private void SpawnMoreTargets()
    {
        // Check if the player still needs stuns for the round
        if (currentRoundTargetCount - targetManager.numStuns <= 0)
            return;

        // If maximum number of targets isn't on screen
        if(targetManager.ActiveTargets.Count < maxTargetsOnScreen)
        {
            int targetIndex = GetNextAvailableBat();
            if (targetIndex >= 0)
                targetManager.SpawnTarget(targetIndex);
        }
    }
}
