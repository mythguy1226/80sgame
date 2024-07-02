using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CooperativeMode : AbsGameMode
{
    float modifierChance = 0.3f;

    public CooperativeMode() : base()
    {
        ModeType = EGameMode.Competitive;

        // Initial round parameters
        NumRounds = 5;
        maxTargetsOnScreen = 15;
        currentRoundTargetCount = 8;
        allowedBats = new Dictionary<TargetManager.TargetType, bool>();

        //Add allowed types
        allowedBats.Add(TargetManager.TargetType.Regular, true);
        allowedBats.Add(TargetManager.TargetType.Modifier, true);
        allowedBats.Add(TargetManager.TargetType.Bonus, true);
        allowedBats.Add(TargetManager.TargetType.Unstable, true);

        numBatsMap = new Dictionary<TargetManager.TargetType, int>();

        // Init map types
        numBatsMap.Add(TargetManager.TargetType.Regular, 0);
        numBatsMap.Add(TargetManager.TargetType.Modifier, 0);
        numBatsMap.Add(TargetManager.TargetType.DiveBomb, 0);
        numBatsMap.Add(TargetManager.TargetType.Unstable, 0);
    }

    protected override void StartNextRound(bool isFirstRound = false)
    {
        if (!isFirstRound)
            UpdateRoundParams();

        // Iterate through and spawn the next set of targets
        for (int i = 0; i < currentRoundTargetCount; i++)
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
        currentRoundTargetCount += 4;
        maxTargetsOnScreen += 1;
        // Keep max targets on screen to at most two fewer than object pool
        if (maxTargetsOnScreen >= targetManager.targets.Count)
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
            DefenseBatStateMachine FSM = (DefenseBatStateMachine)bats[i].FSM;
            if (FSM.bIsActive)
                continue;

            ModifierDefenseBatStateMachine comp = bats[i].GetComponent<ModifierDefenseBatStateMachine>();
            if (comp != null)
                continue;

            // Check for special bat types
            foreach(SpawnRate rate in GameManager.Instance.spawnConfig.rates)
            {
                // Check that type isnt regular
                if(rate.targetType != TargetManager.TargetType.Regular)
                {
                    // Check that there are special types available
                    if(numBatsMap[rate.targetType] > 0)
                    {
                        numBatsMap[rate.targetType]--;
                        return i;
                    }
                }
            }

            // If default bat return index, as there were no available
            // special bats to spawn previously
            if (FSM.IsDefault())
            {
                return i;
            }
        }

        return -1;
    }

    public override void OnTargetReset()
    {
        // Check rates map to see if any special bats should
        // be added to it
        foreach(SpawnRate rate in GameManager.Instance.spawnConfig.rates)
        {
            if(rate.targetType != TargetManager.TargetType.Regular)
            {
                if(targetManager.totalStuns % rate.spawnRate == 0)
                    numBatsMap[rate.targetType]++;
            }
        }

        // Start the next round if desired number of stuns is met
        if (targetManager.numStuns >= currentRoundTargetCount)
        {
            StartNextRound();

            // Spawn a modifier bat and increment target count
            targetManager.SpawnTarget(targetManager.GetNextAvailableTargetOfType<ModifierDefenseBatStateMachine>());
            currentRoundTargetCount++;

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

    /// <summary>
    /// Method calls end game logic and is public so it may be callable
    /// from defendable class when it detects core is destroyed
    /// </summary>
    public void EndCoopGame()
    {
        GameOver = true;
        EndGame();
    }
}
