using UnityEngine;

/// <summary>
/// ScriptableObject representing a generic collectable item in the game.
/// </summary>
[CreateAssetMenu(fileName = "New Item", menuName = "PS/Inventory/Item")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public Sprite icon;
    public int maxStack = 1;
    public GameObject itemPrefab;

    public Sprite Icon => icon;
    
}