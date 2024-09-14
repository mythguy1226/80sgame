
using Steamworks;
using System.Collections.Generic;

namespace Localization
{
    public enum SupportedLanguages
    {
        ENGLISH,
        SPANISH,
        FRENCH
    }

    public enum TextDomain
    {
        UI,
        Achievements,
        Credits,
        GameText
    }

    public enum UITextLabels
    {
        StartGame,
        Achievements,
        Settings,
        Exit,
        Credits,
        Controls,
        SFX_Volume,
        Music_Volume,
        Sensitivity,
        Bloom,
        CRT_Effect,
        Curvature,
        Apply,
        Cancel,
        Fire,
        Aim,
        ExitQ,
        SaveAndExit,
        ExitWithoutSaving,
        MnKb,
        SettingsPrompt,
        Pause
    }

    public static class LocalizationManager
    {

        private static Dictionary<UITextLabels, string> activeUITextLabelsMap;
        private static bool b_ready = false;
        public static bool IsReady { get { return b_ready; } set { b_ready = value; } }

        public static void SetUpLanguageIndex(TextDomain domain, TextElements elements)
        {
            switch (domain)
            {
                default:
                    activeUITextLabelsMap = new Dictionary<UITextLabels, string>();
                    Dictionary<string, UITextLabels> encoder = SetupUILabelIndex(elements);
                    for (int i = 0; i < elements.textElements.Length; i++)
                    {
                        activeUITextLabelsMap.Add(encoder[elements.textElements[i].label], elements.textElements[i].value);
                    }
                    
                    break;
            }
        }


        // These strings are set by Steam. See https://partner.steamgames.com/doc/store/localization/languages
        public static SupportedLanguages GetLangEnum(string name)
        {
            switch (name)
            {
                case "french":
                    return SupportedLanguages.FRENCH;
                case "latam":
                    return SupportedLanguages.SPANISH;
                default:
                    return SupportedLanguages.ENGLISH;
            }
        }

        public static string GetLabelValue(UITextLabels label)
        {
            return activeUITextLabelsMap[label];
        }

        private static Dictionary<string, UITextLabels> SetupUILabelIndex(TextElements elements)
        {
            Dictionary<string, UITextLabels> UIEncoder = new Dictionary<string, UITextLabels>();
            for (int i = 0; i < elements.textElements.Length; i++)
            {
                LanguageDependentTextData data = elements.textElements[i];
                switch (data.label)
                {
                    case "strt":
                        UIEncoder.Add(data.label, UITextLabels.StartGame);
                        break;
                    case "achv":
                        UIEncoder.Add(data.label, UITextLabels.Achievements);
                        break;
                    case "stng":
                        UIEncoder.Add(data.label, UITextLabels.Settings);
                        break;
                    case "exit":
                        UIEncoder.Add(data.label, UITextLabels.Exit);
                        break;
                    case "crdt":
                        UIEncoder.Add(data.label, UITextLabels.Credits);
                        break;
                    case "ctrl":
                        UIEncoder.Add(data.label, UITextLabels.Controls);
                        break;
                    case "sfxv":
                        UIEncoder.Add(data.label, UITextLabels.SFX_Volume);
                        break;
                    case "mscv":
                        UIEncoder.Add(data.label, UITextLabels.Music_Volume);
                        break;
                    case "aply":
                        UIEncoder.Add(data.label, UITextLabels.Apply);
                        break;
                    case "cncl":
                        UIEncoder.Add(data.label, UITextLabels.Cancel);
                        break;
                    case "crvt":
                        UIEncoder.Add(data.label, UITextLabels.Curvature);
                        break;
                    case "crte":
                        UIEncoder.Add(data.label, UITextLabels.CRT_Effect);
                        break;
                    case "blum":
                        UIEncoder.Add(data.label, UITextLabels.Bloom);
                        break;
                    case "aim":
                        UIEncoder.Add(data.label, UITextLabels.Aim);
                        break;
                    case "fire":
                        UIEncoder.Add(data.label, UITextLabels.Fire);
                        break;
                    case "mnkb":
                        UIEncoder.Add(data.label, UITextLabels.MnKb);
                        break;
                    case "stnp":
                        UIEncoder.Add(data.label, UITextLabels.SettingsPrompt);
                        break;
                    case "svex":
                        UIEncoder.Add(data.label, UITextLabels.SaveAndExit);
                        break;
                    case "exws":
                        UIEncoder.Add(data.label, UITextLabels.ExitWithoutSaving);
                        break;
                    case "extq":
                        UIEncoder.Add(data.label, UITextLabels.ExitQ);
                        break;
                    case "paus":
                        UIEncoder.Add(data.label, UITextLabels.Pause);
                        break;

                }
            }
            return UIEncoder;
        }

        



    }
}
