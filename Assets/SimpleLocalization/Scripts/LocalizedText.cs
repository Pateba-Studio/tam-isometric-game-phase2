using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Assets.SimpleLocalization.Scripts
{
	/// <summary>
	/// Localize text component.
	/// </summary>
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class LocalizedText : MonoBehaviour
    {
        public string LocalizationKey;

        public void Start()
        {
            Localize();
            LocalizationManager.OnLocalizationChanged += Localize;
        }

        public void OnDestroy()
        {
            LocalizationManager.OnLocalizationChanged -= Localize;
        }

        private void Localize()
        {
            string temp = LocalizationManager.Localize(LocalizationKey);
            temp = temp.Replace("(username)", DataHandler.instance.GetUsername());
            GetComponent<TextMeshProUGUI>().text = temp;
        }
    }
}