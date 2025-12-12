using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace SubnauticaClone
{
    public class MenuItemSetup : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private Image m_icon;
        [SerializeField] private TextMeshProUGUI m_quantityText;
        [SerializeField] private Button m_interactButton;

        public Button InteractButton => m_interactButton;

        public void Setup(Sprite icon, int quantity = 1)
        {
            if (m_icon != null)
                m_icon.sprite = icon;

            if (m_quantityText != null)
                m_quantityText.text = (quantity > 1) ? quantity.ToString() : string.Empty;
        }
    }
}