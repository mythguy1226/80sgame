/// <summary>
/// Static class that holds the information for the game file currently being played
/// </summary>
public static class CampaignSave
{
    public static int LoadedIndex { get; set; }
    public static GameFileData FileData {  get; set; }
    public static GameFileSaver FileSaver { get; set; }

    public static LevelDataSO ActiveLevel { get; set; }

    public static void Reset()
    {
        FileData = null;
        LoadedIndex = -1;
    }
}