namespace GameplayEvents
{
    public class SteamDataUpdateEvent : IGameplayMessage
    {
        public string key;
        public int value;
        public bool updateIfGreater = false;

        public SteamDataUpdateEvent(string key, int value, bool updateIfGreater = false)
        {
            this.key = key;
            this.value = value;
            this.updateIfGreater = updateIfGreater;
        }
    }

    public class SteamAchievementEvent : IGameplayMessage
    {
        string key;
    }

    public class SteamServerUpdate : IGameplayMessage
    {

    }

    
}