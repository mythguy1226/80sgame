public static class GameModeData
{
    public static EGameMode activeGameMode;
    

    public static int GameModeToSceneIndex()
    {
        switch (activeGameMode)
        {
            case EGameMode.Competitive:
                return 3;
            default:
                return 2;
        }
    }
}