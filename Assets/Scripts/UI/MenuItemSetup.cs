using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace SubnauticaClone
{
    public class MenuItemSetup : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private Image icon;
        [SerializeField] private TextMeshProUGUI quantityText;
        [SerializeField] private Button interactButton;

        public Button InteractButton => interactButton;

        public void Setup(Sprite icon, int quantity = 1)
        {
            if (this.icon != null)
                this.icon.sprite = icon;

            if (quantityText != null)
                quantityText.text = (quantity > 1) ? quantity.ToString() : string.Empty;
        }
    }
}