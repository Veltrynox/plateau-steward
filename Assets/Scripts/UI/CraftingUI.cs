using UnityEngine;
using UnityEngine.UI;

namespace SubnauticaClone
{
    /// <summary>
    /// Manages the crafting UI, displaying recipes and handling crafting actions.
    /// </summary>
    public class CraftingUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GameObject slotPrefab;
        [SerializeField] private GameObject panel;
        [SerializeField] private RecipeDatabase recipeDatabase;
        [SerializeField] private Inventory inventory;
        [SerializeField] private InventoryUI inventoryUI;

        private Transform gridParent;

        private void Awake()
        {
            var grid = panel.GetComponentInChildren<GridLayoutGroup>();

            if (grid != null)
            {
                gridParent = grid.transform;
            }
            else
            {
                Debug.LogError("CraftingUI: Could not find GridLayoutGroup in m_panel children.");
            }
        }

        private bool isOpen = false;

        private void Start()
        {
            if (panel != null)
                panel.SetActive(false);

            Refresh();
        }

        public void Toggle()
        {
            isOpen = !isOpen;
            panel.SetActive(isOpen);

            if (isOpen)
                Refresh();
        }

        private void Refresh()
        {
            // Clear previous UI
            foreach (Transform child in gridParent)
                Destroy(child.gameObject);

            // Populate UI
            foreach (var recipe in recipeDatabase.allRecipes)
            {
                var slot = Instantiate(slotPrefab, gridParent);
                var craftingSlot = slot.GetComponent<MenuItemSetup>();
                craftingSlot.Setup(recipe.icon);
                bool canCraft = CanCraft(recipe);
                craftingSlot.InteractButton.interactable = canCraft;
                craftingSlot.InteractButton.onClick.AddListener(() => OnClickSlot(recipe));
            }

            LayoutRebuilder.ForceRebuildLayoutImmediate(gridParent.GetComponent<RectTransform>());
        }

        public void OnClickSlot(RecipeData recipe)
        {
            if (CanCraft(recipe))
            {
                CraftItem(recipe);
            }
            else
            {
                Debug.Log($"Cannot craft {recipe.resultItem.itemName}. Missing ingredients.");
            }
        }

        private bool CanCraft(RecipeData recipe)
        {
            foreach (var ingredient in recipe.ingredients)
            {
                if (!inventory.HasItem(ingredient.item, ingredient.amount))
                    return false;
            }
            return true;
        }

        private void CraftItem(RecipeData recipe)
        {
            foreach (var ingredient in recipe.ingredients)
            {
                inventory.RemoveItem(ingredient.item, ingredient.amount);
            }
            inventory.AddItem(recipe.resultItem, recipe.resultAmount);
            inventoryUI.Refresh();
            Refresh();
        }
    }
}