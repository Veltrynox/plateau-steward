using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace SubnauticaClone
{
    /// <summary>
    /// Manages the inventory UI, displaying items and quantities.
    /// </summary>
    public class InventoryUI : MonoBehaviour
    {
        [Header("UI")]
        public GameObject slotPrefab;
        public GameObject panel;
        public Inventory inventory;

        private Transform m_GridParent;

        private void Awake()
        {
            var grid = panel.GetComponentInChildren<GridLayoutGroup>();

            if (grid != null)
            {
                m_GridParent = grid.transform;
            }
            else
            {
                Debug.LogError("InventoryUI: Could not find GridLayoutGroup in children.");
            }

            if (inventory == null) return;

            if (panel != null)
                panel.SetActive(false);
        }

        private bool isOpen = false;

        public void Toggle()
        {
            isOpen = !isOpen;
            panel.SetActive(isOpen);

            if (isOpen)
                Refresh();
        }

        public void Refresh()
        {
            // Clear previous UI
            foreach (Transform child in m_GridParent)
                Destroy(child.gameObject);

            // Populate UI
            foreach (var item in inventory.items)
            {
                var slot = Instantiate(slotPrefab, m_GridParent);

                var icon = slot.GetComponent<Image>();

                var qtyText = slot.GetComponentInChildren<TextMeshProUGUI>();

                if (item.itemData.icon != null)
                    icon.sprite = item.itemData.icon;

                qtyText.text =
                    (item.quantity > 1) ? item.quantity.ToString() : string.Empty;
            }
        }
    }
}
