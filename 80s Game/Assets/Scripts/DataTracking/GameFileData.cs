using System;

[Serializable]
public struct LevelClearInformation
{
    public int levelId;
    public int starsObtained;
    public bool cleared;
}


[Serializable]
public class GameFileData
{
    public string owner = "";
    public LevelClearInformation[] levelClearInformation;
    public int playerAvatarLocation = -1;

}