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
}
