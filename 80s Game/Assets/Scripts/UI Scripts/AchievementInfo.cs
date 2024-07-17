using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AchievementInfo : MonoBehaviour
{
    public TextMeshProUGUI title;
    public TextMeshProUGUI description;

    public AchievementsUI achievementsUI;

    public void SelectAchievement()
    {
        achievementsUI.UpdateInfoPanel(this);
    }
}
