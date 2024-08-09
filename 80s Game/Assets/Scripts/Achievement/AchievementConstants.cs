//This class holds the internal reference for all achievement keys
//Any achievement Scriptable Object created must have a key in this registry otherwise
//it will never unlock
public static class AchievementConstants
{
    public enum Achievements
    {
        CLASSIC_FAN = 0,
        CLASSIC_ENJOYER,
        TIMELESS_CLASSIC,
        PROMPT_PROTECTOR,
        STAUNCH_DEFENDER,
        BULWARK_OF_RESISTANCE,
        MK1_NOVICE,
        MK1_ADEPT,
        MK1_EXPERT,
        UNSTABLE_NOVICE,
        UNSTABLE_ADEPT,
        UNSTABLE_EXPERT,
        BONUS_NOVICE,
        BONUS_ADEPT,
        BONUS_EXPERT,
        MOD_BAT_NOVICE,
        MOD_BAT_ADEPT,
        MOD_BAT_EXPERT,
        UNFAZED,
        KITTED_OUT,
        FULLY_CHARGED,
        BOMB_VOYAGE,
        GREASED_LIGHTNING,
        WELL_THATS_AWKWARD,
        BULLSEYE,
        CAREFUL_FRAGILE,
        PROUD_OF_YOU,
        MARKSMAN,
        SHARPSHOOTER,
        HAWKEYE,
        EMPLOYEE_OF_THE_MONTH
    }

    public static string CLASSIC_FAN = "classic-1";
    public static string CLASSIC_ENJOYER = "classic-2";
    public static string TIMELESS_CLASSIC = "classic-3";
    public static string PROMPT_PROTECTOR = "def-1";
    public static string STAUNCH_DEFENDER = "def-2";
    public static string BULWARK_OF_RESISTANCE = "def-3";
    public static string MK1_NOVICE = "reg-1";
    public static string MK1_ADEPT = "reg-2";
    public static string MK1_EXPERT = "reg-3";
    public static string UNSTABLE_NOVICE = "unst-1";
    public static string UNSTABLE_ADEPT = "unst-2";
    public static string UNSTABLE_EXPERT = "unst-3";
    public static string BONUS_NOVICE = "bb-1";
    public static string BONUS_ADEPT = "bb-2";
    public static string BONUS_EXPERT = "bb-3";
    public static string MOD_BAT_NOVICE = "mod-1";
    public static string MOD_BAT_ADEPT = "mod-2";
    public static string MOD_BAT_EXPERT = "mod-3";
    public static string UNFAZED = "unfazed";
    public static string KITTED_OUT = "kitted";
    public static string FULLY_CHARGED = "charged";
    public static string BOMB_VOYAGE = "voyage";
    public static string GREASED_LIGHTNING = "grease";
    public static string WELL_THATS_AWKWARD = "awkward";
    public static string BULLSEYE = "bulls";
    public static string CAREFUL_FRAGILE = "fragile";
    public static string PROUD_OF_YOU = "good-1";
    public static string MARKSMAN = "acc-1";
    public static string SHARPSHOOTER = "acc-2";
    public static string HAWKEYE = "acc-3";
    public static string EMPLOYEE_OF_THE_MONTH = "plat";

    public static string MapEnumToKey(Achievements name)
    {
        switch (name)
        {
            case Achievements.CLASSIC_FAN:
                return CLASSIC_FAN;
            case Achievements.CLASSIC_ENJOYER:
                return CLASSIC_ENJOYER;
            case Achievements.TIMELESS_CLASSIC:
                return TIMELESS_CLASSIC;
            case Achievements.PROMPT_PROTECTOR:
                return PROMPT_PROTECTOR;
            case Achievements.STAUNCH_DEFENDER:
                return STAUNCH_DEFENDER;
            case Achievements.BULWARK_OF_RESISTANCE:
                return BULWARK_OF_RESISTANCE;
            case Achievements.MK1_NOVICE:
                return MK1_NOVICE;
            case Achievements.MK1_ADEPT:
                return MK1_ADEPT;
            case Achievements.MK1_EXPERT:
                return MK1_EXPERT;
            case Achievements.UNSTABLE_NOVICE:
                return UNSTABLE_NOVICE;
            case Achievements.UNSTABLE_ADEPT:
                return UNSTABLE_ADEPT;
            case Achievements.UNSTABLE_EXPERT:
                return UNSTABLE_EXPERT;
            case Achievements.BONUS_NOVICE:
                return BONUS_NOVICE;
            case Achievements.BONUS_ADEPT:
                return BONUS_ADEPT;
            case Achievements.BONUS_EXPERT:
                return BONUS_EXPERT;
            case Achievements.MOD_BAT_NOVICE:
                return MOD_BAT_NOVICE;
            case Achievements.MOD_BAT_ADEPT:
                return MOD_BAT_ADEPT;
            case Achievements.MOD_BAT_EXPERT:
                return MOD_BAT_EXPERT;
            case Achievements.UNFAZED:
                return UNFAZED;
            case Achievements.KITTED_OUT:
                return KITTED_OUT;
            case Achievements.FULLY_CHARGED:
                return FULLY_CHARGED;
            case Achievements.BOMB_VOYAGE:
                return BOMB_VOYAGE;
            case Achievements.GREASED_LIGHTNING:
                return GREASED_LIGHTNING;
            case Achievements.WELL_THATS_AWKWARD:
                return WELL_THATS_AWKWARD;
            case Achievements.BULLSEYE:
                return BULLSEYE;
            case Achievements.CAREFUL_FRAGILE:
                return CAREFUL_FRAGILE;
            case Achievements.PROUD_OF_YOU:
                return PROUD_OF_YOU;
            case Achievements.MARKSMAN:
                return MARKSMAN;
            case Achievements.SHARPSHOOTER:
                return SHARPSHOOTER;
            case Achievements.HAWKEYE:
                return HAWKEYE;
            default:
                return EMPLOYEE_OF_THE_MONTH;
        }
    }
}