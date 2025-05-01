using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StoreManager : MonoBehaviour
{
    [Header("패널 참조")]
    [SerializeField] private GameObject storePanel; // 
    [SerializeField] private Transform categoryButtonParent;
    [SerializeField] private Transform subCategoryButtonParent;
    [SerializeField] private Transform itemButtonParent;

    [Header("프리팹")]
    [SerializeField] private GameObject categoryButtonPrefab;
    [SerializeField] private GameObject subCategoryButtonPrefab;
    [SerializeField] private GameObject itemButtonPrefab;
    [SerializeField] private GameObject purchasePopupPrefab; // 구매확인

    [Header("아이템 데이터")]
    [SerializeField] private List<StoreItemData> storeItems;

    private ItemCategory selectedCategory;
    private System.Enum selectedSubCategory;

    public void Open()
    {
        storePanel.SetActive(true);
        ShowCategoryButtons();
    }

    public void Close()
    {
        storePanel.SetActive(false);
        ClearAll();
    }

    private void ClearAll()
    {
        ClearChildren(categoryButtonParent);
        ClearChildren(subCategoryButtonParent);
        ClearChildren(itemButtonParent);
    }


    private void ShowCategoryButtons()
    {
        ClearChildren(categoryButtonParent);
        foreach (ItemCategory category in System.Enum.GetValues(typeof(ItemCategory)))
        {
            GameObject buttonObj = Instantiate(categoryButtonPrefab, categoryButtonParent);
            var button = buttonObj.GetComponent<Button>();
            var text = buttonObj.GetComponentInChildren<TextMeshProUGUI>();
            text.text = ConvertEnumToKorean(category.ToString());
            button.onClick.AddListener(() => SelectCategory(category));
        }
    }

    private void SelectCategory(ItemCategory category)
    {
        selectedCategory = category;
        ShowSubCategories(category);
    }

    private void ShowSubCategories(ItemCategory category)
    {
        ClearChildren(subCategoryButtonParent);
        ClearChildren(itemButtonParent);

        var subType = GetSubCategoryType(category);
        foreach (var sub in System.Enum.GetValues(subType))
        {
            GameObject buttonObj = Instantiate(subCategoryButtonPrefab, subCategoryButtonParent);
            var button = buttonObj.GetComponent<Button>();
            var text = buttonObj.GetComponentInChildren<TextMeshProUGUI>();
            text.text = ConvertEnumToKorean(sub.ToString());
            button.onClick.AddListener(() => SelectSubCategory((System.Enum)sub));
        }
    }

    private void SelectSubCategory(System.Enum subCategory)
    {
        selectedSubCategory = subCategory;
        ShowStoreItems();
    }

    private void ShowStoreItems()
    {
        ClearChildren(itemButtonParent);
        var filteredItems = storeItems.Where(i =>
            i.category == selectedCategory &&
            i.subCategory.ToString() == selectedSubCategory.ToString()).ToList();

        foreach (var item in filteredItems)
        {
            GameObject itemObj = Instantiate(itemButtonPrefab, itemButtonParent);
            var icon = itemObj.transform.Find("Icon").GetComponent<Image>();
            var name = itemObj.transform.Find("Name").GetComponent<TextMeshProUGUI>();
            var price = itemObj.transform.Find("Price").GetComponent<TextMeshProUGUI>();

            icon.sprite = item.icon;
            name.text = item.itemName;
            price.text = $"{item.price}";

            itemObj.GetComponent<Button>().onClick.AddListener(() =>
            {
                OpenPurchasePopup(item);
            });
        }
    }

    private void OpenPurchasePopup(StoreItemData itemData)
    {
        GameObject popup = Instantiate(purchasePopupPrefab, storePanel.transform);
        var cancelBtn = popup.transform.Find("Btn_Cancel").GetComponent<Button>();
        var confirmBtn = popup.transform.Find("Btn_Purchase").GetComponent<Button>();

        popup.transform.Find("ItemName").GetComponent<TextMeshProUGUI>().text = itemData.itemName;
        popup.transform.Find("Price").GetComponent<TextMeshProUGUI>().text = $"{itemData.price}";

        confirmBtn.onClick.AddListener(() =>
        {
            TryPurchase(itemData);
            Destroy(popup);
        });

        cancelBtn.onClick.AddListener(() =>
        {
            Destroy(popup);
        });
    }

    private void TryPurchase(StoreItemData item)
    {
        bool success = false;
        var stats = PlayerStatsManager.Instance;

        switch (item.currency)
        {
            case CurrencyType.Coin:
                if (stats.Coin >= item.price)
                {
                    stats.AddCoin(-item.price);
                    success = true;
                }
                break;
            case CurrencyType.Gem:
                if (stats.Gem >= item.price)
                {
                    stats.AddGem(-item.price);
                    success = true;
                }
                break;
            case CurrencyType.CoffeeBean:
                if (stats.CoffeeBeans >= item.price)
                {
                    stats.AddCoffeeBean(-item.price);
                    success = true;
                }
                break;
        }

        if (success)
        {
            Inventory.Instance.AddItem(item.itemData, 1);
        }
        else
        {
            Debug.Log("재화 부족!");
        }
    }

    private void ClearChildren(Transform parent)
    {
        foreach (Transform child in parent)
            Destroy(child.gameObject);
    }

    private System.Type GetSubCategoryType(ItemCategory category)
    {
        return category switch
        {
            ItemCategory.Kitchen => typeof(KitchenType),
            ItemCategory.Interior => typeof(InteriorType),
            ItemCategory.Exterior => typeof(ExteriorType),
            _ => null
        };
    }

    private string ConvertEnumToKorean(string name)
    {
        return InventoryUI._enumKoreanMap.TryGetValue(name, out var korean) ? korean : name;
    }
}
