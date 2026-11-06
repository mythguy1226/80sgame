using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Game mode adapted for the rules of classic mode in the campaign
/// </summary>
public class CampaignClassicMode : AbsCampaignMode
{
    LevelDataSO levelDataSpec;
    Dictionary<TargetManager.TargetType, int> spawnRates;
    public CampaignClassicMode() : base()
    {
        ModeType = EGameMode.CampaignClassic;
        CurrentRound = 0;
        allowedBats = new Dictionary<TargetManager.TargetType, bool>();
    }


    public override void OnTargetReset()
    {
        if (targetManager.numStuns >= currentRoundTargetCount)
        {
            if (CurrentRound == levelDataSpec.GetStartingParameters().MaxRounds)
            {
                GameOver = true;
                EndGame();
            } else
            {
                onRoundEnd?.Invoke();
            }
        }
    }

    public override void SetupMode(LevelDataSO data, TargetManager targetMan)
    {
        RoundParameters roundParameters = data.GetStartingParameters();
        NumRounds = roundParameters.MaxRounds;
        currentRoundTargetCount = roundParameters.RoundTargetCount[CurrentRound];
        maxTargetsOnScreen = roundParameters.MaxTargetPerRound;
        targetManager = targetMan;
        levelDataSpec = data;
        spawnRates = levelDataSpec.GetSpawnRates();
        SetupAllowedData();
    }

    protected override void CallNextRound()
    {
        StartNextRound();
    }

    protected override void EndGame()
    {
        onGameEnd?.Invoke();
    }

    protected override int GetNextAvailableBat()
    {
        List<Target> targets = targetManager.targets;

        for (int i = 0; i < targets.Count; i++)
        {
            Target bat = targets[i];
            if (debugMode && allowedBats[bat.type] && bat.FSM.IsActive())
            {
                return i;
            }

            if (SkipBat(bat))
            {
                Debug.Log("Skipping");
                continue;
            }

            if (bat.FSM.IsDefault())
            {
                return i;
            } else
            {
                // Pick from among the special bats
                // to do
                Debug.Log("Looking for special–");
                continue;
            }
        }
        Debug.Log("Defaulting out");
        // Could not find a bat to spawn
        return -1;
    }

    protected override void SetupAllowedData()
    {
        allowedBats.Clear();   
        AllowedSpawns roundSpawns = levelDataSpec.GetAllowedSpawns(CurrentRound);
        for (int i = 0; i < roundSpawns.allowedSpawns.Length; i++) {
            allowedBats.Add(roundSpawns.allowedSpawns[i], true);
        }
    }

    protected override void StartNextRound(bool isFirstRound = false)
    {
        if (!isFirstRound)
        {
            UpdateRoundParams();
        }
        Debug.Log(currentRoundTargetCount);
        for (int i = 0; i < currentRoundTargetCount; i++)
        {
            int targetIndex = GetNextAvailableBat();
            if (targetIndex == -1 && allowedBats[TargetManager.TargetType.Regular])
            {
                targetIndex = targetManager.GetNextAvailableTargetOfEnumType(TargetManager.TargetType.Regular);
                if (targetIndex == -1) continue;
            }
            Debug.Log("Spawning Enemy");
            targetManager.SpawnTarget(targetIndex);
            // Check if screen is full
            if (targetManager.ActiveTargets.Count == maxTargetsOnScreen)
                return;
        }
    }

    protected override void UpdateRoundParams()
    {
        RoundParameters roundParams = levelDataSpec.GetStartingParameters();
        CurrentRound++;
        currentRoundTargetCount += roundParams.RoundTargetCount[CurrentRound];
        maxTargetsOnScreen += roundParams.TargetsPerRoundIncrement;

        // Keep max targets on screen to at most two fewer than object pool
        if (maxTargetsOnScreen >= targetManager.targets.Count)
        {
            maxTargetsOnScreen = targetManager.targets.Count - 2;
        }

        targetManager.numStuns = 0;
        targetManager.UpdateTargetParams();
    }
}