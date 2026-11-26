using GameplayEvents;

public enum TargetEventType
{
    Spawn,
    Reset
}

public class TargetMessage : IGameplayMessage
{
    public TargetEventType EventType { get; private set; }
    public Target Target { get; private set; }
    public TargetMessage(Target target, TargetEventType eventType)
    {
        Target = target;
        EventType = eventType;
    }

    public PlayerController RelatedPlayer;
}

public class SpawnRequest : IGameplayMessage
{
    public TargetType TargetType { get; private set; }
    public int TargetIndex { get; private set; }

    public int TargetLimit { get; private set; }

    public SpawnRequest(int targetIndex, int targetLimit)
    {
        TargetIndex = targetIndex;
        TargetLimit = targetLimit;
    }
    public SpawnRequest(TargetType type, int targetLimit)
    {
        TargetIndex = -1;
        TargetType = type;
        TargetLimit = targetLimit;
    }
}
