using System.Collections.Generic;
using UnityEngine;

public class AchievementLoader : MonoBehaviour
{
    public List<AchievementData> achievements;
    private void Start()
    {
        AchievementManager.RegisterRequirements(achievements);
    }
}