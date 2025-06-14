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
        [SerializeField] private GameObject slotPrefab; // ���� ������

        // ���� ī�װ� �� ����ī�װ� ����
        private List<CategoryGroup> categoryGroups = new List<CategoryGroup>();
        private int currentGroupIndex = 0;

        private void Start()
        {
            InitializeCategoryGroups();
            SetupButtons();

            // ��ġ ����� ���� �г� Ȱ��ȭ
            UpdatePanelVisibility();
        }

        private void Update()
        {
            // DecorateManager�� ���¿� ���� �г� ǥ��/����
            UpdatePanelVisibility();
        }

        /// <summary>
        /// ī�װ� �׷���� �ʱ�ȭ
        /// </summary>
        private void InitializeCategoryGroups()
        {
            categoryGroups.Clear();

            // Kitchen ī�װ��� ��� ����ī�װ�
            foreach (KitchenType kitchenType in System.Enum.GetValues(typeof(KitchenType)))
            {
                categoryGroups.Add(new CategoryGroup
                {
                    category = ItemCategory.Kitchen,
                    subCategory = kitchenType,
                    displayName = $"�ֹ� - {GetKitchenTypeDisplayName(kitchenType)}"
                });
            }

            // Interior ī�װ��� ��� ����ī�װ�
            foreach (InteriorType interiorType in System.Enum.GetValues(typeof(InteriorType)))
            {
                categoryGroups.Add(new CategoryGroup
                {
                    category = ItemCategory.Interior,
                    subCategory = interiorType,
                    displayName = $"���׸��� - {GetInteriorTypeDisplayName(interiorType)}"
                });
            }

            // Exterior ī�װ��� ��� ����ī�װ�
            foreach (ExteriorType exteriorType in System.Enum.GetValues(typeof(ExteriorType)))
            {
                categoryGroups.Add(new CategoryGroup
                {
                    category = ItemCategory.Exterior,
                    subCategory = exteriorType,
                    displayName = $"�ͽ��׸��� - {GetExteriorTypeDisplayName(exteriorType)}"
                });
            }

            Debug.Log($"[DecorationInventoryUI] �� {categoryGroups.Count}�� ī�װ� �׷� �ʱ�ȭ��");
        }

        /// <summary>
        /// ��ư �̺�Ʈ ����
        /// </summary>
        private void SetupButtons()
        {
            if (arrowButtonL != null)
                arrowButtonL.onClick.AddListener(OnPreviousCategory);

            if (arrowButtonR != null)
                arrowButtonR.onClick.AddListener(OnNextCategory);
        }

        /// <summary>
        /// �г� ǥ�� ���� ������Ʈ
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
                        // ��ġ ��尡 Ȱ��ȭ�� �� ù ��° ī�װ� ǥ��
                        RefreshCurrentCategory();
                    }
                }
            }
        }

        /// <summary>
        /// ���� ī�װ��� �̵�
        /// </summary>
        private void OnPreviousCategory()
        {
            currentGroupIndex--;
            if (currentGroupIndex < 0)
            {
                currentGroupIndex = categoryGroups.Count - 1; // ���������� �̵�
            }
            RefreshCurrentCategory();
        }

        /// <summary>
        /// ���� ī�װ��� �̵�
        /// </summary>
        private void OnNextCategory()
        {
            currentGroupIndex++;
            if (currentGroupIndex >= categoryGroups.Count)
            {
                currentGroupIndex = 0; // ó������ �̵�
            }
            RefreshCurrentCategory();
        }

        /// <summary>
        /// ���� ī�װ��� �����۵��� ǥ��
        /// </summary>
        private void RefreshCurrentCategory()
        {
            if (categoryGroups.Count == 0) return;

            var currentGroup = categoryGroups[currentGroupIndex];

            // Ÿ�� ǥ�� �ؽ�Ʈ ������Ʈ
            if (typeNumberText != null)
            {
                typeNumberText.text = currentGroup.displayName;
            }

            // ���� ���Ե� ����
            ClearSlots();

            // ���� ī�װ��� �����۵� ��������
            var items = GetItemsForCurrentCategory(currentGroup);

            // ���� ����
            CreateSlots(items);

            Debug.Log($"[DecorationInventoryUI] {currentGroup.displayName} ī�װ� ǥ�� (������ ��: {items.Count})");
        }

        /// <summary>
        /// ���� ī�װ��� �ش��ϴ� �����۵� ��������
        /// </summary>
        private List<InventoryItem> GetItemsForCurrentCategory(CategoryGroup group)
        {
            if (Inventory.Instance == null) return new List<InventoryItem>();

            // ��ü ī�װ��� �����۵� ��������
            var categoryItems = Inventory.Instance.GetItemsByCategory(group.category);

            // ����ī�װ��� ���͸�
            var filteredItems = categoryItems.Where(item =>
            {
                if (item.itemData.SubCategory == null) return false;
                return item.itemData.SubCategory.Equals(group.subCategory);
            }).ToList();

            return filteredItems;
        }

        /// <summary>
        /// ���� ���Ե� ����
        /// </summary>
        private void ClearSlots()
        {
            foreach (Transform child in slotsParent)
            {
                Destroy(child.gameObject);
            }
        }

        /// <summary>
        /// ������ ���Ե� ����
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
                    Debug.LogError("[DecorationInventoryUI] ���� �����տ� InventorySlot ������Ʈ�� �����ϴ�!");
                }
            }
        }

        /// <summary>
        /// Kitchen Ÿ�� ǥ�� �̸� ��������
        /// </summary>
        private string GetKitchenTypeDisplayName(KitchenType type)
        {
            return type switch
            {
                KitchenType.RoastingMachine => "�ν��øӽ�",
                KitchenType.CoffeeMachine => "Ŀ�Ǹӽ�",
                KitchenType.Workbench => "�۾���",
                KitchenType.CookingMachine => "��ŷ�ӽ�",
                KitchenType.Showcase => "�����̽�",
                KitchenType.Counter => "����",
                KitchenType.Mixer => "�ͼ���",
                _ => type.ToString()
            };
        }

        /// <summary>
        /// Interior Ÿ�� ǥ�� �̸� ��������
        /// </summary>
        private string GetInteriorTypeDisplayName(InteriorType type)
        {
            return type switch
            {
                InteriorType.Table => "���̺�",
                InteriorType.Chair => "����",
                InteriorType.Partition => "��Ƽ��",
                InteriorType.Decoration => "���ǰ",
                InteriorType.BeanContainer => "������",
                InteriorType.WallDecoration => "�����",
                InteriorType.Tile => "Ÿ��",
                InteriorType.Wallpaper => "����",
                _ => type.ToString()
            };
        }

        /// <summary>
        /// Exterior Ÿ�� ǥ�� �̸� ��������
        /// </summary>
        private string GetExteriorTypeDisplayName(ExteriorType type)
        {
            return type switch
            {
                ExteriorType.SecondFloorOnly => "2������",
                ExteriorType.OutdoorDecoration => "�߿����ǰ",
                ExteriorType.WallExteriorDecoration => "�ǹ��ܺ����",
                ExteriorType.Railing2F => "2������",
                ExteriorType.Stair2F => "2�����",
                ExteriorType.WallExterior => "�ǹ��ܺ�",
                ExteriorType.Entrance => "�Ա�",
                _ => type.ToString()
            };
        }
    }

    /// <summary>
    /// ī�װ� �׷� ������ ��� Ŭ����
    /// </summary>
    [System.Serializable]
    public class CategoryGroup
    {
        public ItemCategory category;
        public System.Enum subCategory;
        public string displayName;
    }
}