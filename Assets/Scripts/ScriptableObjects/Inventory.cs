using System;
using System.Collections.Generic;
using UnityEngine;

namespace SubnauticaClone
{
    /// <summary>
    /// Represents an item in the inventory.
    /// </summary>
    [System.Serializable]
    public class InventoryItem
    {
        public ItemData itemData;
        public int quantity;

        public InventoryItem(ItemData data, int qty)
        {
            itemData = data;
            quantity = qty;
        }

        public bool IsEmpty()
        {
            return itemData == null;
        }

        public void UpdateItem(ItemData data, int qty)
        {
            itemData = data;
            quantity = qty;
        }
    }

    [CreateAssetMenu(fileName = "New Inventory", menuName = "PS/Inventory/System")]
    public class Inventory : ScriptableObject
    {
        public int capacity = 20;
        public List<InventoryItem> items = new();

        private void OnEnable()
        {
            items.Clear();
        }

        public bool AddItem(ItemData item, int quantity)
        {
            if (items.Count < capacity)
            {
                foreach (var itemInInventory in items)
                {
                    if (itemInInventory.itemData == item)
                    {
                        itemInInventory.quantity += quantity;
                        return true;
                    }
                }

                items.Add(new InventoryItem(item, quantity));
                return true;
            }

            Debug.LogWarning("Inventory is full! Cannot add " + item.itemName);
            return false;
        }

        public bool HasItem(ItemData item, int quantity)
        {
            foreach (var itemInInventory in items)
            {
                if (itemInInventory.itemData == item && itemInInventory.quantity >= quantity)
                    return true;
            }

            return false;
        }

        public void RemoveItem(ItemData item, int quantity)
        {
            InventoryItem itemToRemove = null;
            foreach (var invItem in items)
            {
                if (invItem.itemData == item)
                {
                    invItem.quantity -= quantity;
                    if (invItem.quantity <= 0)
                    {
                        itemToRemove = invItem;
                    }
                    break;
                }
            }
            if (itemToRemove != null) items.Remove(itemToRemove);
        }
    }
}

