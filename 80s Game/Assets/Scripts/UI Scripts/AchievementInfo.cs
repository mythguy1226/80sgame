using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AchievementInfo : MonoBehaviour
{
    public AchievementData data;
    public TextMeshProUGUI achievementName;
    public TextMeshProUGUI description;
    public Image icon;

    public AchievementsUI achievementsUI;

    public void Start()
    {
        achievementName.text = data.nameText;
        description.text = data.requirementText;

        if (data.isUnlocked())
        {
            icon.color = Color.white;
        }

        else
        {
            icon.color = Color.gray;
        }

        achievementsUI = GameObject.Find("Achievements Panel").GetComponent<AchievementsUI>();
    }

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
