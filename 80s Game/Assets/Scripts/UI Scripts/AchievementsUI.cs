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
    public GameObject progressBar;
    public TextMeshProUGUI progressNum;
    public Image progressBarFill;
    public GameObject achievementContent;
    public GameObject achievementItemPrefab;

    public List<Image> rewardThumbnails;

    // Start is called before the first frame update
    void Start()
    {
        achievementScrollbar.value = 1;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Updates the info panel with the currently selected achievement's data
    public void UpdateInfoPanel(AchievementInfo achievement)
    {
        //Change panel name and description
        infoPanelName.text = achievement.achievementName.text;
        infoPanelDescription.text = achievement.description.text;

        //If the achievement has a progress bar, show it
        if (achievement.data.showProgressBar)
        {
            int currentProgress = AchievementManager.GetData(achievement.data.requirementTrackingKey);
            progressBar.SetActive(true);
            
            //If the achievement is unlocked, show the progress at full
            if (achievement.data.isUnlocked())
            {
                progressBarFill.fillAmount = 1;
                progressNum.text = achievement.data.testValue + " / " + achievement.data.testValue;

            }

            //Otherwise, show the player's current progress towards the achievement
            else
            {
                progressBarFill.fillAmount = (float)currentProgress / (float)achievement.data.testValue;
                progressNum.text = currentProgress + " / " + achievement.data.testValue;
            }
        }

        //Disable the progress bar if it is not relevant to the selected achievement
        else
        {
            progressBar.SetActive(false);
        }

        //If the achievement is unlocked, show the lore text
        if (achievement.data.isUnlocked())
        {
            infoPanelLore.text = achievement.data.descriptionText;
            infoPanelLore.alignment = TextAlignmentOptions.Left;
        }

        //Otherwise, show a locked message
        else
        {
            infoPanelLore.text = "Unlock this achievement to read encrypted files";
            infoPanelLore.alignment = TextAlignmentOptions.Center;
        }

        //If the achievement only has 1 reward, disable the 2nd reward icon
        if (achievement.data.rewardSprites.Count == 1)
        {
            rewardThumbnails[1].gameObject.SetActive(false);
        }

        //Otherwise, show both reward icons              
        else
        {
            rewardThumbnails[1].gameObject.SetActive(true);
        }

        //Show the reward thumbnails for the achievement
        for(int i = 0; i < achievement.data.rewardSprites.Count; i++)
        {
            rewardThumbnails[i].sprite = achievement.data.rewardSprites[i];
        }
    }
    
    //Creates the achievements within the achievement screen UI
    public void CreateAchievements(List<AchievementData> achievements)
    {
        foreach (AchievementData achievement in achievements)
        {
            achievementItemPrefab.GetComponent<AchievementInfo>().data = achievement;

            Instantiate(achievementItemPrefab, achievementContent.transform);
        }
    }
}
