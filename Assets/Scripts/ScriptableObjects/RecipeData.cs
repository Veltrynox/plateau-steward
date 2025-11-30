using UnityEngine;
using System.Collections.Generic;

namespace SubnauticaClone
{
    /// <summary>
    /// Represents an ingredient required for a recipe.
    /// </summary>
    [System.Serializable]
    public struct Ingredient
    {
        public ItemData item;
        public int amount;
    }

    /// <summary>
    /// ScriptableObject representing a crafting recipe.
    /// </summary>
    [CreateAssetMenu(fileName = "New Recipe", menuName = "Crafting/Recipe")]
    public class RecipeData : ScriptableObject
    {
        public string recipeName;
        public Sprite icon;
        public ItemData resultItem;
        public int resultAmount = 1;
        public List<Ingredient> ingredients;
        public bool isUnlockedByDefault = false;
    }
}