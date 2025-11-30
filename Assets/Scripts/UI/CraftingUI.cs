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
        [SerializeField] private GameObject m_slotPrefab;
        [SerializeField] private GameObject m_panel;
        [SerializeField] private RecipeDatabase m_recipeDatabase;
        [SerializeField] private Inventory m_inventory;
        [SerializeField] private InventoryUI m_inventoryUI;

        private Transform m_GridParent;

        private void Awake()
        {
            var grid = m_panel.GetComponentInChildren<GridLayoutGroup>();

            if (grid != null)
            {
                m_GridParent = grid.transform;
            }
            else
            {
                Debug.LogError("CraftingUI: Could not find GridLayoutGroup in m_Panel children.");
            }
        }

        private bool isOpen = false;

        private void Start()
        {
            if (m_panel != null)
                m_panel.SetActive(false);

            Refresh();
        }

        public void Toggle()
        {
            isOpen = !isOpen;
            m_panel.SetActive(isOpen);

            if (isOpen)
                Refresh();
        }

        private void Refresh()
        {
            // Clear previous UI
            foreach (Transform child in m_GridParent)
                Destroy(child.gameObject);

            // Populate UI
            foreach (var recipe in m_recipeDatabase.allRecipes)
            {
                var slot = Instantiate(m_slotPrefab, m_GridParent);
                var craftingSlot = slot.GetComponent<CraftingSetup>();

                // Slot setup
                craftingSlot.craftButton.interactable = CanCraft(recipe);
                craftingSlot.craftButton.onClick.AddListener(() => OnClickSlot(recipe, slot));

                craftingSlot.SetIcon(recipe.icon);
            }
            LayoutRebuilder.ForceRebuildLayoutImmediate(m_GridParent.GetComponent<RectTransform>());
        }

        public void OnClickSlot(RecipeData recipe, GameObject slot)
        {
            if (CanCraft(recipe))
                CraftItem(recipe);
            else
                Debug.Log("Cannot craft " + recipe.resultItem.itemName + ". Missing ingredients.");
        }

        private bool CanCraft(RecipeData recipe)
        {
            foreach (var ingredient in recipe.ingredients)
                if (!m_inventory.HasItem(ingredient.item, ingredient.amount)) return false;
            return true;
        }

        private void CraftItem(RecipeData recipe)
        {
            foreach (var ingredient in recipe.ingredients)
            {
                m_inventory.RemoveItem(ingredient.item, ingredient.amount);
            }
            m_inventory.AddItem(recipe.resultItem, recipe.resultAmount);
            m_inventoryUI.Refresh();
            Refresh();
        }
    }
}
