using UnityEngine;

namespace SubnauticaClone
{
    /// <summary>
    /// Handles picking up items from the game world and adding them to the player's inventory.
    /// </summary>
    public class PickupItem : MonoBehaviour, IInteractable
    {
        public ItemData itemData;
        public int quantity = 1;

        public void Interact(GameObject interactor)
        {
            InventoryManager inventoryManager = interactor.GetComponent<InventoryManager>();
            if (inventoryManager != null)
            {
                if (inventoryManager.AddItem(itemData, quantity))
                {
                    Destroy(gameObject);
                }
            }
        }
    }
}