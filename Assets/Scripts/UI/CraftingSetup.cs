using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// This class is responsible for setting up and managing a single crafting slot in the UI.
/// </summary>
public class CraftingSetup : MonoBehaviour
{
    public Image icon;
    public TextMeshProUGUI quantityText;
    public Button craftButton;

    public void SetIcon(Sprite icon)
    {
        this.icon.sprite = icon;
    }

    public void SetQuantity(int quantity)
    {
        quantityText.text = (quantity > 1) ? quantity.ToString() : string.Empty;
    }
}
