public static class GameModeFactory
{

    public static AbsGameMode MakeCampaignGameMode(LevelType type)
    {

        switch(type){
            default:
                return new CampaignClassicMode();
        }
    }
}