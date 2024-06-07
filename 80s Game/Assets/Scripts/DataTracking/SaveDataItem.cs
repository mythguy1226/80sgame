using System;

[Serializable]
public class SaveDataItem
{
    public string gameMode;
    public BatData batsSpawned;
    public BatData batsShot;
    public int duration;
    public int playerCount;
    public PlayerSaveData[] playerData;
}