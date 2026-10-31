public static class GameModeFactory
{

    public static AbsCampaignMode MakeCampaignGameMode(LevelType type)
    {

        switch(type){
            default:
                return new CampaignClassicMode();
        }
    }
}