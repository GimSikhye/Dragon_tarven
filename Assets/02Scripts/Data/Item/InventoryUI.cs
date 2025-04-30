using System.Collections.Generic;
using DalbitCafe.Operations;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;

public class InventoryUI : MonoBehaviour
{
    [Header("�г� ����")]
    [SerializeField] private GameObject inventoryPanel; 
    [SerializeField] private Transform categoryButtonParent; 
    [SerializeField] private Transform subCategoryButtonParent; // ���� ��� ��ư�� �θ�
    [SerializeField] private Transform itemButtonParent; // ������ ��ư�� �θ�

    [Header("������")]
    [SerializeField] private GameObject categoryButtonPrefab;
    [SerializeField] private GameObject subCategoryButtonPrefab;
    [SerializeField] private GameObject itemButtonPrefab;

    private ItemCategory selectedCategory; // ���õ� ī�װ�
    private System.Enum selectedSubCategory; // ���õ� ���� ���

    private Inventory inventory;

    private void Start()
    {
        inventory = FindObjectOfType<Inventory>();
        inventoryPanel.SetActive(false); // ó������ ���α�
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


    // Ŭ���� �� �ʵ�
    public static readonly Dictionary<string, string> _enumKoreanMap = new()
{
    // ī�װ�
    { "Kitchen", "�ֹ�" },
    { "Interior", "���׸���" },
    { "Exterior", "�ͽ��׸���" },

    // �ֹ� ����ī�װ�
    { "RoastingMachine", "�ν��øӽ�" },
    { "CoffeeMachine", "Ŀ�Ǹӽ�" },
    { "Workbench", "�۾���" },
    { "CookingMachine", "��ŷ�ӽ�" },
    { "Showcase", "�����̽�" },
    { "Counter", "����" },
    { "Mixer", "�ͼ���" },

    // ���׸���
    { "Table", "���̺�" },
    { "Chair", "����" },
    { "Partition", "��Ƽ��" },
    { "Decoration", "���ǰ" },
    { "BeanContainer", "������" },
    { "WallDecoration", "�����" },
    { "Tile", "Ÿ��" },
    { "Wallpaper", "����" },

    // �ͽ��׸���
    { "SecondFloorOnly", "2������" },
    { "OutdoorDecoration", "�߿����ǰ" },
    { "WallExteriorDecoration", "�ǹ� �ܺ� ���" },
    { "Railing2F", "2�� ����" },
    { "Stair2F", "2�� ���" },
    { "WallExterior", "�ǹ� �ܺ�" },
    { "Entrance", "�Ա�" }
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
