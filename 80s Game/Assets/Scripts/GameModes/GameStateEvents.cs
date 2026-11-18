namespace GameplayEvents
{
    public enum GameStateEventType
    {
        RoundEnd,
        GameEnd
    }

    public class GameStateEvent : IGameplayMessage
    {
        public GameStateEventType EventType;
        public GameStateEvent(GameStateEventType eventType) { 
            EventType = eventType;
        }
    }
}