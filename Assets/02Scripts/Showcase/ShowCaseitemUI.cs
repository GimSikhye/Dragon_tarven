using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class ShowcaseItemUI : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI quantityText;
    [SerializeField] private Vector2 tooltipOffset = new Vector2(20f, -20f);

    private InventoryItem inventoryItem;
    private TooltipUI tooltip;
    private SellConfirmUI sellConfirmUI;

    private float lastTapTime = 0f;
    private const float doubleTapThreshold = 0.3f; // 0.3초 안에 두 번 터치하면 더블탭

    public void Setup(InventoryItem inventoryItem)
    {
        this.inventoryItem = inventoryItem;

        if (inventoryItem.itemData is FoodItemData foodItem)
        {
            iconImage.sprite = foodItem.icon;
            quantityText.text = $"x{inventoryItem.quantity}";
        }

        if (sellConfirmUI == null)
            sellConfirmUI = FindAnyObjectByType<SellConfirmUI>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (inventoryItem == null) return;

        // 툴팁 표시
        if (tooltip == null) tooltip = FindAnyObjectByType<TooltipUI>();
        tooltip.tooltipObject.SetActive(true);
        tooltip.SetData(inventoryItem.itemData.itemName, inventoryItem.itemData.price);
        PositionTooltip();

        // 더블탭 감지
        if (Time.time - lastTapTime < doubleTapThreshold)
        {
            Debug.Log("더블탭!");
            ShowSellConfirm();
            lastTapTime = 0f; // 초기화
        }
        else
        {
            lastTapTime = Time.time;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (tooltip != null) tooltip.tooltipObject.SetActive(false);
    }

    private void ShowSellConfirm()
    {
        if (inventoryItem == null || sellConfirmUI == null) return;

        sellConfirmUI.Show(inventoryItem.itemData.itemName, () =>
        {
            // 코인 추가
            PlayerStatsManager.Instance.AddCoin(inventoryItem.itemData.price);

            // 아이템 1개 제거
            Inventory.Instance.RemoveItemAmount(inventoryItem.itemData, 1);

            // Showcase 갱신
            FindAnyObjectByType<ShowcaseManager>()?.RefreshShowcase();
        });
    }

    private void PositionTooltip()
    {
        RectTransform slotRect = transform as RectTransform;
        RectTransform tooltipRect = tooltip.tooltipObject.transform as RectTransform;
        Canvas canvas = tooltipRect.GetComponentInParent<Canvas>();
        RectTransform canvasRect = canvas.transform as RectTransform;

        Vector3[] corners = new Vector3[4];
        slotRect.GetWorldCorners(corners);
        Vector3 targetWorldPos = corners[3];

        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            RectTransformUtility.WorldToScreenPoint(null, targetWorldPos),
            canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera,
            out localPoint);

        localPoint += tooltipOffset;
        tooltipRect.anchoredPosition = localPoint;
    }
}
