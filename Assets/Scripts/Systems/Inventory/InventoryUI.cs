using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace SubnauticaClone
{
    public class InventoryUI : MonoBehaviour
    {
        [Header("UI")]
        public GameObject m_SlotPrefab;
        private GameObject m_Panel;
        private Transform m_GridParent;

        private void Awake()
        {
            m_Panel = GetComponentInChildren<CanvasGroup>(true)?.gameObject
                      ?? GetComponentInChildren<Image>(true)?.gameObject;

            var grid = GetComponentInChildren<GridLayoutGroup>(true);

            if (grid != null)
            {
                m_GridParent = grid.transform;
            }
            else
            {
                Debug.LogError("InventoryUI: Could not find GridLayoutGroup in children.");
            }

            if (m_Panel != null)
                m_Panel.SetActive(false);
        }

        private bool isOpen = false;

        private void Start()
        {
            if (m_Panel != null)
                m_Panel.SetActive(false);
        }

        public void Toggle()
        {
            isOpen = !isOpen;
            m_Panel.SetActive(isOpen);

            if (isOpen)
                Refresh();
        }

        private void Refresh()
        {
            var inv = Inventory.Instance;
            if (inv == null)
            {
                Debug.LogError("InventoryUI: Inventory.Instance is null.");
                return;
            }

            // Clear previous UI
            foreach (Transform child in m_GridParent)
                Destroy(child.gameObject);

            // Populate UI
            foreach (var item in inv.items)
            {
                var slot = Instantiate(m_SlotPrefab, m_GridParent);

                // Root slot image
                var icon = slot.GetComponent<Image>();

                // Quantity text
                var qtyText = slot.GetComponentInChildren<TextMeshProUGUI>();

                // Set icon
                if (item.itemData.icon != null)
                    icon.sprite = item.itemData.icon;

                // Set quantity (no text if 1)
                qtyText.text =
                    (item.quantity > 1) ? item.quantity.ToString() : string.Empty;
            }
        }
    }
}
