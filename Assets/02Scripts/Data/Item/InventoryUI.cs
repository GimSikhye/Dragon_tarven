using System.Collections.Generic;
using DalbitCafe.Operations;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;

public class InventoryUI : MonoBehaviour
{
    [Header("패널 참조")]
    [SerializeField] private GameObject inventoryPanel; 
    [SerializeField] private Transform categoryButtonParent; 
    [SerializeField] private Transform subCategoryButtonParent; // 하위 목록 버튼들 부모
    [SerializeField] private Transform itemButtonParent; // 아이템 버튼들 부모

    [Header("프리팹")]
    [SerializeField] private GameObject categoryButtonPrefab;
    [SerializeField] private GameObject subCategoryButtonPrefab;
    [SerializeField] private GameObject itemButtonPrefab;

    private ItemCategory selectedCategory; // 선택된 카테고리
    private System.Enum selectedSubCategory; // 선택된 하위 목록

    private Inventory inventory;

    private void Start()
    {
        inventory = FindObjectOfType<Inventory>();
        inventoryPanel.SetActive(false); // 처음에는 꺼두기
    }

    public void Open()
    {
        inventoryPanel.SetActive(true);
        ShowCategoryButtons(); // 
    }

    public void Close()
    {
        inventoryPanel.SetActive(false);
        ClearAll();
    }

    private void ClearAll()
    {
        ClearChildren(categoryButtonParent);
        ClearChildren(subCategoryButtonParent);
        ClearChildren(itemButtonParent);
    }

    private void ClearChildren(Transform parent)
    {
        foreach (Transform child in parent)
        {
            Destroy(child.gameObject);
        }
    }

    private void ShowCategoryButtons()
    {
        ClearChildren(categoryButtonParent); 

        foreach (ItemCategory category in System.Enum.GetValues(typeof(ItemCategory)))
        {
            GameObject buttonObj = Instantiate(categoryButtonPrefab, categoryButtonParent);
            Button button = buttonObj.GetComponent<Button>();
            TextMeshProUGUI buttonText = buttonObj.GetComponentInChildren<TextMeshProUGUI>();
            buttonText.text = ConvertEnumToKorean(category.ToString());




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

        System.Type subCategoryType = GetSubCategoryType(category);

        foreach (var subCategory in System.Enum.GetValues(subCategoryType))
        {
            GameObject buttonObj = Instantiate(subCategoryButtonPrefab, subCategoryButtonParent);
            Button button = buttonObj.GetComponent<Button>();
            TextMeshProUGUI buttonText = buttonObj.GetComponentInChildren<TextMeshProUGUI>();
            buttonText.text = buttonText.text = ConvertEnumToKorean(subCategory.ToString());


            button.onClick.AddListener(() => SelectSubCategory((System.Enum)subCategory));
        }
    }

    private void SelectSubCategory(System.Enum subCategory)
    {
        selectedSubCategory = subCategory;
        ShowItems();
    }

    private void ShowItems()
    {
        ClearChildren(itemButtonParent);

        List<InventoryItem> itemsInCategory = inventory.GetItemsByCategory(selectedCategory);

        foreach (var item in itemsInCategory)
        {
            if (IsItemInSelectedSubCategory(item.itemData))
            {
                GameObject buttonObj = Instantiate(itemButtonPrefab, itemButtonParent);

                Image iconImage = buttonObj.transform.Find("Icon").GetComponent<Image>();
                TextMeshProUGUI nameText = buttonObj.transform.Find("Name").GetComponent<TextMeshProUGUI>();
                TextMeshProUGUI quantityText = buttonObj.transform.Find("Quantity").GetComponent<TextMeshProUGUI>();

                iconImage.sprite = item.itemData.icon;
                nameText.text = item.itemData.itemName;
                quantityText.text = $"x{item.quantity}";
            }
        }
    }


    // 클래스 내 필드
    public static readonly Dictionary<string, string> _enumKoreanMap = new()
{
    // 카테고리
    { "Kitchen", "주방" },
    { "Interior", "인테리어" },
    { "Exterior", "익스테리어" },

    // 주방 서브카테고리
    { "RoastingMachine", "로스팅머신" },
    { "CoffeeMachine", "커피머신" },
    { "Workbench", "작업대" },
    { "CookingMachine", "쿠킹머신" },
    { "Showcase", "쇼케이스" },
    { "Counter", "계산대" },
    { "Mixer", "믹서기" },

    // 인테리어
    { "Table", "테이블" },
    { "Chair", "의자" },
    { "Partition", "파티션" },
    { "Decoration", "장식품" },
    { "BeanContainer", "원두통" },
    { "WallDecoration", "벽장식" },
    { "Tile", "타일" },
    { "Wallpaper", "벽지" },

    // 익스테리어
    { "SecondFloorOnly", "2층전용" },
    { "OutdoorDecoration", "야외장식품" },
    { "WallExteriorDecoration", "건물 외벽 장식" },
    { "Railing2F", "2층 난간" },
    { "Stair2F", "2층 계단" },
    { "WallExterior", "건물 외벽" },
    { "Entrance", "입구" }
};
    private string ConvertEnumToKorean(string enumName)
    {
        return _enumKoreanMap.TryGetValue(enumName, out var korean) ? korean : enumName;
    }



    private bool IsItemInSelectedSubCategory(ItemData itemData)
    {
        switch (selectedCategory)
        {
            case ItemCategory.Kitchen:
                return (itemData as KitchenItemData)?.kitchenType.ToString() == selectedSubCategory.ToString();
            case ItemCategory.Interior:
                return (itemData as InteriorItemData)?.interiorType.ToString() == selectedSubCategory.ToString();
            case ItemCategory.Exterior:
                return (itemData as ExteriorItemData)?.exteriorType.ToString() == selectedSubCategory.ToString();
            default:
                return false;
        }
    }

    private System.Type GetSubCategoryType(ItemCategory category)
    {
        switch (category)
        {
            case ItemCategory.Kitchen:
                return typeof(KitchenType);
            case ItemCategory.Interior:
                return typeof(InteriorType);
            case ItemCategory.Exterior:
                return typeof(ExteriorType);
            default:
                return null;
        }
    }
}
