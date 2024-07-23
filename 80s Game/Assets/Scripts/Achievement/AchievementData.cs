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
    public int testValue;

    public Sprite image;
    public bool isUnlocked()
    {
        return AchievementManager.HasBeenUnlocked(internalAchivementKey);
    }

}