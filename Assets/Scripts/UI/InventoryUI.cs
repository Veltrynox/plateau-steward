using UnityEngine;
using UnityEngine.UI;

namespace SubnauticaClone
{
    public class InventoryUI : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private GameObject menuItemPrefab;
        [SerializeField] private GameObject panel;
        [SerializeField] private InventoryMenu inventoryMenuUI;
        [SerializeField] private Inventory inventoryScriptableObject;

        private Transform m_gridParent;

        private void Awake()
        {
            var grid = panel.GetComponentInChildren<GridLayoutGroup>();
            if (grid != null)
            {
                m_gridParent = grid.transform;
            }
            else
            {
                Debug.LogError("InventoryUI: Could not find GridLayoutGroup in children.");
            }

            if (panel != null) panel.SetActive(false);
        }

        private bool isOpen = false;

        public void Toggle()
        {
            isOpen = !isOpen;
            panel.SetActive(isOpen);
            if (isOpen) Refresh();
        }

        public void Refresh()
        {
            foreach (Transform child in m_gridParent)
                Destroy(child.gameObject);

            foreach (var item in inventoryScriptableObject.items)
            {
                var slot = Instantiate(menuItemPrefab, m_gridParent);
                var inventorySlot = slot.GetComponent<MenuItemSetup>();

                inventorySlot.Setup(item.itemData.icon, item.quantity);

                inventorySlot.InteractButton.onClick.AddListener(() =>
                    inventoryMenuUI.Open(item.itemData.itemName, () => DropItem(item))
                );
            }
        }

        private void DropItem(InventoryItem item)
        {
            inventoryScriptableObject.RemoveItem(item.itemData, 1);
            SpawnItemInWorld(item.itemData);
            Refresh();
        }

        private void SpawnItemInWorld(ItemData data)
        {
            var player = LevelBuilder.Instance.Player;
            if (player != null && data.itemPrefab != null)
            {
                Vector3 dropPos = player.transform.position + (player.transform.forward * 1.5f) + Vector3.up;
                Instantiate(data.itemPrefab, dropPos, Quaternion.identity);
            }
            else
            {
                Debug.LogWarning($"Cannot drop {data.itemName}: Missing Player reference or Item Prefab.");
            }
        }
    }
}