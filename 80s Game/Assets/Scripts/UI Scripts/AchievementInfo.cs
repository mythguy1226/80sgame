using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class AchievementInfo : MonoBehaviour
{
    public string achievementNameText;
    public string descriptionText;
    public TextMeshProUGUI achievementName;
    public TextMeshProUGUI description;

    public AchievementsUI achievementsUI;

    public void Start()
    {
        achievementName.text = achievementNameText;
        description.text = descriptionText;
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
