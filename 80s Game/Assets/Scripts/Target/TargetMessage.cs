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
}
