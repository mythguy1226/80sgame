using UnityEngine.InputSystem;

public abstract class AbsCampaignMode : AbsGameMode
{
    protected AbsCampaignMode() : base() { }
    public abstract void SetupMode(LevelDataSO data);

}