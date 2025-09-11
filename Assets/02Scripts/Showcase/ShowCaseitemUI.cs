using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShowcaseItemUI : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI quantityText;

    public void Setup(InventoryItem inventoryItem)
    {
        if (inventoryItem.itemData is FoodItemData foodItem)
        {
            iconImage.sprite = foodItem.icon; 
            quantityText.text = $"x{inventoryItem.quantity}";
        }
    }
}
