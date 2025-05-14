using System.Collections;
using System.Collections.Generic;
using DalbitCafe.Deco;
using DalbitCafe.Operations;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;

[System.Serializable]
public class SubCategoryIconEntry // ����ī�װ� ������ ���
{
    public string enumName;
    public Sprite iconSprite;
}
public class InventoryUI : MonoBehaviour
{
    [Header("UI ���")]
    [SerializeField] private GameObject inventoryPanel; 
    [SerializeField] private Transform categoryButtonParent; 
    [SerializeField] private Transform subCategoryButtonParent; // ���� ��� ��ư�� �θ�
    [SerializeField] private Transform itemButtonParent; // ������ ��ư�� �θ�

    [Header("������")]
    [SerializeField] private GameObject categoryButtonPrefab;
    [SerializeField] private GameObject subCategoryButtonPrefab;
    [SerializeField] private GameObject itemButtonPrefab;
    private GameObject draggableItemPrefab;

    [Header("����ī�װ� ������ ��")]
    [SerializeField] private List<SubCategoryIconEntry> subCategoryIcons; // SerializedField�� �־���
    private Dictionary<string, Sprite> _subCategoryIconMap;

 

    private ItemCategory selectedCategory; // ���õ� ī�װ�
    private System.Enum selectedSubCategory; // ���õ� ���� ���
    private Button prieviousButton; // ������ ���õƴ� ��ư

    private Inventory inventory;

    private void Start()
    {
        inventory = FindObjectOfType<Inventory>();
        inventoryPanel.SetActive(false);

        _subCategoryIconMap = new Dictionary<string, Sprite>();
        foreach(var entry in subCategoryIcons)
        {
            if (!_subCategoryIconMap.ContainsKey(entry.enumName)) // �ش� Ű�� ���ٸ� ��� �߰�����
                _subCategoryIconMap.Add(entry.enumName, entry.iconSprite);
        }
    }

    public void Open()
    {
        inventoryPanel.SetActive(true);
        ShowCategoryButtons(); // (3��)
    }

