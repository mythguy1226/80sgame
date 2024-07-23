using System.Collections.Generic;
using UnityEngine;
using static AchievementData;

public static class AchievementManager
{
    public static Dictionary<string, int> requirements;

    public static void RegisterRequirements(List<AchievementData> achievements)
    {
        requirements = new Dictionary<string, int>();
        for (int i = 0; i < achievements.Count; i++)
        {
            AchievementData achievement = achievements[i];
            requirements[achievement.internalAchivementKey] = achievement.testValue;
        }
    }

    public static bool HasBeenUnlocked(string name)
    {
        int value = PlayerPrefs.GetInt(name, 0);
        return value == 1;
    }

    public static void RegisterData(string name, int value)
    {
        PlayerPrefs.SetInt(name, value);
    }

    public static int GetData(string name) { 
    
        return PlayerPrefs.GetInt(name, 0);
    }

    public static void UnlockAchievement(string name)
    {
        Debug.Log("Unlocking Achievement " + name);
        PlayerPrefs.SetInt(name , 1);
        TestForPlat();
    }

    private static void TestForPlat()
    {
        bool plat = true;
        foreach (KeyValuePair<string, int> kvp in requirements)
        {
            if (GetData(kvp.Key) == 0)
            {
                plat = false;
                break;
            }
        }

        if (plat) {
            UnlockAchievement(AchievementConstants.EMPLOYEE_OF_THE_MONTH);
        }
    }

    private static bool TestUnlock(TestType testType, int expectedValue, int testValue)
    {
        bool testSuccessful = false;
        switch (testType)
        {
            case TestType.GreaterThan:
                testSuccessful = expectedValue < testValue;
                break;
            case TestType.LessThan:
                testSuccessful = expectedValue > testValue;
                break;
            case TestType.LessThanOrEqual:
                testSuccessful = expectedValue >= testValue;
                break;
            case TestType.GreaterThanOrEqual:
                testSuccessful = expectedValue <= testValue;
                break;
            default:
                testSuccessful = expectedValue == testValue;
                break;
        }

        return testSuccessful;
    }

    public static bool TestUnlock(TestType testType, float expectedValue, float testValue)
    {
        bool testSuccessful = false;
        switch (testType)
        {
            case TestType.GreaterThan:
                testSuccessful = expectedValue > testValue;
                break;
            case TestType.LessThan:
                testSuccessful = expectedValue < testValue;
                break;
            case TestType.LessThanOrEqual:
                testSuccessful = expectedValue <= testValue;
                break;
            case TestType.GreaterThanOrEqual:
                testSuccessful = expectedValue >= testValue;
                break;
            default:
                testSuccessful = expectedValue == testValue;
                break;
        }

        return testSuccessful;
    }

    public static void TestEndGameAchievements(EGameMode gameMode, int currentRound, int score)
    {
        TestType greaterTest = TestType.GreaterThanOrEqual;
        string key;
        switch (gameMode)
        {
            case EGameMode.Classic:
                key = AchievementConstants.TIMELESS_CLASSIC;
                TestAndUnlock(key, requirements[key], score, greaterTest);
                key = AchievementConstants.CLASSIC_ENJOYER;
                TestAndUnlock(key, requirements[key], score, greaterTest);
                key = AchievementConstants.CLASSIC_FAN;
                TestAndUnlock(key, requirements[key], score, greaterTest);
                key = AchievementConstants.PROUD_OF_YOU;
                TestAndUnlock(key, requirements[key], 0, TestType.EqualTo);
                break;
            case EGameMode.Competitive:
                key = AchievementConstants.PROUD_OF_YOU;
                TestAndUnlock(key, requirements[key], 0, TestType.EqualTo);
                break;
            case EGameMode.Defense:
                key = AchievementConstants.BULWARK_OF_RESISTANCE;
                TestAndUnlock(key, requirements[key], currentRound, greaterTest);
                key = AchievementConstants.STAUNCH_DEFENDER;
                TestAndUnlock(key, requirements[key], currentRound, greaterTest);
                key = AchievementConstants.PROMPT_PROTECTOR;
                TestAndUnlock(key, requirements[key], currentRound, greaterTest);
                break;
        }
    }

