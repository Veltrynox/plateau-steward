using UnityEngine;
using UnityEngine.InputSystem;

namespace SubnauticaClone
{
    /// <summary>
    /// Manages the player's inventory, including adding items and toggling the inventory UI.
    /// </summary>
    public class InventoryManager : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Inventory inventory;
        private PlayerInput playerInput;
        private GameObject m_menuGUI;

        [Header("Settings")]
        [SerializeField] private string gameplayMapName = "Player";
        [SerializeField] private string uiMapName = "UI";

        private bool isPaused = false;

        private void Start()
        {
            CloseInventory();
            m_menuGUI = LevelBuilder.Instance.LevelGUI;
            playerInput = GetComponent<PlayerInput>();
        }

        private void OnInventory(InputValue value)
        {
            if (m_menuGUI == null)
                return;
            
            isPaused = !isPaused;


            var inventory = m_menuGUI.GetComponentInChildren<InventoryUI>(true);
            if (inventory != null)
                inventory.Toggle();

            var crafting = m_menuGUI.GetComponentInChildren<CraftingUI>(true);
            if (crafting != null)
                crafting.Toggle();

            if (isPaused)
            {
                Time.timeScale = 0f;
                playerInput.SwitchCurrentActionMap(uiMapName);
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                Time.timeScale = 1f;
                playerInput.SwitchCurrentActionMap(gameplayMapName);
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }

        internal bool AddItem(ItemData itemData, int quantity)
        {
            if (inventory.AddItem(itemData, quantity))
            {
                return true;
            }
            return false;
        }

        private void CloseInventory()
        {
            isPaused = false;
            Time.timeScale = 1f;
        }
    }
}