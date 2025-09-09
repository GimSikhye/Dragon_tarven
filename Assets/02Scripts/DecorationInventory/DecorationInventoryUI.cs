using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using TMPro;

namespace DalbitCafe.Deco
{
    public class DecorationInventoryUI : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private GameObject inventoryPanel;
        [SerializeField] private Button arrowButtonL;
        [SerializeField] private Button arrowButtonR;
        [SerializeField] private TextMeshProUGUI typeNumberText; // 타입넘버텍스트 -> 개수?
        [SerializeField] private Transform slotsParent;

        [Header("Slot Prefab")]
        [SerializeField] private GameObject slotPrefab; // 슬롯 프리팹

        [Header("Button Settings")]
        [SerializeField] private float buttonCooldownTime = 0.3f; // 버튼 쿨다운 시간 (초)

        // 현재 카테고리 및 서브카테고리 정보
        private List<CategoryGroup> categoryGroups = new List<CategoryGroup>();
        private int currentGroupIndex = 0;

        // 버튼 중복 클릭 방지를 위한 변수들
        private float lastButtonClickTime = 0f;
        private bool isProcessingButtonClick = false;

        // 현재 생성된 슬롯들 저장
        private List<InventorySlot> currentSlots = new List<InventorySlot>();

        private void Start()
        {
            InitializeCategoryGroups();
            SetupButtons();

            // 배치 모드일 때만 패널 활성화
            UpdatePanelVisibility();
        }

        private void Update()
        {
            // DecorateManager의 상태에 따라 패널 표시/숨김
            UpdatePanelVisibility();
        }

        /// <summary>
        /// 카테고리 그룹들을 초기화
        /// </summary>
        private void InitializeCategoryGroups()
        {
            categoryGroups.Clear();

            // Kitchen 카테고리의 모든 서브카테고리
            foreach (KitchenType kitchenType in System.Enum.GetValues(typeof(KitchenType)))
            {
                categoryGroups.Add(new CategoryGroup
                {
                    category = ItemCategory.Kitchen,
                    subCategory = kitchenType,
                    displayName = $"{GetKitchenTypeDisplayName(kitchenType)}"
                });
            }

            // Interior 카테고리의 모든 서브카테고리
            foreach (InteriorType interiorType in System.Enum.GetValues(typeof(InteriorType)))
            {
                categoryGroups.Add(new CategoryGroup
                {
                    category = ItemCategory.Interior,
                    subCategory = interiorType,
                    displayName = $"{GetInteriorTypeDisplayName(interiorType)}"
                });
            }

            // Exterior 카테고리의 모든 서브카테고리
            foreach (ExteriorType exteriorType in System.Enum.GetValues(typeof(ExteriorType)))
            {
                categoryGroups.Add(new CategoryGroup
                {
                    category = ItemCategory.Exterior,
                    subCategory = exteriorType,
                    displayName = $"{GetExteriorTypeDisplayName(exteriorType)}"
                });
            }

        }

        /// <summary>
        /// 버튼 이벤트 설정
        /// </summary>
        private void SetupButtons()
        {
            if (arrowButtonL != null)
                arrowButtonL.onClick.AddListener(OnPreviousCategory);

            if (arrowButtonR != null)
                arrowButtonR.onClick.AddListener(OnNextCategory);
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
                Debug.Log($"[DecorationInventoryUI] 버튼 클릭 무시 - 쿨다운 중 (경과시간: {currentTime - lastButtonClickTime:F2}초)");
                return false;
            }

