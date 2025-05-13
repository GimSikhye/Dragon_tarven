using System.Collections;
using System.Collections.Generic;
using DalbitCafe.Operations;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;

[System.Serializable]
public class SubCategoryIconEntry // 하위카테고리 아이콘 명단
{
    public string enumName;
    public Sprite iconSprite;
}
public class InventoryUI : MonoBehaviour
{
    [Header("UI 요소")]
    [SerializeField] private GameObject inventoryPanel; 
    [SerializeField] private Transform categoryButtonParent; 
    [SerializeField] private Transform subCategoryButtonParent; // 하위 목록 버튼들 부모
    [SerializeField] private Transform itemButtonParent; // 아이템 버튼들 부모

    [Header("프리팹")]
    [SerializeField] private GameObject categoryButtonPrefab;
    [SerializeField] private GameObject subCategoryButtonPrefab;
    [SerializeField] private GameObject itemButtonPrefab;

    [Header("서브카테고리 아이콘 맵")]
    [SerializeField] private List<SubCategoryIconEntry> subCategoryIcons; // SerializedField로 넣어줌
    private Dictionary<string, Sprite> _subCategoryIconMap;

 

    private ItemCategory selectedCategory; // 선택된 카테고리
    private System.Enum selectedSubCategory; // 선택된 하위 목록
    private Button prieviousButton; // 이전에 선택됐던 버튼

    private Inventory inventory;

    private void Start()
    {
        inventory = FindObjectOfType<Inventory>();
        inventoryPanel.SetActive(false);

        _subCategoryIconMap = new Dictionary<string, Sprite>();
        foreach(var entry in subCategoryIcons)
        {
            if (!_subCategoryIconMap.ContainsKey(entry.enumName)) // 해당 키가 없다면 요소 추가해줌
                _subCategoryIconMap.Add(entry.enumName, entry.iconSprite);
        }
    }

    public void Open()
    {
        inventoryPanel.SetActive(true);
        ShowCategoryButtons(); // (3개)
    }

    public void Close()
    {
        inventoryPanel.SetActive(false);
        ClearAll(); // 전부 지워주기
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
            buttonText.text = ConvertEnumToKorean(category.ToString()); // ConvertEnumToKorean


            button.onClick.AddListener(() =>
            {
                SelectCategory(category);
            });
        }
    }

    private void SelectCategory(ItemCategory category)
    {
        selectedCategory = category;
        ShowSubCategories(category);
    }

    private void ShowSubCategories(ItemCategory category) // 선택된 대분류 카테고리에 따라 하위 카테고리 버튼을 생성함
    {
        ClearChildren(subCategoryButtonParent);
        ClearChildren(itemButtonParent);

        System.Type subCategoryType = GetSubCategoryType(category);

        foreach (var subCategory in System.Enum.GetValues(subCategoryType)) // 해당 하위 enum들 각각에 대한 버튼을 생성
        {
            GameObject buttonObj = Instantiate(subCategoryButtonPrefab, subCategoryButtonParent);
            Button button = buttonObj.GetComponent<Button>();
            //TextMeshProUGUI buttonText = buttonObj.GetComponentInChildren<TextMeshProUGUI>();
            //buttonText.text = ConvertEnumToKorean(subCategory.ToString()); // 한글로 바꿔줌

            // 아이콘 이미지 찾기
            Image iconImage = buttonObj.transform.Find("Icon").GetComponent<Image>();
            if (_subCategoryIconMap.TryGetValue(subCategory.ToString(), out var icon))
            {
                iconImage.sprite = icon;
                iconImage.enabled = true;
                iconImage.preserveAspect = true;
            }
            else
            {
                iconImage.enabled = false; // 아이콘 없으면 숨기기
            }
            // 버튼 쿨타임 주기(한번에 중복 터치하지 못하도록)

            var capturedButton = button;
            button.onClick.AddListener(() =>
            {
                if (!button.interactable) return;
                Debug.Log("서브카테고리 버튼 눌림");
                button.interactable = false;
                button.GetComponent<Image>().enabled = true;
                SelectSubCategory((System.Enum)subCategory);
                prieviousButton = capturedButton; // 갱신

                StartCoroutine(EnableButtonAfterDelay(button, 0.1f)); // 0.1초 후 다시 활성화
            });
        }
    }

    private void SelectSubCategory(System.Enum subCategory) // 
    {
        if(prieviousButton != null)
        {
            Debug.Log("이전 버튼 비활성화");
            prieviousButton.GetComponent<Image>().enabled = false;  // 이전에 눌렀던 버튼 색깔 없애기

        }
        selectedSubCategory = subCategory;
        ShowItems();
    }

    private void ShowItems()
    {
        ClearChildren(itemButtonParent);

        List<InventoryItem> itemsInCategory = inventory.GetItemsByCategory(selectedCategory); // 선택한 카테고리와 같은것들

        foreach (var item in itemsInCategory)
        {
            if (IsItemInSelectedSubCategory(item.itemData))
            {

                GameObject buttonObj = Instantiate(itemButtonPrefab, itemButtonParent);

                Image iconImage = buttonObj.transform.Find("Icon").GetComponent<Image>();
                iconImage.preserveAspect = true;
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


    private bool IsItemInSelectedSubCategory(ItemData itemData) //
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

    private IEnumerator EnableButtonAfterDelay(Button btn, float delay)
    {
        yield return new WaitForSeconds(delay);
        btn.interactable = true;
    }

}
