using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Database for all crafting recipes in the game.
/// </summary>
namespace SubnauticaClone
{
    [CreateAssetMenu(fileName = "RecipeDatabase", menuName = "Crafting/RecipeDatabase")]
    public class RecipeDatabase : ScriptableObject
    {
        public List<RecipeData> allRecipes;
    }
}