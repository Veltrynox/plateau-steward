using System.Collections.Generic;
using UnityEngine;

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
}

namespace SubnauticaClone
{
    public class Inventory : SingletonBase<Inventory>
    {
        public int capacity = 20;
        public List<InventoryItem> items = new List<InventoryItem>();

        protected override void Awake()
        {
            base.Awake();
            if (Instance != this) return;
 
            if (items.Capacity < capacity)
                items.Capacity = capacity;
        }

        public bool AddItem(ItemData item, int quantity)
        {
            InventoryItem existingItem = items.Find(i =>
                i.itemData == item &&
                i.itemData.maxStack > 1
            );

            if (existingItem != null)
            {
                existingItem.quantity += quantity;
                return true;
            }

            if (items.Count >= capacity)
            {
                Debug.Log("Inventory full!");
                return false;
            }

            items.Add(new InventoryItem(item, quantity));
            return true;
        }

        public void RemoveItem(ItemData item, int quantity)
        {
            InventoryItem existingItem = items.Find(i => i.itemData == item);
            if (existingItem != null)
            {
                existingItem.quantity -= quantity;
                if (existingItem.quantity <= 0)
                    items.Remove(existingItem);
            }
        }
    }
}