            lastButtonClickTime = currentTime;
            return true;
        }

        /// <summary>
        /// 패널 표시 상태 업데이트
        /// </summary>
        private void UpdatePanelVisibility()
        {
            //Debug.Log($"UpdatePanelVisibility 호출됨");

            if (DecorateManager.Instance != null)
            {
                bool shouldShow = DecorateManager.Instance.IsDecorateMode;
                bool wasShowing = inventoryPanel.activeSelf;

                //Debug.Log($"DecorateMode: {shouldShow}, 현재 패널 상태: {inventoryPanel.activeSelf}");

                if (inventoryPanel.activeSelf != shouldShow)
                {
                    //Debug.Log("패널 표시 상태 업데이트");
                    inventoryPanel.SetActive(shouldShow);

                    if (shouldShow)
                    {
                        RefreshCurrentCategory();
                    }
                    else
                    {
                        // 배치모드가 종료될 때 모든 슬롯의 배치 상태 취소
                        OnDecorateModeExited();
                    }
                }
                else
                {
                    //Debug.Log("패널 상태 변경 불필요");
                }
            }
            else
            {
                //Debug.Log("DecorateManager.Instance가 null입니다");
            }
        }

        /// <summary>
        /// 배치모드 종료 시 모든 슬롯의 배치 상태 정리
        /// </summary>
        private void OnDecorateModeExited()
        {
            //Debug.Log("[DecorationInventoryUI] 배치모드 종료 - 모든 배치 중인 아이템 취소");

            // 현재 배치 중인 아이템이 있다면 취소
            if (DecorateManager.Instance.targetItem != null &&
                DecorateManager.Instance.targetItem.IsPendingPlacement)
            {
                var currentItem = DecorateManager.Instance.targetItem;

                // 아이템 취소 (신규 아이템인 경우 파괴됨)
                currentItem.CancelPendingPlacement();

                // 타겟 아이템 해제
                DecorateManager.Instance.targetItem = null;

                // UI 비활성화
                DecorateManager.Instance.DecorateUIElement.SetActive(false);
            }

            // 모든 슬롯의 배치 상태 해제
            foreach (var slot in currentSlots)
            {
                if (slot != null)
                {
                    slot.OnDecorateModeExited();
                }
            }
        }

        /// <summary>
        /// 이전 카테고리로 이동
        /// </summary>
        private void OnPreviousCategory()
        {
            if (!IsButtonClickValid()) return;

            isProcessingButtonClick = true;

            try
            {
                currentGroupIndex--;
                if (currentGroupIndex < 0)
                {
                    currentGroupIndex = categoryGroups.Count - 1; // 마지막으로 이동
                }
                RefreshCurrentCategory();
                Debug.Log($"[DecorationInventoryUI] 이전 카테고리로 이동: {currentGroupIndex}");
            }
            finally
            {
                isProcessingButtonClick = false;
            }
        }

        /// <summary>
        /// 다음 카테고리로 이동
        /// </summary>
        private void OnNextCategory()
        {
            if (!IsButtonClickValid()) return;

            isProcessingButtonClick = true;

            try
            {
                currentGroupIndex++;
                if (currentGroupIndex >= categoryGroups.Count)
                {
                    currentGroupIndex = 0; // 처음으로 이동
                }
                RefreshCurrentCategory();
                Debug.Log($"[DecorationInventoryUI] 다음 카테고리로 이동: {currentGroupIndex}");
            }
            finally
            {
                isProcessingButtonClick = false;
            }
        }

        /// <summary>
        /// 현재 카테고리의 아이템들을 표시
        /// </summary>
        private void RefreshCurrentCategory()
        {
            if (categoryGroups.Count == 0) return;

            var currentGroup = categoryGroups[currentGroupIndex];

            // 타입 표시 텍스트 업데이트
            if (typeNumberText != null)
            {
                typeNumberText.text = currentGroup.displayName;
            }

            // 기존 슬롯들 제거
            ClearSlots();

            // 현재 카테고리의 아이템들 가져오기
            var items = GetItemsForCurrentCategory(currentGroup);

            // 슬롯 생성
            CreateSlots(items);

            Debug.Log($"[DecorationInventoryUI] {currentGroup.displayName} 카테고리 표시 (아이템 수: {items.Count})");
        }

        /// <summary>
        /// 현재 카테고리에 해당하는 아이템들 가져오기
        /// </summary>
        private List<InventoryItem> GetItemsForCurrentCategory(CategoryGroup group)
        {
            if (Inventory.Instance == null) return new List<InventoryItem>();

            // 전체 카테고리의 아이템들 가져오기
            var categoryItems = Inventory.Instance.GetItemsByCategory(group.category);

            // 서브카테고리로 필터링
            var filteredItems = categoryItems.Where(item =>
            {
                if (item.itemData.SubCategory == null) return false;
                return item.itemData.SubCategory.Equals(group.subCategory);
            }).ToList();

            return filteredItems;
        }

        /// <summary>
        /// 기존 슬롯들 제거
        /// </summary>
        private void ClearSlots()
        {
            // 현재 슬롯들의 배치 상태 정리
            foreach (var slot in currentSlots)
            {
                if (slot != null)
                {
                    slot.OnDecorateModeExited();
                }
            }

            // 슬롯 리스트 초기화
            currentSlots.Clear();

            // 슬롯 오브젝트들 파괴
            foreach (Transform child in slotsParent)
            {
                Destroy(child.gameObject);
            }
        }

        /// <summary>
        /// 아이템 슬롯들 생성
        /// </summary>
        private void CreateSlots(List<InventoryItem> items)
        {
            foreach (var item in items)
            {
                // 수량이 0인 아이템은 슬롯을 생성하지 않음
                if (item.quantity <= 0) continue;

                GameObject slotObj = Instantiate(slotPrefab, slotsParent);
                InventorySlot slot = slotObj.GetComponent<InventorySlot>();

                if (slot != null)
                {
                    slot.SetupSlot(item);
                    // 슬롯 제거 이벤트 구독
                    slot.OnSlotShouldBeRemoved += OnSlotRemovalRequested;

                    // 슬롯 리스트에 추가
                    currentSlots.Add(slot);
                }
                else
                {
                    Debug.LogError("[DecorationInventoryUI] 슬롯 프리팹에 InventorySlot 컴포넌트가 없습니다!");
                }
            }
        }

        /// <summary>
        /// 슬롯 제거 요청 처리
        /// </summary>
        private void OnSlotRemovalRequested(InventorySlot slot)
        {
            if (slot != null)
            {
                Debug.Log($"[DecorationInventoryUI] 슬롯 제거: {slot.name}");

                // 슬롯 리스트에서 제거
                currentSlots.Remove(slot);

                // 이벤트 구독 해제
                slot.OnSlotShouldBeRemoved -= OnSlotRemovalRequested;

                // 슬롯 오브젝트 파괴
                Destroy(slot.gameObject);
            }
        }

        /// <summary>
        /// Kitchen 타입 표시 이름 가져오기
        /// </summary>
        private string GetKitchenTypeDisplayName(KitchenType type)
        {
            return type switch
            {
                KitchenType.RoastingMachine => "로스팅머신",
                KitchenType.CoffeeMachine => "커피머신",
                KitchenType.Workbench => "작업대",
                KitchenType.CookingMachine => "쿠킹머신",
                KitchenType.Showcase => "쇼케이스",
                KitchenType.Counter => "계산대",
                KitchenType.Mixer => "믹서기",
                _ => type.ToString()
            };
        }

        /// <summary>
        /// Interior 타입 표시 이름 가져오기
        /// </summary>
        private string GetInteriorTypeDisplayName(InteriorType type)
        {
            return type switch
            {
                InteriorType.Table => "테이블",
                InteriorType.Chair => "의자",
                InteriorType.Partition => "파티션",
                InteriorType.Decoration => "장식품",
                InteriorType.BeanContainer => "원두통",
                InteriorType.WallDecoration => "벽장식",
                InteriorType.Tile => "타일",
                InteriorType.Wallpaper => "벽지",
                _ => type.ToString()
            };
        }

        /// <summary>
        /// Exterior 타입 표시 이름 가져오기
        /// </summary>
        private string GetExteriorTypeDisplayName(ExteriorType type)
        {
            return type switch
            {
                ExteriorType.SecondFloorOnly => "2층전용",
                ExteriorType.OutdoorDecoration => "야외장식품",
                ExteriorType.WallExteriorDecoration => "건물외벽장식",
                ExteriorType.Railing2F => "2층난간",
                ExteriorType.Stair2F => "2층계단",
                ExteriorType.WallExterior => "건물외벽",
                ExteriorType.Entrance => "입구",
                _ => type.ToString()
            };
        }
    }

    /// <summary>
    /// 카테고리 그룹 정보를 담는 클래스
    /// </summary>
    [System.Serializable]
    public class CategoryGroup
    {
        public ItemCategory category;
        public System.Enum subCategory;
        public string displayName;
    }
}