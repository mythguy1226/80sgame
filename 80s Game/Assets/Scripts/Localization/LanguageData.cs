namespace Localization
{
    [System.Serializable]
    public class LanguageDependentTextData
    {
        public string label;
        public string value;
    }

    [System.Serializable]
    public class TextElements
    {
        public LanguageDependentTextData[] textElements;
    }
}
