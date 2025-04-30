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
            buttonText.text = ConvertCategoryToKorean(category);




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
            buttonText.text = buttonText.text = ConvertToKoreanText(subCategory.ToString());


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

    private string ConvertCategoryToKorean(ItemCategory category)
    {
        switch (category)
        {
            case ItemCategory.Kitchen: return "주방";
            case ItemCategory.Interior: return "인테리어";
            case ItemCategory.Exterior: return "익스테리어";
            default: return category.ToString();
        }
    }


    private string ConvertToKoreanText(string enumName)
    {
        // 필요에 따라 Enum -> 한글 매핑
        switch (enumName)
        {
            // 주방
            case "RoastingMachine": return "로스팅머신";
            case "CoffeeMachine": return "커피머신";
            case "Workbench": return "작업대";
            case "CookingMachine": return "쿠킹머신";
            case "Showcase": return "쇼케이스";
            case "Counter": return "계산대";
            case "Mixer": return "믹서기";

            // 인테리어
            case "Table": return "테이블";
            case "Chair": return "의자";
            case "Partition": return "파티션";
            case "Decoration": return "장식품";
            case "BeanContainer": return "원두통";
            case "WallDecoration": return "벽장식";
            case "Tile": return "타일";
            case "WallPaper": return "벽지";

            //익스테리어
            case "SecondFloorOnly": return "2층전용";
            case "OutdoorDecoration": return "야외장식품";
            case "WallExteriorDecoration": return "건물 외벽 장식";
            case "Railing2F": return "2층 난간";
            case "Stair2F": return "2층 계단";
            case "WallExterior": return "건물 외벽";
            case "Entrance": return "입구";

            default: return enumName;
 

        }
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
