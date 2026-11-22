namespace GameplayEvents
{
    public enum GameStateEventType
    {
        RoundEnd,
        GameEnd,
        Quit
    }

    public class GameStateEvent : IGameplayMessage
    {
        public GameStateEventType EventType;
        public EGameMode GameMode;
        public GameStateEvent(GameStateEventType eventType) { 
            EventType = eventType;
        }
    }
}