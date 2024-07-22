using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class AchievementInfo : MonoBehaviour
{
    public TextMeshProUGUI achievementName;
    public TextMeshProUGUI description;

    public AchievementsUI achievementsUI;

    public void Update()
    {
        if (EventSystem.current.currentSelectedGameObject == this.gameObject)
        {
            achievementsUI.UpdateInfoPanel(this);
        }
    }

    public void SelectAchievement()
    {
        achievementsUI.UpdateInfoPanel(this);
    }
}
