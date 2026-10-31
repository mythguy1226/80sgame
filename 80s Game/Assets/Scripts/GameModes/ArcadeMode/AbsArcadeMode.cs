public abstract class AbsArcadeMode : AbsGameMode
{
    protected AbsArcadeMode() : base()
    {
        targetManager = GameManager.Instance.TargetManager;
    }
}