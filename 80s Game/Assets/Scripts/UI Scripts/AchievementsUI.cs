using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AchievementsUI : MonoBehaviour
{
    public Scrollbar achievementScrollbar;
    public TextMeshProUGUI infoPanelName;
    public TextMeshProUGUI infoPanelDescription;

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
        infoPanelName.text = achievement.title.text;
        infoPanelDescription.text = achievement.description.text;
    }
}
