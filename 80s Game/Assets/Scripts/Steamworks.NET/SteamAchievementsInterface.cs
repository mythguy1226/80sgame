using Steamworks;
using System.Collections.Generic;
using UnityEngine;


namespace SteamIntegration
{
    public class SteamAchievementsInterface
    {
        private const int appID = 3099010;
        private bool _requestStatusSuccess = false;

        public bool ServerStatRequest()
        {
            _requestStatusSuccess = SteamUserStats.RequestCurrentStats();
            return _requestStatusSuccess;
        }

        private int LoadStat(string statKey)
        {
            if (_requestStatusSuccess)
            {
                int data;
                SteamUserStats.GetStat(statKey, out data);
                return data;
            }
            return 0;
        }

        public void UpdateStat(string statKey, int value, bool updateIfGreater)
        {

            if (!_requestStatusSuccess) {
                Debug.LogError("Can't update stats");
            }

            if (!updateIfGreater) {
                SteamUserStats.SetStat(statKey, value);
                return;
            }

            int existingValue = LoadStat(statKey);
            if (value > existingValue) {
                SteamUserStats.SetStat(statKey, value);
            }
            
        }

        public void UnlockAchievement(string achievementKey)
        {
            SteamUserStats.SetAchievement(achievementKey);
        }

        public bool StoreUserStats()
        {
            return SteamUserStats.StoreStats();
        }
    }
}
