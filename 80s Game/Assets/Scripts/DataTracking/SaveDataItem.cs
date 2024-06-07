using System;

[Serializable]
public class SaveDataItem
{
    // Consolidated data object that represents the telemetry on player activity that we save.
    public string gameMode;
    public BatData batsSpawned;
    public BatData batsShot;
    public int duration;
    public int playerCount;
    public PlayerSaveData[] playerData;
}