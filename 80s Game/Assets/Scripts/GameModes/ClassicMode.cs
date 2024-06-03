using System;
using System.Collections;
using System.Collections.Generic;

public class ClassicMode : AbsGameMode
{

    public ClassicMode() : base()
    {
        ModeType = EGameMode.Classic;

        // Initial round parameters
        NumRounds = 10;
        maxTargetsOnScreen = 8;
        currentRoundTargetCount = 5;
    }

    protected override void StartNextRound(bool isFirstRound = false)
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

    protected override void UpdateRoundParams()
    {
        CurrentRound++;
        currentRoundTargetCount += 2;
        maxTargetsOnScreen += 1;
        // Keep max targets on screen to at most two fewer than object pool
        if(maxTargetsOnScreen >= targetManager.targets.Count)
        {
            maxTargetsOnScreen = targetManager.targets.Count - 2;
        }

        targetManager.numStuns = 0;
        targetManager.UpdateTargetParams();
        if (GameManager.Instance.roundEndTheme != null)
            SoundManager.Instance.PlayNonloopMusic(GameManager.Instance.roundEndTheme);
    }

    protected override int GetNextAvailableBat()
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

    public override void OnTargetReset()
    {
        if(targetManager.totalStuns % 3 == 0)
            numBonusBats++;

        if(targetManager.numStuns >= currentRoundTargetCount)
        {

            // If last round completed
            if(CurrentRound == NumRounds)
                GameOver = true;
            // Otherwise start next round
            else
            {
                GameManager.EmitRoundOverEvent();
                StartNextRound();
            }
                

            return;
        }

        // If not the end of a round, check if more targets can be spawned
        SpawnMoreTargets();
    }

    private void SpawnMoreTargets()
    {
        // Check if the player still needs stuns for the round
        int neededStuns = currentRoundTargetCount - targetManager.numStuns;
        if (neededStuns <= 0)
            return;

        // If maximum number of targets isn't on screen
        if (
            targetManager.ActiveTargets.Count < maxTargetsOnScreen 
            && targetManager.ActiveTargets.Count < neededStuns
        )
        {
            int targetIndex = GetNextAvailableBat();

            if (targetIndex >= 0)
                targetManager.SpawnTarget(targetIndex);
        }
    }
}
