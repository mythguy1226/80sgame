using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AchievementReward : MonoBehaviour
{
    //Reward type
    public string rewardType;

    //List of sprites to place on thumbnails
    public List<Sprite> rewardThumbnails;

    //Reward text header object
    public TextMeshProUGUI rewardText;

    //Mod Description Text Object
    public TextMeshProUGUI modDescription;

    //List of thumbnail image objects within the reward screen
    public List<Image> rewardThumbnailImages;

    //Next button to dismiss reward screen
    public GameObject nextButton;

    // Start is called before the first frame update
    void Start()
    {
        //Select the next button
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(nextButton);
        
        //Set text to the correct cosmetic reward type
        rewardText.text = "NEW " + rewardType + " UNLOCKED!";

        rewardThumbnailImages[1].color = Color.white;

        //Set the reward thumbnails for the image thumbnails
        if (rewardThumbnails.Count > 1)
        {
            rewardThumbnailImages[0].sprite = rewardThumbnails[0];
            rewardThumbnailImages[1].sprite = rewardThumbnails[1];
        }

        //If there is only one reward, hide the second thumbnail
        if (rewardThumbnails.Count == 1)
        {
            rewardThumbnailImages[0].sprite = rewardThumbnails[0];
            rewardThumbnailImages[1].gameObject.SetActive(false);
        }

        switch (rewardType)
        {
            case "OVERCHARGED":
                rewardText.text = rewardType + " UNLOCKED!";
                rewardThumbnailImages[1].gameObject.SetActive(true);
                rewardThumbnailImages[1].color = new Color(255,255,255,0);

                modDescription.gameObject.SetActive(true);
                modDescription.text = "Overcharged:\n\nBigger Shot Radius";
                break;
            case "ROGUE BAT":
                rewardText.text = rewardType + " UNLOCKED!";
                rewardThumbnailImages[1].gameObject.SetActive(true);
                rewardThumbnailImages[1].color = new Color(255,255,255,0);

                modDescription.gameObject.SetActive(true);
                modDescription.text = "Rogue Bat:\n\nSpawns A Friendly Bat That Stuns Other Bats";
                break;
            case "EMP":
                rewardText.text = rewardType + " UNLOCKED!";
                rewardThumbnailImages[1].gameObject.SetActive(true);
                rewardThumbnailImages[1].color = new Color(255,255,255,0);

                modDescription.gameObject.SetActive(true);
                modDescription.text = "EMP:\n\nStuns All Bats On Screen";
                break;
        }
    }

    //Destroy this reward screen and create a new one for the next reward obtained (if there are any)
    public void DismissReward()
    {
        GameManager.Instance.UIManager.titleScreenUI.CreateAchievementRewards();

        Destroy(gameObject);
    }
}
