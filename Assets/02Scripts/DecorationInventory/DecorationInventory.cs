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
        [SerializeField] private TextMeshProUGUI typeNumberText;
        [SerializeField] private Transform slotsParent;

        [Header("Slot Prefab")]
        [SerializeField] private GameObject slotPrefab; // 슬롯 프리팹

        // 현재 카테고리 및 서브카테고리 정보
        private List<CategoryGroup> categoryGroups = new List<CategoryGroup>();
        private int currentGroupIndex = 0;

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
                    displayName = $"주방 - {GetKitchenTypeDisplayName(kitchenType)}"
                });
            }

            // Interior 카테고리의 모든 서브카테고리
            foreach (InteriorType interiorType in System.Enum.GetValues(typeof(InteriorType)))
            {
                categoryGroups.Add(new CategoryGroup
                {
                    category = ItemCategory.Interior,
                    subCategory = interiorType,
                    displayName = $"인테리어 - {GetInteriorTypeDisplayName(interiorType)}"
                });
            }

            // Exterior 카테고리의 모든 서브카테고리
            foreach (ExteriorType exteriorType in System.Enum.GetValues(typeof(ExteriorType)))
            {
                categoryGroups.Add(new CategoryGroup
                {
                    category = ItemCategory.Exterior,
                    subCategory = exteriorType,
                    displayName = $"익스테리어 - {GetExteriorTypeDisplayName(exteriorType)}"
                });
            }

            Debug.Log($"[DecorationInventoryUI] 총 {categoryGroups.Count}개 카테고리 그룹 초기화됨");
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
        /// 패널 표시 상태 업데이트
        /// </summary>
        private void UpdatePanelVisibility()
        {
            if (DecorateManager.Instance != null)
            {
                bool shouldShow = DecorateManager.Instance.IsDecorateMode;
                if (inventoryPanel.activeSelf != shouldShow)
                {
                    inventoryPanel.SetActive(shouldShow);

                    if (shouldShow)
                    {
                        // 배치 모드가 활성화될 때 첫 번째 카테고리 표시
                        RefreshCurrentCategory();
                    }
                }
            }
        }

        /// <summary>
        /// 이전 카테고리로 이동
        /// </summary>
        private void OnPreviousCategory()
        {
            currentGroupIndex--;
            if (currentGroupIndex < 0)
            {
                currentGroupIndex = categoryGroups.Count - 1; // 마지막으로 이동
            }
            RefreshCurrentCategory();
        }

        /// <summary>
        /// 다음 카테고리로 이동
        /// </summary>
        private void OnNextCategory()
        {
            currentGroupIndex++;
            if (currentGroupIndex >= categoryGroups.Count)
            {
                currentGroupIndex = 0; // 처음으로 이동
            }
            RefreshCurrentCategory();
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
                GameObject slotObj = Instantiate(slotPrefab, slotsParent);
                InventorySlot slot = slotObj.GetComponent<InventorySlot>();

                if (slot != null)
                {
                    slot.SetupSlot(item);
                }
                else
                {
                    Debug.LogError("[DecorationInventoryUI] 슬롯 프리팹에 InventorySlot 컴포넌트가 없습니다!");
                }
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