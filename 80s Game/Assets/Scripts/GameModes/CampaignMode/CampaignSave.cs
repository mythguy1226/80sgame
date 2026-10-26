public static class CampaignSave
{
    public static int LoadedIndex { get; set; }
    public static GameFileData FileData {  get; set; }
    public static GameFileSaver FileSaver { get; set; }

    public static void Reset()
    {
        FileData = null;
        LoadedIndex = -1;
    }
}