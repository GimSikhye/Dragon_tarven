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
        [SerializeField] private GameObject checkmarkIndicator; // 체크 표시 UI 요소

        [Header("Button Settings")]
        [SerializeField] private float buttonCooldownTime = 0.3f;

        private InventoryItem inventoryItem;

        // 버튼 중복 클릭 방지를 위한 변수들
        private float lastButtonClickTime = 0f;
        private bool isProcessingButtonClick = false;

        // 슬롯 제거를 위한 델리게이트
        public System.Action<InventorySlot> OnSlotShouldBeRemoved;

        // 배치 상태 관리
        private bool _isCurrentlyPlacing = false; // 현재 이 슬롯의 아이템이 배치 중인지

        public bool IsCurrentlyPlacing => _isCurrentlyPlacing;

        private void Start()
        {
            // 슬롯 버튼 클릭 이벤트 설정
            if (slotButton != null)
            {
                slotButton.onClick.AddListener(OnSlotClicked);
            }

            // 체크마크 초기 상태 비활성화
            if (checkmarkIndicator != null)
            {
                checkmarkIndicator.SetActive(false);
            }
        }

        /// <summary>
        /// 버튼 클릭이 유효한지 확인
        /// </summary>
        private bool IsButtonClickValid()
        {
            float currentTime = Time.time;

            // 이미 처리 중이거나 쿨다운 시간 내에 클릭된 경우 무시
            if (isProcessingButtonClick || (currentTime - lastButtonClickTime) < buttonCooldownTime)
            {
                Debug.Log($"[InventorySlot] 버튼 클릭 무시 - 쿨다운 중 (경과시간: {currentTime - lastButtonClickTime:F2}초)");
                return false;
            }

            lastButtonClickTime = currentTime;
            return true;
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
                if (iconImage != null) // 아이콘이 null 이라면
                    iconImage.gameObject.SetActive(false);
            }

            // 수량 텍스트 설정
            UpdateQuantityDisplay();

            Debug.Log($"[InventorySlot] 슬롯 설정됨: {item.itemData.itemName} (수량: {item.quantity})");
        }

        /// <summary>
        /// 수량 표시 업데이트
        /// </summary>
        private void UpdateQuantityDisplay()
        {
            if (quantityText != null && inventoryItem != null)
            {
                if (inventoryItem.quantity >= 1)
                {
                    quantityText.text = $"x{inventoryItem.quantity}";
                    quantityText.gameObject.SetActive(true);
                }
                else
                {
                    quantityText.gameObject.SetActive(false);
                    // 수량이 0이면 슬롯 제거 요청
                    RequestSlotRemoval();
                }
            }
        }

        /// <summary>
        /// 슬롯 제거 요청
        /// </summary>
        private void RequestSlotRemoval()
        {
            Debug.Log($"[InventorySlot] 수량이 0이 된 슬롯 제거 요청: {inventoryItem?.itemData?.itemName}");
            OnSlotShouldBeRemoved?.Invoke(this);
        }

        /// <summary>
        /// 슬롯이 클릭되었을 때 호출
        /// </summary>
        private void OnSlotClicked()
        {
            if (!IsButtonClickValid()) return;

            isProcessingButtonClick = true;

            try
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

                // 기존에 배치 중인 아이템이 있다면 취소 (단, 같은 슬롯이면 새로 생성하지 않음)
                CancelAnyExistingPlacement();

                // CancelAnyExistingPlacement에서 같은 슬롯 아이템이 이미 배치 중이라 return된 경우
                if (DecorateManager.Instance.targetItem != null &&
                    DecorateManager.Instance.targetItem.sourceSlot == this)
                {
                    return; // 새로 생성하지 않고 종료
                }

                // 현재 슬롯을 배치 중 상태로 설정
                SetPlacingState(true);

                // 아이템 배치 시작 (수량 차감하지 않음)
                StartItemPlacement();
            }
            finally
            {
                isProcessingButtonClick = false;
            }
        }

        /// <summary>
        /// 기존 배치 중인 아이템 취소
        /// </summary>
        private void CancelAnyExistingPlacement()
        {
            // DecorateManager가 현재 다른 아이템을 배치 중인지 확인
            if (DecorateManager.Instance.targetItem != null &&
                DecorateManager.Instance.targetItem.IsPendingPlacement)
            {
                var previousItem = DecorateManager.Instance.targetItem;

                // ★ 중요: 다른 슬롯의 아이템인 경우에만 취소
                if (previousItem.sourceSlot != null && previousItem.sourceSlot != this)
                {
                    // 기존 배치 중인 아이템 취소
                    previousItem.CancelPendingPlacement();
                    DecorateManager.Instance.targetItem = null;
                    DecorateManager.Instance.DecorateUIElement.SetActive(false);

                    // 이전 슬롯의 배치 상태 해제
                    previousItem.sourceSlot.SetPlacingState(false);
                }
                // 같은 슬롯의 아이템이면 새로 생성하지 않고 기존 아이템 유지
                else if (previousItem.sourceSlot == this)
                {
                    Debug.Log("[InventorySlot] 이미 이 슬롯의 아이템이 배치 중입니다. 새로 생성하지 않습니다.");
                    return; // 새 아이템 생성하지 않고 종료
                }
            }

            // 다른 모든 슬롯의 배치 상태 해제 (현재 슬롯 제외)
            var allSlots = FindObjectsOfType<InventorySlot>();
            foreach (var slot in allSlots)
            {
                if (slot != this)
                {
                    slot.SetPlacingState(false);
                }
            }
        }

        /// <summary>
        /// 배치 상태 설정
        /// </summary>
        /// <param name="isPlacing">배치 중인지 여부</param>
        public void SetPlacingState(bool isPlacing)
        {
            _isCurrentlyPlacing = isPlacing;

            if (checkmarkIndicator != null)
            {
                checkmarkIndicator.SetActive(isPlacing);
            }

            Debug.Log($"[InventorySlot] {inventoryItem?.itemData?.itemName} 배치 상태: {isPlacing}");
        }

        /// <summary>
        /// 아이템 배치가 취소되었을 때 수량 복구
        /// </summary>
        public void RestoreItemQuantity()
        {
            if (inventoryItem != null)
            {
                inventoryItem.quantity++;
                Debug.Log($"[InventorySlot] {inventoryItem.itemData.itemName} 수량 복구: {inventoryItem.quantity - 1} -> {inventoryItem.quantity}");
                
                // 수량 표시 업데이트
                UpdateQuantityDisplay();
            }
        }

        /// <summary>
        /// 아이템 배치 시작 (수량 차감하지 않음)
        /// </summary>
        private void StartItemPlacement()
        {
            DecorateManager.Instance.RequestPlacement(inventoryItem, this);
        }

        /// <summary>
        /// 아이템 배치 취소 시 (수량 복구할 필요 없음)
        /// </summary>
        public void OnItemPlacementCancelled()
        {
            Debug.Log($"[InventorySlot] {inventoryItem?.itemData?.itemName} 배치 취소 (수량 복구 필요 없음)");

            // 배치 상태 해제
            SetPlacingState(false);
        }

        /// <summary>
        /// 배치모드 종료 시 호출 (모든 배치 중인 아이템 취소)
        /// </summary>
        public void OnDecorateModeExited()
        {
            if (_isCurrentlyPlacing)
            {
                Debug.Log($"[InventorySlot] 배치모드 종료로 인한 배치 취소: {inventoryItem?.itemData?.itemName}");
                SetPlacingState(false);
            }
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

        public void OnItemPlacementConfirmed()
        {
            if (inventoryItem != null && inventoryItem.quantity > 0)
            {
                int oldQuantity = inventoryItem.quantity;
                inventoryItem.quantity--;

                // 인벤토리에서도 수량 업데이트
                if (Inventory.Instance != null)
                {
                    Inventory.Instance.UpdateItemQuantity(inventoryItem.itemData, -1);
                }

                UpdateQuantityDisplay();
                SetPlacingState(false);
            }
        }


        /// <summary>
        /// 현재 아이템의 수량을 반환
        /// </summary>
        public int GetCurrentQuantity()
        {
            return inventoryItem?.quantity ?? 0;
        }

        /// <summary>
        /// 슬롯이 비어있는지 확인
        /// </summary>
        public bool IsEmpty()
        {
            return inventoryItem == null || inventoryItem.quantity <= 0;
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