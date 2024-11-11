using UnityEngine;

public class LookingGlass : MonoBehaviour
{
    public GameObject[] modPrefabs;
    int achievementInteger = 0;

    /*
     Bat Controls
    */

    /// <summary>
    /// Tells the target manager to spawn a bat of the specific type
    /// </summary>
    /// <param name="type">The type of bat to spawn</param>
    public void SpawnBat(int type)
    {
        GameManager.Instance.TargetManager.SpawnTarget((TargetManager.TargetType)type);
    }


    /// <summary>
    /// Tells the active game mode to toggle whether a certain type of bat can spawn or not
    /// </summary>
    /// <param name="type"></param>
    public void ToggleCanSpawn(int type)
    {
        TargetManager.TargetType toggleType = (TargetManager.TargetType)type;
        GameManager.Instance.ActiveGameMode.ToggleAllowedBatType(toggleType);
    }

    /*
    Mod Controls 
    */


    /// <summary>
    /// Toggles the availability of a particular modifier
    /// </summary>
    /// <param name="type">The type, as integer, of the modifier to toggle</param>
    public void ToggleModAvailable(int type)
    {
        AbsModifierEffect.ModType toggleType = (AbsModifierEffect.ModType)type;
        if (AbsModifierEffect.ModTypeIsBuff(toggleType))
        {
            if (!GameManager.Instance.buffs.Contains(modPrefabs[type]))
            {
                GameManager.Instance.buffs.Add(modPrefabs[type]);
            }
            
        } else
        {
            if (!GameManager.Instance.debuffs.Contains(modPrefabs[type]))
            {
                GameManager.Instance.buffs.Add(modPrefabs[type]);
            }
        }
        GameManager.Instance.ActiveGameMode.ToggleAllowedModType(toggleType);
        
    }

    public void ResetAchievements()
    {
        AchievementManager.SetAchievementStatus(0);
    }

    public void UnlockAllAchievements()
    {
        AchievementManager.SetAchievementStatus(1);
    }

    public void UnlockSpecificAchievement()
    {
        string achievementKey = AchievementConstants.MapEnumToKey((AchievementConstants.Achievements)achievementInteger);
        AchievementManager.UnlockAchievement(achievementKey);
    }

    public void SetAchievementInteger(int newValue)
    {
        achievementInteger = newValue;
    }

    public void DoubleAchievementTest()
    {
        AchievementManager.UnlockAchievement(AchievementConstants.CLASSIC_FAN);
        AchievementManager.UnlockAchievement(AchievementConstants.CLASSIC_ENJOYER);
    }

    public void LockFrameRate30()
    {
        Application.targetFrameRate = 30;
    }

    public void LockFrameRate60()
    {
        Application.targetFrameRate = 60;
    }
}