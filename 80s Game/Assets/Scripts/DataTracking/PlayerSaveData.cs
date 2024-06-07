using System;

[Serializable]
public class PlayerSaveData
{
    public int shotsFired;
    public int shotsHit;
    public ModifierData modifiersCollected;
    public string device;
    public string deviceMake;
    public SensitivityData sensitivity;
}

[Serializable]
public class SensitivityData
{
    public float x;
    public float y;

    public SensitivityData(float x, float y)
    {
        this.x = x; this.y = y;
    }
}