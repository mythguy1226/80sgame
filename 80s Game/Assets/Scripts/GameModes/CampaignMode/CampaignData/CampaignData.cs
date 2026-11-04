using System;

public enum LevelType
{
    Classic,
    TimeTrial,
    Defense,
    Boss,
    Shop
}

public enum ObjectiveType
{
    Score,
    Accuracy,
    RemainingTime,
    RoundCount,
    ShootBatType,
    RemainingHealth,
    OverheatCounter,
    PowerUpsCollected
}

[Serializable]
public struct AllowedSpawns
{
    public TargetManager.TargetType[] allowedSpawns;

    public override string ToString()
    {
        return allowedSpawns.Length.ToString();
    }
}

[Serializable]
public class RoundParameters
{
    public int MaxRounds;
    public int MaxTargetPerRound;
    public int TargetsPerRoundIncrement;
    public int[] RoundTargetCount;
}
