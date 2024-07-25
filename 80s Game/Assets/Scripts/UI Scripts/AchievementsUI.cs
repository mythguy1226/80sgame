using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class AchievementsUI : MonoBehaviour
{
    public Scrollbar achievementScrollbar;
    public TextMeshProUGUI infoPanelName;
    public TextMeshProUGUI infoPanelDescription;
    public TextMeshProUGUI infoPanelLore;
    public TextMeshProUGUI progressNum;
    public Image progressBar;
    public GameObject achievementContent;
    public GameObject achievementItemPrefab;

    // Start is called before the first frame update
    void Start()
    {
        achievementScrollbar.value = 1;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateInfoPanel(AchievementInfo achievement)
    {
        infoPanelName.text = achievement.achievementName.text;
        infoPanelDescription.text = achievement.description.text;

        if (achievement.data.isUnlocked())
        {
            infoPanelLore.text = achievement.data.descriptionText;
            infoPanelLore.alignment = TextAlignmentOptions.Left;
        }

        else
        {
            infoPanelLore.text = "Files Encrypted";
            infoPanelLore.alignment = TextAlignmentOptions.Center;
        }
    }
    
    public void CreateAchievements(List<AchievementData> achievements)
    {
        foreach (AchievementData achievement in achievements)
        {
            achievementItemPrefab.GetComponent<AchievementInfo>().data = achievement;

            Instantiate(achievementItemPrefab, achievementContent.transform);
        }
    }
}
