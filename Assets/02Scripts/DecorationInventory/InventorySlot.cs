using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace DalbitCafe.Deco
{
    public class InventorySlot : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private Image iconImage;
        [SerializeField] private TextMeshProUGUI quantityText;
        [SerializeField] private Button slotButton;

        private InventoryItem inventoryItem;

        private void Start()
        {
            // 슬롯 버튼 클릭 이벤트 설정
            if (slotButton != null)
            {
                slotButton.onClick.AddListener(OnSlotClicked);
            }
        }

        /// <summary>
        /// 슬롯 설정
        /// </summary>
        /// <param name="item">표시할 인벤토리 아이템</param>
        public void SetupSlot(InventoryItem item)
        {
            inventoryItem = item;

            if (item == null || item.itemData == null)
            {
                Debug.LogWarning("[InventorySlot] 유효하지 않은 아이템이 슬롯에 설정되었습니다.");
                return;
            }

            // 아이콘 설정
            if (iconImage != null && item.itemData.icon != null)
            {
                iconImage.sprite = item.itemData.icon;
                iconImage.gameObject.SetActive(true);
            }
            else
            {
                if (iconImage != null)
                    iconImage.gameObject.SetActive(false);
            }

            // 수량 텍스트 설정
            if (quantityText != null)
            {
                if (item.quantity > 1)
                {
                    quantityText.text = $"x{item.quantity}";
                    quantityText.gameObject.SetActive(true);
                }
                else
                {
                    quantityText.gameObject.SetActive(false);
                }
            }

            Debug.Log($"[InventorySlot] 슬롯 설정됨: {item.itemData.itemName} (수량: {item.quantity})");
        }

        /// <summary>
        /// 슬롯이 클릭되었을 때 호출
        /// </summary>
        private void OnSlotClicked()
        {
            if (inventoryItem == null || inventoryItem.itemData == null)
            {
                Debug.LogWarning("[InventorySlot] 클릭된 슬롯에 유효한 아이템이 없습니다.");
                return;
            }

            // 배치할 수 있는 아이템인지 확인 (프리팹이 있는지)
            if (inventoryItem.itemData.prefab == null)
            {
                Debug.LogWarning($"[InventorySlot] {inventoryItem.itemData.itemName}은 배치할 수 없는 아이템입니다. (프리팹이 없음)");
                return;
            }

            // 수량이 0개 이하인 경우
            if (inventoryItem.quantity <= 0)
            {
                Debug.LogWarning($"[InventorySlot] {inventoryItem.itemData.itemName}의 수량이 부족합니다.");
                return;
            }

            Debug.Log($"[InventorySlot] 아이템 선택됨: {inventoryItem.itemData.itemName}");

            // 아이템 배치 시작
            StartItemPlacement();
        }

        /// <summary>
        /// 아이템 배치 시작
        /// </summary>
        private void StartItemPlacement()
        {
            // DecorateManager가 현재 다른 아이템을 배치 중인지 확인
            if (DecorateManager.Instance.targetItem != null &&
                DecorateManager.Instance.targetItem.IsPendingPlacement)
            {
                // 기존 배치 중인 아이템 취소
                DecorateManager.Instance.targetItem.CancelPendingPlacement();
                DecorateManager.Instance.targetItem = null;
                DecorateManager.Instance.DecorateUIElement.SetActive(false);
            }

            // 새 아이템 인스턴스 생성
            GameObject newItemObj = Instantiate(inventoryItem.itemData.prefab);
            DraggableItem draggableItem = newItemObj.GetComponent<DraggableItem>();

            if (draggableItem == null)
            {
                Debug.LogError($"[InventorySlot] {inventoryItem.itemData.itemName} 프리팹에 DraggableItem 컴포넌트가 없습니다!");
                Destroy(newItemObj);
                return;
            }

            // 아이템을 적절한 시작 위치에 배치 (예: 화면 중앙 또는 플레이어 근처)
            Vector3 startPosition = GetStartPosition();
            newItemObj.transform.position = startPosition;

            // DraggableItem을 배치 대기 상태로 설정
            draggableItem.StartPendingPlacement();

            // DecorateManager에 현재 타겟 아이템으로 설정
            DecorateManager.Instance.targetItem = draggableItem;

            // 배치 UI 활성화
            DecorateManager.Instance.DecorateUIElement.SetActive(true);

            Debug.Log($"[InventorySlot] {inventoryItem.itemData.itemName} 배치 시작");
        }

        /// <summary>
        /// 아이템 배치의 시작 위치 계산
        /// </summary>
        /// <returns>배치 시작 위치</returns>
        private Vector3 GetStartPosition()
        {
            // 카메라 중앙의 월드 좌표를 계산
            Camera mainCamera = Camera.main;
            if (mainCamera != null)
            {
                Vector3 screenCenter = new Vector3(Screen.width / 2, Screen.height / 2, 0);
                Vector3 worldCenter = mainCamera.ScreenToWorldPoint(new Vector3(screenCenter.x, screenCenter.y, mainCamera.nearClipPlane + 5f));
                return new Vector3(worldCenter.x, worldCenter.y, 0);
            }

            // 카메라가 없는 경우 원점 반환
            return Vector3.zero;
        }

        private void OnDestroy()
        {
            // 버튼 이벤트 해제
            if (slotButton != null)
            {
                slotButton.onClick.RemoveListener(OnSlotClicked);
            }
        }
    }
}