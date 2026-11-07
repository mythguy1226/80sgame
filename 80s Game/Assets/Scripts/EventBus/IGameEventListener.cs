namespace GameplayEvents
{
    public interface IGameplayListener<T> where T : IGameplayMessage
    {
        public void OnGameplayEvent(T GameplayMessage);
    }
}
