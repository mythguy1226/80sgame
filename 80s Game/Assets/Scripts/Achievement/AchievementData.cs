using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="ScriptableObjects/Achievement")]
public class AchievementData : ScriptableObject
{
    public enum TestType
    {
        EqualTo,
        GreaterThan,
        LessThan,
        GreaterThanOrEqual,
        LessThanOrEqual
    }
    public int order;
    public string nameText;
    
    [TextArea]
    public string requirementText;
    
    
    [TextArea]
    public string descriptionText;


    public string internalAchivementKey;
    public string requirementTrackingKey;
    public int testValue;
    public bool platRelevant;

    public Sprite image;

    public string rewardText;
    public List<Sprite> rewardSprites;

    public bool showProgressBar;
    
    //Defer this test to the AchievementManager, just in case
    public bool isUnlocked()
    {
        return AchievementManager.HasBeenUnlocked(internalAchivementKey);
    }

}