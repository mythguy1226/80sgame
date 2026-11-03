using System;

public abstract class AbsCampaignMode : AbsGameMode
{
    public Action onGameEnd;
    protected AbsCampaignMode() : base() { }
    public abstract void SetupMode(LevelDataSO data);

}