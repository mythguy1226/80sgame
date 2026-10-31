using System.Collections.Generic;
/// <summary>
/// Game mode adapted for the rules of classic mode in the campaign
/// </summary>
public class CampaignClassicMode : AbsCampaignMode
{
    LevelDataSO levelDataSpec;
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
        SetupAllowedData();
    }

    protected override void CallNextRound()
    {
        throw new System.NotImplementedException();
    }

    protected override void EndGame()
    {
        throw new System.NotImplementedException();
    }

    protected override int GetNextAvailableBat()
    {
        throw new System.NotImplementedException();
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
        throw new System.NotImplementedException();
    }

    protected override void UpdateRoundParams()
    {
        
    }
}