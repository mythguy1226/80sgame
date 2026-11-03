using System.Collections.Generic;
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
        throw new System.NotImplementedException();
    }

    public override void SetupMode(LevelDataSO data)
    {
        RoundParameters roundParameters = data.GetStartingParameters();
        NumRounds = roundParameters.MaxRounds;
        currentRoundTargetCount = roundParameters.RoundTargetCount[CurrentRound];
        maxTargetsOnScreen = roundParameters.MaxTargetPerRound;
       
        levelDataSpec = data;
        spawnRates = levelDataSpec.GetSpawnRates();
        SetupAllowedData();
    }

    protected override void CallNextRound()
    {
        throw new System.NotImplementedException();
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
                continue;
            }



            if (bat.FSM.IsDefault())
            {
                return i;
            } else
            {
                return 0;
            }
        }
        return -1;
    }

    protected override void SetupAllowedData()
    {
        allowedBats.Clear();   
        AllowedSpawns roundSpawns = levelDataSpec.GetAllowedSpawns(CurrentRound);
        for (int i = 0; i < allowedBats.Count; i++) {
            allowedBats.Add(roundSpawns.allowedSpawns[i], true);
        }
    }

    protected override void StartNextRound(bool isFirstRound = false)
    {
        if (!isFirstRound)
        {
            UpdateRoundParams();
        }

        for (int i = 0; i < currentRoundTargetCount; i++)
        {
            int targetIndex = GetNextAvailableBat();
            if (targetIndex == -1 && allowedBats[TargetManager.TargetType.Regular])
            {
                targetIndex = targetManager.GetNextAvailableTargetOfEnumType(TargetManager.TargetType.Regular);
                if (targetIndex == -1) continue;
            }
            targetManager.SpawnTarget(targetIndex);

            // Check if screen is full
            if (targetManager.ActiveTargets.Count == maxTargetsOnScreen)
                return;
        }
    }

    protected override void UpdateRoundParams()
    {
        
    }
}