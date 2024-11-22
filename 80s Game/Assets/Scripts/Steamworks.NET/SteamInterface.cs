using Localization;
using Steamworks;
using System;
using UnityEngine;

namespace SteamIntegration
{
    public class SteamInterface : MonoBehaviour
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

        public void SetSteamData(string key, int value, bool updateIfGreater = false)
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

        public void UpdateSteamServer()
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

        public void OnDestroy()
        {
            gameOverlayEvent = null;
        }
    }
}
