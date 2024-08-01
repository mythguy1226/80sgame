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
        //Set the info for the achievement based off of the achievement data
        achievementName.text = data.nameText;
        description.text = data.requirementText;
        icon.sprite = data.image;

        //Make the icon full color if it is unlocked
        if (data.isUnlocked())
        {
            icon.color = Color.white;
        }

        //Darken the icon if it is locked
        else
        {
            icon.color = new Color(0.19f, 0.19f, 0.19f);
        }

        achievementsUI = GameObject.Find("Achievements Panel").GetComponent<AchievementsUI>();
    }

    public void Update()
    {
        //Update the info panel for whichever achievement is selected
        if (EventSystem.current.currentSelectedGameObject == this.gameObject)
        {
            achievementsUI.UpdateInfoPanel(this);
        }
    }
}
