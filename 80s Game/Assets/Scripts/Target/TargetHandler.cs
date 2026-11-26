using GameplayEvents;

public class TargetSpawnRequestHandler : IGameplayEventHandler<SpawnRequest>
{
    TargetManager tm;
    public TargetSpawnRequestHandler(TargetManager tm)
    {
        this.tm = tm;
    }

    public void Handle(SpawnRequest request)
    {
        // Check if screen is now full
        if (tm.ActiveTargets.Count == request.TargetLimit)
            return;

        int spawnIndex = request.TargetIndex;

        if (spawnIndex == -1)
        {
            spawnIndex = tm.GetNextAvailableTargetOfEnumType(request.TargetType);
        }

        if (spawnIndex == -1) {
            return;
        }
        tm.SpawnTarget(spawnIndex);
    }
}

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
        if (!t.FSM.InUnscorableState() && targetResetMessage.RelatedPlayer != null)
        {
            AchievementManager.HandleStunAchievements(t.type);
            tm.AddToCount(t.type, tm.killCount);
        }
        tm.OnTargetReset();
    }
}