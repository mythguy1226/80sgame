using Localization;
using Steamworks;
using System;
using UnityEngine;

namespace SteamIntegration
{
    public class SteamInterface : MonoBehaviour
    {
        protected Callback<GameOverlayActivated_t> m_GameOverlayActivated;
        public static Action gameOverlayEvent;
        public SupportedLanguages gameLanguage;

        private void OnEnable()
        {
            if (SteamManager.Initialized)
            {
                m_GameOverlayActivated = Callback<GameOverlayActivated_t>.Create(OnGameOverlayActivated);
                
                //Configure language
                gameLanguage = LocalizationManager.GetLangEnum(SteamApps.GetCurrentGameLanguage());
            }
        }

        // When the steam overlay is activated, the game should pause.
        // Doesn't work in-editor. http://steamworks.github.io/faq/
        private void OnGameOverlayActivated(GameOverlayActivated_t callbackData)
        {
            gameOverlayEvent?.Invoke();
        }
    }
}
