using UnityEngine;
using TMPro;
namespace Localization
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class LangUIText : MonoBehaviour
    {
        public UITextLabels label;
        public bool transformToUpper;

        private void Update()
        {
            if (LocalizationManager.IsReady)
            {
                TextMeshProUGUI textObject = GetComponent<TextMeshProUGUI>();
                textObject.text = LocalizationManager.GetLabelValue(label);
                if (transformToUpper) { 
                    textObject.text = textObject.text.ToUpper();
                }

                // Having set up the text, it's time to turn off this update request and free up CPU
                this.enabled = false;
            }
        }
    }
}
