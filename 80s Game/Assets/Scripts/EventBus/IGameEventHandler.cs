namespace GameplayEvents
{
    public interface IGameplayEventHandler<T> where T :IGameplayMessage
    {
        public void Handle(T message);
    }
}