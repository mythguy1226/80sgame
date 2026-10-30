using UnityEngine;


/// <summary>
/// Scriptable Object that details the information for a particular level
/// READ ONLY.
/// </summary>
[CreateAssetMenu(menuName = "ScriptableObjects/Campaign/Level")]
public class LevelDataSO : ScriptableObject
{
    [SerializeField]
    int rounds;

    [SerializeField]
    LevelType levelType;

    [SerializeField]
    float metricThresholdOneStar;

    [SerializeField]
    float metricThresholdTwoStars;

    [SerializeField]
    float metricThresholdThreeStars;

    [SerializeField]
    Sprite levelBackground;

    [SerializeField]
    ObjectiveType levelObjective;

    [SerializeField]
    AllowedSpawns[] allowedSpawnsPerRound;

    public LevelType GetLevelType()
    {
        return levelType;
    }

    public Sprite GetLevelBackground()
    {
        return levelBackground;
    }

    public AbsGameObjective GetLevelObjective()
    {
        AbsGameObjective objective = ObjectiveFactory.MakeObjective(levelObjective);
        objective.SetThresholds(metricThresholdOneStar,metricThresholdTwoStars,metricThresholdThreeStars);
        return objective;
    }


    /// <summary>
    /// Returns the allowed spawn types for a given round. Returns the last element if the index is out of bounds.
    /// </summary>
    /// <param name="index">The current active round</param>
    /// <returns>The allowed target types that can spawn in a given round</returns>
    public AllowedSpawns GetAllowedSpawns(int index)
    {
        if(index < allowedSpawnsPerRound.Length-1 && index >= 0)
        {
            return allowedSpawnsPerRound[index];
        }
        return allowedSpawnsPerRound[allowedSpawnsPerRound.Length - 1];
    }

}