    public static void HandleStunAchievements(TargetManager.TargetType type)
    {
        string dataKey = "stun-" + type.ToString().ToLower();
        int stunnedBatsOfThisType = GetData(dataKey);
        HandleAccuracyAchievements();
        // Test for the highest thing first. This can bypass execution of all other checks
        switch (type)
        {
            case TargetManager.TargetType.Unstable:
                if (HasBeenUnlocked(AchievementConstants.MOD_BAT_EXPERT))
                {
                    return;
                }
                stunnedBatsOfThisType++;
                break;
            case TargetManager.TargetType.Modifier:
                if (HasBeenUnlocked(AchievementConstants.MOD_BAT_EXPERT))
                {
                    return;
                }
                stunnedBatsOfThisType++;
                break;
            case TargetManager.TargetType.LowBonus:
            case TargetManager.TargetType.HighBonus:
                if (HasBeenUnlocked(AchievementConstants.BONUS_EXPERT))
                {
                    return;
                }
                int lowBonusCount = GetData("stun-lowbonus");
                int highBonusCount = GetData("stun-highbonus");
                stunnedBatsOfThisType = lowBonusCount + highBonusCount + 1;
                break;
            case TargetManager.TargetType.Regular:
                if (HasBeenUnlocked(AchievementConstants.MK1_EXPERT))
                {
                    return;
                }
                stunnedBatsOfThisType++;
                break;
        }
        TestUnlockByType(type, dataKey, stunnedBatsOfThisType);
    }

    private static void TestUnlockByType(TargetManager.TargetType type, string dataKey, int stunnedBatsOfThisType)
    {
        string key;
        switch (type)
        {
            case TargetManager.TargetType.Unstable:
                key = AchievementConstants.UNSTABLE_EXPERT;
                TestAndUnlock(key, requirements[key], stunnedBatsOfThisType, TestType.GreaterThanOrEqual);
                key = AchievementConstants.UNSTABLE_ADEPT;
                TestAndUnlock(key, requirements[key], stunnedBatsOfThisType, TestType.GreaterThanOrEqual);
                key = AchievementConstants.UNSTABLE_NOVICE;
                TestAndUnlock(key, requirements[key], stunnedBatsOfThisType, TestType.GreaterThanOrEqual);
                break;
            case TargetManager.TargetType.Modifier:
                key = AchievementConstants.MOD_BAT_EXPERT;
                TestAndUnlock(key, requirements[key], stunnedBatsOfThisType, TestType.GreaterThanOrEqual);
                key = AchievementConstants.MOD_BAT_ADEPT;
                TestAndUnlock(key, requirements[key], stunnedBatsOfThisType, TestType.GreaterThanOrEqual);
                key = AchievementConstants.MOD_BAT_NOVICE;
                TestAndUnlock(key, requirements[key], stunnedBatsOfThisType, TestType.GreaterThanOrEqual);
                break;
            case TargetManager.TargetType.LowBonus:
            case TargetManager.TargetType.HighBonus:
                key = AchievementConstants.BONUS_EXPERT;
                TestAndUnlock(key, requirements[key], stunnedBatsOfThisType, TestType.GreaterThanOrEqual);
                key = AchievementConstants.BONUS_ADEPT;
                TestAndUnlock(key, requirements[key], stunnedBatsOfThisType, TestType.GreaterThanOrEqual);
                key = AchievementConstants.BONUS_NOVICE;
                TestAndUnlock(key, requirements[key], stunnedBatsOfThisType, TestType.GreaterThanOrEqual);
                break;
            case TargetManager.TargetType.Regular:
                key = AchievementConstants.MK1_EXPERT;
                TestAndUnlock(key, requirements[key], stunnedBatsOfThisType, TestType.GreaterThanOrEqual);
                key = AchievementConstants.MK1_ADEPT;
                TestAndUnlock(key, requirements[key], stunnedBatsOfThisType, TestType.GreaterThanOrEqual);
                key = AchievementConstants.MK1_NOVICE;
                TestAndUnlock(key, requirements[key], stunnedBatsOfThisType, TestType.GreaterThanOrEqual);
                break;
        }
        RegisterData(dataKey, stunnedBatsOfThisType);
    }

    private static void TestAndUnlock(string key, int expected, int actual, TestType testType)
    {
        if (HasBeenUnlocked(key))
        {
            return;
        }
        if (TestUnlock(testType, expected, actual))
        {
            UnlockAchievement(key);
        }
    }

    private static void HandleAccuracyAchievements()
    {
        int currentKillCount = GetData("killCount");
        currentKillCount++;
        string key = AchievementConstants.HAWKEYE;
        TestAndUnlock(key, requirements[key], currentKillCount, TestType.GreaterThanOrEqual);
        key = AchievementConstants.SHARPSHOOTER;
        TestAndUnlock(key, requirements[key], currentKillCount, TestType.GreaterThanOrEqual);
        key = AchievementConstants.MARKSMAN;
        TestAndUnlock(key, requirements[key], currentKillCount, TestType.GreaterThanOrEqual);
    }

    public static void UnfazedTest(int stunningPlayerOrder)
    {
        int confusionCount = GetData("confmod-pi-" + stunningPlayerOrder.ToString());
        string key = AchievementConstants.UNFAZED;
        if (TestUnlock(TestType.GreaterThanOrEqual, requirements[key], confusionCount+1))
        {
            UnlockAchievement(key);
            return;
        }
        RegisterData("confmod-pi-" + stunningPlayerOrder.ToString(), confusionCount + 1);
    }

    public static void BullseyeTest(PlayerController player)
    {
        
    }
}