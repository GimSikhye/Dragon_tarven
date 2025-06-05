using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Fridge : MonoBehaviour
{
    [SerializeField] private GameObject fridgeUI;
    [SerializeField] private Transform slotParent;
    [SerializeField] private GameObject fridgeSlotPrefab;

    private bool isOpen = false;

    public void ToggleFridgeUI(int slotCount)
    {
        isOpen = !isOpen;

        if (isOpen)
        {
            Debug.Log("≥√¿Â∞Ì ø≠∏≤");
            ShowFridgeItems(slotCount);
            fridgeUI.SetActive(true);
        }
        else
        {
            fridgeUI.SetActive(false);
        }
    }

    private void ShowFridgeItems(int slotCount)
    {
        foreach (Transform child in slotParent)
            Destroy(child.gameObject);

        List<InventoryItem> fridgeItems = Inventory.Instance
            .GetItemsByCategory(ItemCategory.Material)
            .Where(item =>
            {
                var sub = item.itemData.SubCategory;
                return sub != null &&
                        (MaterialType)sub == MaterialType.SideMenu ||
                        (MaterialType)sub == MaterialType.Both;
            })
            .ToList();

        for (int i = 0; i < slotCount; i++)
        {
            GameObject slot = Instantiate(fridgeSlotPrefab, slotParent);
            if (i < fridgeItems.Count)
                slot.GetComponent<FridgeSlot>().SetItem(fridgeItems[i].itemData.icon);
            else
                slot.GetComponent<FridgeSlot>().SetItem(null);
        }
    }
}
