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

    [Header("ShoptCategory UI")]
    //public GameObject shopCategoryPanel;

    [Header("SubCategory UI")]
    public GameObject shopItemPanel;
    public Transform categoryButtonParent;
    public GameObject categoryButtonPrefab;

    [Header("Item List")]
    public Transform itemParent; // itemParent: UI_ShopPanel�� content, UI_DecoPanel�� content
    public GameObject itemPrefab;

    [Header("Player Info")]
    public TextMeshProUGUI coinText;

    private ShopCategoryType currentCategory;
    private DecoSubCategory currentSubCategory;

    void Start()
    {
        materialTab.onClick.AddListener(() => OnTabSelected(ShopCategoryType.Material));
        upgradeTab.onClick.AddListener(() => OnTabSelected(ShopCategoryType.Upgrade));
        decoTab.onClick.AddListener(() => OnTabSelected(ShopCategoryType.Decoration));

        OnTabSelected(ShopCategoryType.Material);
        UpdateCoinUI();
    }

    void OnTabSelected(ShopCategoryType category)
    {
        currentCategory = category;

        if (category == ShopCategoryType.Decoration)
        {
            ClearItems();
            ShowSubCategories();
        }
        else
        {
            ClearItems();
            ShowItems(ShopManager.Instance.GetItems(category));
        }

    }

    void ShowSubCategories() // ����ī�װ� ������(����)
    {
        Debug.Log("����ī�װ� ������");
        ClearChildren(categoryButtonParent);

        GameObject backButton = Instantiate(categoryButtonPrefab, categoryButtonParent);
        backButton.GetComponentInChildren<TextMeshProUGUI>().text = "�ڷ� ����";
        backButton.GetComponent<Button>().onClick.AddListener(() =>
        {
            ClearChildren(categoryButtonParent);
            OnTabSelected(ShopCategoryType.Material);

            foreach (ShopCategoryType category in System.Enum.GetValues(typeof(ShopCategoryType)))
            {
                GameObject btn = Instantiate(categoryButtonPrefab, categoryButtonParent);
                btn.GetComponentInChildren<TextMeshProUGUI>().text = ConvertToKorean(category.ToString());
                btn.GetComponent<Button>().onClick.RemoveAllListeners();
                btn.GetComponent<Button>().onClick.AddListener(() =>
                {
                    OnTabSelected(category);
                });

            }

        });

        foreach (DecoSubCategory sub in System.Enum.GetValues(typeof(DecoSubCategory)))
        {
            if (sub == DecoSubCategory.None) continue; // None�� ��� ����

            GameObject btn = Instantiate(categoryButtonPrefab, categoryButtonParent);
            btn.GetComponentInChildren<TextMeshProUGUI>().text = ConvertToKorean(sub.ToString());
            btn.GetComponent<Button>().onClick.RemoveAllListeners();
            btn.GetComponent<Button>().onClick.AddListener(() =>
            {
                currentSubCategory = sub;
                ClearItems();
                ShowItems(ShopManager.Instance.GetItems(currentCategory, sub));
            });
        }
    }

    void ShowItems(List<ShopItemData> items)
    {
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


    void UpdateCoinUI()
    {
        coinText.text = PlayerStatsManager.Instance.Coin.ToString();
    }

    string ConvertToKorean(string name)
    {
        return name switch
        {
            "Material" => "���",
            "Upgrade" => "���׷��̵�",
            "Decoration" => "���",
            "Item" => "������",
            "Wall" => "��",
            "Floor" => "�ٴ�",
            "Table" => "���̺�",
            _ => name
        };
    }
}
