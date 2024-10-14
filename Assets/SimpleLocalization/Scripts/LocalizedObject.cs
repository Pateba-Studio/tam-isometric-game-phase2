using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Assets.SimpleLocalization.Scripts
{
	/// <summary>
	/// Localize text component.
	/// </summary>
    public class LocalizedObject : MonoBehaviour
    {
        public GameObject objectId;
        public GameObject objectEn;

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
            objectId.SetActive(false);
            objectEn.SetActive(false);

            switch (LocalizationManager.Language)
            {
                case "en-US":
                    objectEn.SetActive(true);
                    break;
                case "id-ID":
                    objectId.SetActive(true);
                    break;
            }
        }
    }
}