using GameplayEvents;
using Localization;
using Steamworks;
using System;
using UnityEngine;

namespace SteamIntegration
{
    public class SteamInterface : MonoBehaviour, IGameplayListener<SteamAchievementEvent>, IGameplayListener<SteamDataUpdateEvent>, IGameplayListener<SteamServerUpdate>
    {
        private bool _connected = false;
        public bool IsConnected {  get { return _connected; } }
        protected Callback<GameOverlayActivated_t> m_GameOverlayActivated;
        public static Action gameOverlayEvent;
        public SupportedLanguages gameLanguage;
        SteamAchievementsInterface achievementsInterface;

        private void OnEnable()
        {
            if (SteamManager.Initialized)
            {
                GameEventBus.Register<SteamAchievementEvent>(this);
                GameEventBus.Register<SteamDataUpdateEvent>(this);
                GameEventBus.Register<SteamServerUpdate>(this);
                m_GameOverlayActivated = Callback<GameOverlayActivated_t>.Create(OnGameOverlayActivated);
                
                //Configure language
                gameLanguage = LocalizationManager.GetLangEnum(SteamApps.GetCurrentGameLanguage());

                achievementsInterface = new SteamAchievementsInterface();
                _connected = true;
            }
        }

        // When the steam overlay is activated, the game should pause.
        // Doesn't work in-editor. http://steamworks.github.io/faq/
        private void OnGameOverlayActivated(GameOverlayActivated_t callbackData)
        {
            gameOverlayEvent?.Invoke();
        }

        public void OnGameplayEvent(SteamDataUpdateEvent callbackData) { 
            SetSteamData(callbackData.key, callbackData.value, callbackData.updateIfGreater);
            UpdateSteamServer();
        }

        public void OnGameplayEvent(SteamAchievementEvent callbackData) { 
            
        }

        public void OnGameplayEvent(SteamServerUpdate callbackData)
        {
            UpdateSteamServer();
        }

        public void UnlockSteamAchievement(string key)
        {
            if (_connected)
            {
                achievementsInterface.UnlockAchievement(key);
            } else
            {
                Debug.LogWarning("Steam Achievement API failed");
            }
        }

        private void SetSteamData(string key, int value, bool updateIfGreater = false)
        {
            if (!_connected)
            {
                Debug.LogWarning("Steam Data API failed");
                return;
            }

            achievementsInterface.UpdateStat(key, value, updateIfGreater);
        }

        public bool InitData() {
            if (!_connected)
            {
                Debug.LogWarning("Steam Data API failed");
                return false;
            }
            bool serverRequest = achievementsInterface.ServerStatRequest();
            return serverRequest;

        }

        private void UpdateSteamServer()
        {
            if (!_connected)
            {
                Debug.LogWarning("Steam Data API failed");
                return;
            }
            bool result = achievementsInterface.StoreUserStats();
            if (!result)
            {
                Debug.LogError("Steam Server Data Update Failed!");
            }
        }

        public void OnDisable()
        {
            gameOverlayEvent = null;
            GameEventBus.Unregister<SteamAchievementEvent>(this);
            GameEventBus.Unregister<SteamDataUpdateEvent>(this);
            GameEventBus.Unregister<SteamServerUpdate>(this);
        }

        public void OnDestroy()
        {
            gameOverlayEvent = null;
            GameEventBus.Unregister<SteamAchievementEvent>(this);
            GameEventBus.Unregister<SteamDataUpdateEvent>(this);
            GameEventBus.Unregister<SteamServerUpdate>(this);
        }
    }
}
