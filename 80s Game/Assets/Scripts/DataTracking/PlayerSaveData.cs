using System;

[Serializable]
public class PlayerSaveData
{
    // Data class to represent our players, their performance and
    // gameplay configuration

    public int shotsFired;
    public int shotsHit;
    public ModifierData modifiersCollected;
    public string device;
    public string deviceMake;
    public SensitivityData sensitivity;
    public float[] crossHairColor;
    public int crossHairIndex;
}

[Serializable]
public class SensitivityData
{
    // Serialized sub object for the sensitivity configuration for each player
    public float x;
    public float y;

    public SensitivityData(float x, float y)
    {
        this.x = x; this.y = y;
    }
}