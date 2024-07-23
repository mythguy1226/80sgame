using System.Collections.Generic;
using UnityEngine;

public class AchievementLoader : MonoBehaviour
{
    public List<AchievementData> achievements;

    /// <summary>
    /// Class only exists to load data into the achievement manager
    /// </summary>
    private void Start()
    {
        AchievementManager.RegisterRequirements(achievements);
    }
}