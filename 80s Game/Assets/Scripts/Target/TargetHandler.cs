using GameplayEvents;


public class TargetSpawnHandler : IGameplayEventHandler<TargetMessage>
{
    TargetManager tm;
    public TargetSpawnHandler(TargetManager tm)
    {
        this.tm = tm;
    }

    public void Handle(TargetMessage targetSpawnedMessage)
    {
        tm.RegisterActiveTarget(targetSpawnedMessage.Target);
    }
}

public class TargetManagementResetHandler : IGameplayEventHandler<TargetMessage>
{
    TargetManager tm;
    public TargetManagementResetHandler(TargetManager tm)
    {
        this.tm = tm;
    }

    public void Handle(TargetMessage targetResetMessage)
    {
        
        Target t = targetResetMessage.Target;
        tm.RemoveActiveTarget(t);
        if (!t.FSM.InUnscorableState())
        {
            if (t.stunningPlayer != null)
            {
                AchievementManager.HandleStunAchievements(t.type);
                tm.AddToCount(t.type, tm.killCount);
            }
        }
        tm.OnTargetReset();
    }
}