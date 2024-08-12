public static class GameModeData
{
    public static EGameMode activeGameMode;
    
    // Function that maps a GameMode enum to a scene index.
    // This is hard-coded and if build-settings change, this data might need to be re-synced.
    // We should consider re-working this into a more robust system.
    public static int GameModeToSceneIndex()
    {
        switch (activeGameMode)
        {
            case EGameMode.Defense:
                return 5;
            case EGameMode.Competitive:
                return 4;
            default:
                return 3;
        }
    }

    public static string GameModeToString()
    {
        switch (activeGameMode)
        {
            case EGameMode.Competitive:
                return "competitive";
            case EGameMode.Defense:
                return "defense";
            case EGameMode.Classic:
                return "classic";
            default:
                return "unreg";
        }
    }
}