using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ShowcaseManager : MonoBehaviour
{
    [SerializeField] private Transform contentParent; // Content 오브젝트
    [SerializeField] private GameObject showcaseItemPrefab;
    [SerializeField] private GameObject showcaseWindow;
    [SerializeField] private Button exitButton;

    private void OnEnable()
    {
        RefreshShowcase();
    }

    private void Start()
    {
        exitButton.onClick.AddListener(() => showcaseWindow.SetActive(false));
    }

    public void RefreshShowcase()
    {
        // 기존 아이템 제거
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }

        // 인벤토리에서 Food 아이템 가져오기
        var foodItems = Inventory.Instance.GetItemsByCategory(ItemCategory.Food);

        foreach (var inventoryItem in foodItems)
        {
            var newItem = Instantiate(showcaseItemPrefab, contentParent);
            var ui = newItem.GetComponent<ShowcaseItemUI>();
            ui.Setup(inventoryItem);
        }
    }


    public void ToggleShowcaseUI()
    {
        if (showcaseWindow.activeSelf)
        {
            showcaseWindow.SetActive(false);
        }
        else
        {
            RefreshShowcase();
            showcaseWindow.SetActive(true);
        }
    }
}
