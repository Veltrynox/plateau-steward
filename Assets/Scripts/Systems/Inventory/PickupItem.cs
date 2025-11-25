using UnityEngine;

namespace SubnauticaClone
{
    public class PickupItem : MonoBehaviour, IInteractable
    {
        public ItemData itemData;
        public int quantity = 1;

        public void Interact(GameObject interactor)
        {
            Inventory inventory = interactor.GetComponent<Inventory>();
            if (inventory != null)
            {
                bool pickedUp = inventory.AddItem(itemData, quantity);
                if (pickedUp)
                    Destroy(gameObject);
            }
        }
    }
}