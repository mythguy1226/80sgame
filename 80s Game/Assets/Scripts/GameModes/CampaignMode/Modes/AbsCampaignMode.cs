using System;

public abstract class AbsCampaignMode : AbsGameMode
{
    public Action onGameEnd;
    public Action onRoundEnd;
    protected AbsCampaignMode() : base() { }
    public abstract void SetupMode(LevelDataSO data, TargetManager manager);

}