    public void Close()
    {
        inventoryPanel.SetActive(false);
        ClearAll(); // ���� �����ֱ�
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

    private void ShowSubCategories(ItemCategory category) // ���õ� ��з� ī�װ��� ���� ���� ī�װ� ��ư�� ������
    {
        ClearChildren(subCategoryButtonParent);
        ClearChildren(itemButtonParent);

        System.Type subCategoryType = GetSubCategoryType(category);

        foreach (var subCategory in System.Enum.GetValues(subCategoryType)) // �ش� ���� enum�� ������ ���� ��ư�� ����
        {
            GameObject buttonObj = Instantiate(subCategoryButtonPrefab, subCategoryButtonParent);
            Button button = buttonObj.GetComponent<Button>();
            //TextMeshProUGUI buttonText = buttonObj.GetComponentInChildren<TextMeshProUGUI>();
            //buttonText.text = ConvertEnumToKorean(subCategory.ToString()); // �ѱ۷� �ٲ���

            // ������ �̹��� ã��
            Image iconImage = buttonObj.transform.Find("Icon").GetComponent<Image>();
            if (_subCategoryIconMap.TryGetValue(subCategory.ToString(), out var icon))
            {
                iconImage.sprite = icon;
                iconImage.enabled = true;
                iconImage.preserveAspect = true;
            }
            else
            {
                iconImage.enabled = false; // ������ ������ �����
            }
            // ��ư ��Ÿ�� �ֱ�(�ѹ��� �ߺ� ��ġ���� ���ϵ���)

            var capturedButton = button;
            button.onClick.AddListener(() =>
            {
                if (!button.interactable) return;
                Debug.Log("����ī�װ� ��ư ����");
                button.interactable = false;
                button.GetComponent<Image>().enabled = true;
                SelectSubCategory((System.Enum)subCategory);
                prieviousButton = capturedButton; // ����

                StartCoroutine(EnableButtonAfterDelay(button, 0.1f)); // 0.1�� �� �ٽ� Ȱ��ȭ
            });
        }
    }

    private void SelectSubCategory(System.Enum subCategory) // 
    {
        if(prieviousButton != null)
        {
            Debug.Log("���� ��ư ��Ȱ��ȭ");
            prieviousButton.GetComponent<Image>().enabled = false;  // ������ ������ ��ư ���� ���ֱ�

        }
        selectedSubCategory = subCategory;
        ShowItems();
    }

    private void ShowItems()
    {
        ClearChildren(itemButtonParent);

        List<InventoryItem> itemsInCategory = inventory.GetItemsByCategory(selectedCategory); // ������ ī�װ��� �����͵�

        foreach (var item in itemsInCategory)
        {
            if (IsItemInSelectedSubCategory(item.itemData))
            {

                GameObject buttonObj = Instantiate(itemButtonPrefab, itemButtonParent);

                Image iconImage = buttonObj.transform.Find("Icon").GetComponent<Image>();
                iconImage.preserveAspect = true;
                TextMeshProUGUI nameText = buttonObj.transform.Find("Name").GetComponent<TextMeshProUGUI>();
                TextMeshProUGUI quantityText = buttonObj.transform.Find("Quantity").GetComponent<TextMeshProUGUI>();
                Button mainButton = buttonObj.GetComponent<Button>();
                Image fadeImage = buttonObj.transform.Find("FadeImage").GetComponent<Image>();  

                // ������ ��ư�� ã��
                GameObject placeButtonObj = buttonObj.transform.Find("PlaceButton").gameObject;
                GameObject sellBtuttonObj = buttonObj.transform.Find("SellButton").gameObject;

                // ó���� �����
                placeButtonObj.SetActive(false);    
                sellBtuttonObj.SetActive(false);
                fadeImage.enabled = false;

                // �ؽ�Ʈ & �̹���
                iconImage.sprite = item.itemData.icon;
                nameText.text = item.itemData.itemName;
                quantityText.text = $"x{item.quantity}";

                // Ŭ�� �� : ��ġ/�Ǹ� ��ư ���̱� + ��ư ��Ӱ�
                mainButton.onClick.AddListener(() =>
                {
                    Debug.Log("������ ��ư ����");
                    fadeImage.enabled = true;
                    //fadeImage.color = new Color(0, 0, 0, 0.7f);
                    placeButtonObj.SetActive(true);
                    sellBtuttonObj.SetActive(true);
                });

                // ��ġ�ϱ� ��ư ���
                placeButtonObj.GetComponent<Button>().onClick.AddListener(() =>
                {
                    HandlePlaceItem(item, buttonObj); // ����
                });

                // �Ǹ��ϱ� ����� ���߿� �߰� ����
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

    private void HandlePlaceItem(InventoryItem item, GameObject buttonObj)
    {
        // ���� ����
        item.quantity--;

        // ���� UI ����
        TextMeshProUGUI qtyText = buttonObj.transform.Find("Quantity").GetComponent<TextMeshProUGUI>(); 
        qtyText.text = $"x{item.quantity}";

        // ���� 0�̸� �κ��丮���� ����
        if(item.quantity <=0)
        {
            inventory.RemoveItem(item);
            Destroy(buttonObj);
        }

        // �κ��丮 �ݱ�
        Close();

        // ������ ��ġ �õ�
        PlaceDraggableItem(item.itemData);
    }

    private void PlaceDraggableItem(ItemData data)
    {
        draggableItemPrefab = data.prefab;   
        // ������ ����
        GameObject go = Instantiate(draggableItemPrefab);

        DraggableItem draagable = go.GetComponent<DraggableItem>();

        // ��ġ ã�� (0,0���� ���� ����� �� ����)
        Vector2Int position = FindNearestPlaceablePosition(draagable._itemSize);

        // ��ǥ�� �� �߽����� ��ȯ 
        Vector3 worldPos = GridManager.Instance.tilemap.GetCellCenterWorld(new Vector3Int(position.x, position.y, 0));
        go.transform.position = worldPos;

        // ��ġ ó��
        GridManager.Instance.PlaceItem(position, draagable._itemSize);   
        
    }

    // �� �� ã��(0, 0 ���� ����)
    private Vector2Int FindNearestPlaceablePosition(Vector2Int itemSize)
    {
        // Ž�� ���� ����
        int searchRadius = 20; // �� ������

        for(int r = 0; r <= searchRadius; r++)
        {
            for(int x = -r; x<=r; x++)
            {
                for(int y= -r; y<=r; y++)
                {
                    Vector2Int pos = new Vector2Int(x, y);
                    if(DecorateManager.Instance.CanPlaceItem(pos, itemSize))
                    {
                        return pos;
                    }
                }
            }
        }

        Debug.LogWarning("�� ������ ã�� �� �����ϴ�.");
        return Vector2Int.zero;
    }
}
