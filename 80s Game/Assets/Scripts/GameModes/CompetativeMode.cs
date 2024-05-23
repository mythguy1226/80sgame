using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompetativeMode : AbsGameMode
{
    public CompetativeMode() : base()
    {
        ModeType = EGameMode.Competitive;
        // numRounds = ...
    }

    protected override int GetNextAvailableBat()
    {
        throw new System.NotImplementedException();
    }

    protected override void StartNextRound(bool isFirstRound = false)
    {
        throw new System.NotImplementedException();
    }

    protected override void UpdateRoundParams()
    {
        throw new System.NotImplementedException();
    }
    public override void OnTargetReset()
    {
        throw new System.NotImplementedException();
    }

}
