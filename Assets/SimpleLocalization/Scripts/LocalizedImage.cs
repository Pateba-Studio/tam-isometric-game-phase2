using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Assets.SimpleLocalization.Scripts
{
    /// <summary>
    /// Localize text component.
    /// </summary>
    [RequireComponent(typeof(Image))]
    public class LocalizedImage : MonoBehaviour
    {
        public Sprite enSprite;
        public Sprite idSprite;

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
            switch (LocalizationManager.Language)
            {
                case "en-US":
                    GetComponent<Image>().sprite = enSprite;
                    break;
                case "id-ID":
                    GetComponent<Image>().sprite = idSprite;
                    break;
            }
        }
    }
}