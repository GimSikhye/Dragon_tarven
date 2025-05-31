using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ShopUIController : MonoBehaviour
{
    [Header("Tab Buttons")]
    public Button materialTab;
    public Button upgradeTab;
    public Button decoTab;

    [Header("SubCategory UI")]
    public GameObject subCategoryPanel;
    public Button backButton;
    public Transform subCategoryButtonParent;
    public GameObject subCategoryButtonPrefab;

    [Header("Item List")]
    public Transform itemParent;
    public GameObject itemPrefab;

    [Header("Player Info")]
    public TMP_Text coinText;

    private ShopCategoryType currentCategory;
    private DecoSubCategory currentSubCategory;

    void Start()
    {
        materialTab.onClick.AddListener(() => OnTabSelected(ShopCategoryType.Material));
        upgradeTab.onClick.AddListener(() => OnTabSelected(ShopCategoryType.Upgrade));
        decoTab.onClick.AddListener(() => OnTabSelected(ShopCategoryType.Decoration));
        backButton.onClick.AddListener(CloseSubCategories);

        OnTabSelected(ShopCategoryType.Material);
        UpdateCoinUI();
    }

    void OnTabSelected(ShopCategoryType category)
    {
        currentCategory = category;

        subCategoryPanel.SetActive(category == ShopCategoryType.Decoration);
        ClearItems();

        if (category == ShopCategoryType.Decoration)
        {
            ShowSubCategories();
        }
        else
        {
            ShowItems(ShopManager.Instance.GetItems(category));
        }
    }

    void ShowSubCategories()
    {
        ClearChildren(subCategoryButtonParent);
        foreach (DecoSubCategory sub in System.Enum.GetValues(typeof(DecoSubCategory)))
        {
            GameObject btn = Instantiate(subCategoryButtonPrefab, subCategoryButtonParent);
            btn.GetComponentInChildren<TMP_Text>().text = ConvertToKorean(sub.ToString());
            btn.GetComponent<Button>().onClick.AddListener(() =>
            {
                currentSubCategory = sub;
                ShowItems(ShopManager.Instance.GetItems(currentCategory, sub));
            });
        }
    }

    void ShowItems(List<ShopItemData> items)
    {
        ClearItems();
        foreach (var item in items)
        {
            GameObject obj = Instantiate(itemPrefab, itemParent);
            obj.GetComponent<ShopItemButton>().Init(item, UpdateCoinUI);
        }
    }

    void ClearItems()
    {
        ClearChildren(itemParent);
    }

    void ClearChildren(Transform parent)
    {
        foreach (Transform child in parent) Destroy(child.gameObject);
    }

    void CloseSubCategories()
    {
        ShowSubCategories();
    }

    void UpdateCoinUI()
    {
        coinText.text = PlayerStatsManager.Instance.Coin.ToString();
    }

    string ConvertToKorean(string name)
    {
        return name switch
        {
            "Item" => "아이템",
            "Wall" => "벽",
            "Floor" => "바닥",
            "Box" => "박스",
            "Table" => "테이블",
            _ => name
        };
    }
}
