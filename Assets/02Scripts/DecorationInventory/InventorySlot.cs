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
            // ���� ��ư Ŭ�� �̺�Ʈ ����
            if (slotButton != null)
            {
                slotButton.onClick.AddListener(OnSlotClicked);
            }
        }

        /// <summary>
        /// ���� ����
        /// </summary>
        /// <param name="item">ǥ���� �κ��丮 ������</param>
        public void SetupSlot(InventoryItem item)
        {
            inventoryItem = item;

            if (item == null || item.itemData == null)
            {
                Debug.LogWarning("[InventorySlot] ��ȿ���� ���� �������� ���Կ� �����Ǿ����ϴ�.");
                return;
            }

            // ������ ����
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

            // ���� �ؽ�Ʈ ����
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

            Debug.Log($"[InventorySlot] ���� ������: {item.itemData.itemName} (����: {item.quantity})");
        }

        /// <summary>
        /// ������ Ŭ���Ǿ��� �� ȣ��
        /// </summary>
        private void OnSlotClicked()
        {
            if (inventoryItem == null || inventoryItem.itemData == null)
            {
                Debug.LogWarning("[InventorySlot] Ŭ���� ���Կ� ��ȿ�� �������� �����ϴ�.");
                return;
            }

            // ��ġ�� �� �ִ� ���������� Ȯ�� (�������� �ִ���)
            if (inventoryItem.itemData.prefab == null)
            {
                Debug.LogWarning($"[InventorySlot] {inventoryItem.itemData.itemName}�� ��ġ�� �� ���� �������Դϴ�. (�������� ����)");
                return;
            }

            // ������ 0�� ������ ���
            if (inventoryItem.quantity <= 0)
            {
                Debug.LogWarning($"[InventorySlot] {inventoryItem.itemData.itemName}�� ������ �����մϴ�.");
                return;
            }

            Debug.Log($"[InventorySlot] ������ ���õ�: {inventoryItem.itemData.itemName}");

            // ������ ��ġ ����
            StartItemPlacement();
        }

        /// <summary>
        /// ������ ��ġ ����
        /// </summary>
        private void StartItemPlacement()
        {
            // DecorateManager�� ���� �ٸ� �������� ��ġ ������ Ȯ��
            if (DecorateManager.Instance.targetItem != null &&
                DecorateManager.Instance.targetItem.IsPendingPlacement)
            {
                // ���� ��ġ ���� ������ ���
                DecorateManager.Instance.targetItem.CancelPendingPlacement();
                DecorateManager.Instance.targetItem = null;
                DecorateManager.Instance.DecorateUIElement.SetActive(false);
            }

            // �� ������ �ν��Ͻ� ����
            GameObject newItemObj = Instantiate(inventoryItem.itemData.prefab);
            DraggableItem draggableItem = newItemObj.GetComponent<DraggableItem>();

            if (draggableItem == null)
            {
                Debug.LogError($"[InventorySlot] {inventoryItem.itemData.itemName} �����տ� DraggableItem ������Ʈ�� �����ϴ�!");
                Destroy(newItemObj);
                return;
            }

            // �������� ������ ���� ��ġ�� ��ġ (��: ȭ�� �߾� �Ǵ� �÷��̾� ��ó)
            Vector3 startPosition = GetStartPosition();
            newItemObj.transform.position = startPosition;

            // DraggableItem�� ��ġ ��� ���·� ����
            draggableItem.StartPendingPlacement();

            // DecorateManager�� ���� Ÿ�� ���������� ����
            DecorateManager.Instance.targetItem = draggableItem;

            // ��ġ UI Ȱ��ȭ
            DecorateManager.Instance.DecorateUIElement.SetActive(true);

            Debug.Log($"[InventorySlot] {inventoryItem.itemData.itemName} ��ġ ����");
        }

        /// <summary>
        /// ������ ��ġ�� ���� ��ġ ���
        /// </summary>
        /// <returns>��ġ ���� ��ġ</returns>
        private Vector3 GetStartPosition()
        {
            // ī�޶� �߾��� ���� ��ǥ�� ���
            Camera mainCamera = Camera.main;
            if (mainCamera != null)
            {
                Vector3 screenCenter = new Vector3(Screen.width / 2, Screen.height / 2, 0);
                Vector3 worldCenter = mainCamera.ScreenToWorldPoint(new Vector3(screenCenter.x, screenCenter.y, mainCamera.nearClipPlane + 5f));
                return new Vector3(worldCenter.x, worldCenter.y, 0);
            }

            // ī�޶� ���� ��� ���� ��ȯ
            return Vector3.zero;
        }

        private void OnDestroy()
        {
            // ��ư �̺�Ʈ ����
            if (slotButton != null)
            {
                slotButton.onClick.RemoveListener(OnSlotClicked);
            }
        }
    }
}