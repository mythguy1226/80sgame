using System;

/// <summary>
/// Holds the information regarding the completion status of a level for a particular game file
/// </summary>
[Serializable]
public struct LevelClearInformation
{
    public int levelId;
    public int starsObtained;
    public bool cleared;
}


/// <summary>
/// Data class that holds the saved data for a single player campaign file
/// Should hold no behavior, only data
/// </summary>
[Serializable]
public class GameFileData
{
    public string owner = "";
    public LevelClearInformation[] levelClearInformation;
    public int playerAvatarLocation = -1;

}