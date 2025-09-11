using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class ShowcaseItemUI : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI quantityText;
    [SerializeField] private Vector2 tooltipOffset = new Vector2(20f, -20f); // 기본 offset

    private InventoryItem inventoryItem;
    private TooltipUI tooltip;

    public void Setup(InventoryItem inventoryItem)
    {
        this.inventoryItem = inventoryItem;

        if (inventoryItem.itemData is FoodItemData foodItem)
        {
            iconImage.sprite = foodItem.icon;
            quantityText.text = $"x{inventoryItem.quantity}";
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (inventoryItem == null) return;

        if (tooltip == null)
            tooltip = FindAnyObjectByType<TooltipUI>();

        if (tooltip == null || tooltip.tooltipObject == null)
        {
            Debug.LogError("[ShowcaseItemUI] TooltipUI 또는 tooltipObject가 할당되지 않았습니다!");
            return;
        }

        tooltip.tooltipObject.SetActive(true);
        tooltip.SetData(inventoryItem.itemData.itemName, inventoryItem.itemData.price);

        PositionTooltip();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (tooltip != null && tooltip.tooltipObject != null)
            tooltip.tooltipObject.SetActive(false);
    }

    private void PositionTooltip()
    {
        RectTransform slotRect = transform as RectTransform;
        RectTransform tooltipRect = tooltip.tooltipObject.transform as RectTransform;
        Canvas canvas = tooltipRect.GetComponentInParent<Canvas>();
        RectTransform canvasRect = canvas.transform as RectTransform;

        // 슬롯의 우하단 월드 좌표
        Vector3[] corners = new Vector3[4];
        slotRect.GetWorldCorners(corners);
        Vector3 targetWorldPos = corners[3];

        // 월드 → 스크린 → 로컬(Canvas 좌표)
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            RectTransformUtility.WorldToScreenPoint(null, targetWorldPos),
            canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera,
            out localPoint);

        // offset 적용
        localPoint += tooltipOffset;
        tooltipRect.anchoredPosition = localPoint;

        // 오른쪽 화면 밖이면 좌하단으로 이동
        if (tooltipRect.anchoredPosition.x + tooltipRect.rect.width > canvasRect.rect.width / 2f)
        {
            targetWorldPos = corners[2]; // 좌하단
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvasRect,
                RectTransformUtility.WorldToScreenPoint(null, targetWorldPos),
                canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera,
                out localPoint);

            localPoint += new Vector2(-tooltipOffset.x, tooltipOffset.y);
            tooltipRect.anchoredPosition = localPoint;
        }
    }
}
