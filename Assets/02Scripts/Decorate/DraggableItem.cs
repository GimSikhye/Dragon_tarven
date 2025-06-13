using DalbitCafe.Deco;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

namespace DalbitCafe.Deco
{
    public class DraggableItem : MonoBehaviour, IPointerDownHandler, IDragHandler, IBeginDragHandler, IEndDragHandler
    {
        [Header("������ ȸ��")]
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Sprite[] directionSprites; // 0: ������ �Ʒ�, 1: ���� �Ʒ�, 2: ������, 3: ������ ��
        private int _rotationIndex = 0; // 0, 1, 2, 3 �� 0~3 ���̿��� ȸ�� ���� �ε���

        [Header("������ ȸ�� ����")]
        [SerializeField] private int rotationLimit = 4; // ȸ�� ������ ���� �� : 2(�¿�), 4(��ü)

        [Header("������ ��ġ")]
        private Vector3 _initialPosition; // �巡�� ���� �� ������ ��ġ
        private bool _isDragging = false; // �������� �巡�� ������ �˻�
        public Vector2Int _itemSize;  // �������� �����ϴ� ũ�� (��: 1x1, 2x1 ��)
        private Vector2Int _originalGridPosition; // ���� �׸��� ��ġ ����

        // ��ġ Ȯ�� �ý����� ���� ������
        private bool _isPendingPlacement = false; // ��ġ ��� ������
        private Vector3 _pendingPosition; // ��ġ ��� ���� ��ġ
        private Vector2Int _pendingGridPosition; // ��ġ ��� ���� �׸��� ��ġ
        private bool _canPlaceAtPendingPosition = false; // ��� ���� ��ġ�� ��ġ ��������

        [Header("�ƿ����� ȿ��")]
        [SerializeField] private Material greenOutlineMaterial; // ��ġ ������ ��ġ�� ��Ƽ����
        [SerializeField] private Material redOutlineMaterial; // ��ġ �Ұ����� ��ġ�� ��Ƽ����
        private Material _originalMaterial; // ���� ��Ƽ���� ����

        [Header("UI ��������Ʈ ����")]
        [SerializeField] private Sprite confirmActiveSprite; // ��ġ ������ �� ����� ��������Ʈ
        [SerializeField] private Sprite confirmDeactiveSprite; // ��ġ �Ұ����� �� ����� ��������Ʈ 

        [Header("����")]
        public ItemData itemData; // Inspector�� ���� �ʿ�

        // ������Ƽ��
        public bool IsOccupied { get; private set; } = false; // ��� ������
        public bool IsDragging => _isDragging;
        public bool IsPendingPlacement => _isPendingPlacement; // ��ġ ��� ������
        public Vector2Int ItemSize => _itemSize;
        public int RotationIndex => _rotationIndex;
        public int RotationLimit => rotationLimit;
        public Vector3 InitialPosition => _initialPosition;

        private Tilemap FloorTilemap { get; set; }
        private RectTransform RotateUIParent { get; set; }
        private Image ConfirmButtonImage { get; set; } // UI_DecoConfirmBtn�� Image ������Ʈ

        public void SetOccupied(bool state)
        {
            IsOccupied = state;
        }

        private void Start()
        {
            RotateUIParent = GameObject.Find("UI_DecorateUIElement")?.GetComponent<RectTransform>();

            // UI_DecoConfirmBtn�� Image ������Ʈ ã��
            GameObject confirmBtn = GameObject.Find("UI_DecoConfirmBtn");
            if (confirmBtn != null)
            {
                ConfirmButtonImage = confirmBtn.GetComponent<Image>();
            }

            UpdateRotateUIPosition();

            // ���� ��Ƽ���� ����
            if (spriteRenderer != null)
            {
                _originalMaterial = spriteRenderer.material;
            }

            // �ƿ����� ��Ƽ������� Inspector���� �������� ���� ��� ���
            if (greenOutlineMaterial == null || redOutlineMaterial == null)
            {
                Debug.LogWarning($"[DraggableItem] {gameObject.name}�� �ƿ����� ��Ƽ������� �������� �ʾҽ��ϴ�. Inspector���� GreenOutlineMaterial�� RedOutlineMaterial�� �Ҵ����ּ���.");
            }
        }

        private void Update()
        {
            if (!_isDragging && DecorateManager.Instance.targetItem == this && DecorateManager.Instance.IsDecorateMode)
            {
                UpdateRotateUIPosition();
            }
        }

        private void OnEnable()
        {
            FloorTilemap = GameObject.Find("1FFloor")?.GetComponent<Tilemap>();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            // ��ġ��尡 �ƴϸ� �巡�� �Ұ�
            if (!DecorateManager.Instance.IsDecorateMode) return;

            // �ٸ� �������� ��ġ ��� ���̶�� �ش� �������� ���� ��ġ�� �ǵ���
            if (DecorateManager.Instance.targetItem != null &&
                DecorateManager.Instance.targetItem != this &&
                DecorateManager.Instance.targetItem.IsPendingPlacement)
            {
                DecorateManager.Instance.targetItem.CancelPendingPlacement();
            }

            Debug.Log("Ÿ�� ������ ������");
            DecorateManager.Instance.targetItem = this;
        }


        public void OnBeginDrag(PointerEventData eventData)
        {
            // ��ġ��尡 �ƴϸ� �巡�� �Ұ�
            if (!DecorateManager.Instance.IsDecorateMode) return;

            _initialPosition = transform.position;
            _isDragging = true;

            // ���� �׸��� ��ġ ���� �� �ش� ��ġ���� ������ ����
            Vector3Int cellPosition = FloorTilemap.WorldToCell(transform.position);
            _originalGridPosition = new Vector2Int(cellPosition.x, cellPosition.y);
            DecorateManager.Instance.RemoveItem(_originalGridPosition, _itemSize);

            // ���� ��ġ ��� ���� ���
            if (_isPendingPlacement)
            {
                _isPendingPlacement = false;
                EnableOutline(false);
            }

            if (RotateUIParent != null)
                RotateUIParent.gameObject.SetActive(false);
        }

        public void OnDrag(PointerEventData eventData)
        {
            // ��ġ��尡 �ƴϸ� �巡�� �Ұ�
            if (!DecorateManager.Instance.IsDecorateMode || !_isDragging) return;

            // ���콺 ��ġ�� ���� ��ǥ�� ��ȯ
            Vector3 worldMousePosition = Camera.main.ScreenToWorldPoint(eventData.position);
            worldMousePosition.z = 0;

            // �� ���� ��ġ ���
            Vector3Int cellPosition = FloorTilemap.WorldToCell(worldMousePosition);
            Vector3 worldCenter = FloorTilemap.GetCellCenterWorld(cellPosition);

            // �� �߽ɿ� ������ �̵�
            transform.position = worldCenter;

            // ��ġ ���� ���� Ȯ��
            Vector2Int cell2D = new Vector2Int(cellPosition.x, cellPosition.y);
            bool canPlace = DecorateManager.Instance.CanPlaceItem(cell2D, _itemSize);

            // �ƿ����� ���� ����
            UpdateOutlineColor(canPlace);

            // UI ��������Ʈ ����
            UpdateConfirmButtonSprite(canPlace);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            // ��ġ��尡 �ƴϸ� �巡�� �Ұ�
            if (!DecorateManager.Instance.IsDecorateMode) return;

            _isDragging = false;

            Vector3Int cellPosition = FloorTilemap.WorldToCell(transform.position);
            Vector2Int cell2D = new Vector2Int(cellPosition.x, cellPosition.y);

            Debug.Log($"[OnEndDrag] �巡�� ���� ��ġ: {transform.position}");
            Debug.Log($"[OnEndDrag] �� ��ġ: {cellPosition}");
            Debug.Log($"[OnEndDrag] 2D �� ��ġ: {cell2D}");

            // �巡�� ���� �� �׻� ��ġ ��� ���·� ��ȯ
            _isPendingPlacement = true;
            _pendingPosition = transform.position;
            _pendingGridPosition = cell2D;
            _canPlaceAtPendingPosition = DecorateManager.Instance.CanPlaceItem(cell2D, _itemSize);

            Debug.Log($"[OnEndDrag] ��ġ ��� ��ġ: {_pendingPosition}");
            Debug.Log($"[OnEndDrag] ��ġ ��� �׸��� ��ġ: {_pendingGridPosition}");
            Debug.Log($"[OnEndDrag] ��ġ ���� ����: {_canPlaceAtPendingPosition}");

            // �ƿ����� ȿ�� ���� (��ġ ���� ���ο� ����)
            UpdateOutlineColor(_canPlaceAtPendingPosition);

            // UI ��������Ʈ ����
            UpdateConfirmButtonSprite(_canPlaceAtPendingPosition);

            // UI �ٽ� Ȱ��ȭ + ��ġ ������Ʈ
            if (RotateUIParent != null && DecorateManager.Instance.targetItem == this)
            {
                RotateUIParent.gameObject.SetActive(true);
                UpdateRotateUIPosition();
            }

            Debug.Log($"[OnEndDrag] ���� ���� - IsPendingPlacement: {_isPendingPlacement}, CanPlace: {_canPlaceAtPendingPosition}");
        }
        // <summary>
        /// ��ġ Ȯ�� (Confirm ��ư�� ������ �� ȣ��)
        /// </summary>
        public void ConfirmPlacement()
        {
            Debug.Log($"[ConfirmPlacement] ���� - IsPendingPlacement: {_isPendingPlacement}");

            if (!_isPendingPlacement)
            {
                Debug.LogError("[ConfirmPlacement] ��ġ ��� ���°� �ƴմϴ�!");
                return;
            }

            Debug.Log($"[ConfirmPlacement] ���� Transform ��ġ: {transform.position}");
            Debug.Log($"[ConfirmPlacement] ��� ���� ��ġ: {_pendingPosition}");
            Debug.Log($"[ConfirmPlacement] ��� ���� �׸��� ��ġ: {_pendingGridPosition}");
            Debug.Log($"[ConfirmPlacement] ��ġ ���� ����: {_canPlaceAtPendingPosition}");

            if (_canPlaceAtPendingPosition)
            {
                Debug.Log($"[ConfirmPlacement] ��ġ Ȯ�� ���� ��...");

                // 1. ���� �׸��忡 ������ ��ġ
                bool placementResult = DecorateManager.Instance.GridManager.CanPlaceItem(_pendingGridPosition, _itemSize);
                Debug.Log($"[ConfirmPlacement] �׸��� �Ŵ��� ��ġ ���� ��Ȯ��: {placementResult}");

                if (placementResult)
                {
                    DecorateManager.Instance.PlaceItem(_pendingGridPosition, _itemSize);
                    Debug.Log($"[ConfirmPlacement] �׸��忡 ������ ��ġ �Ϸ�");
                }
                else
                {
                    Debug.LogError("[ConfirmPlacement] �׸��� �Ŵ������� ��ġ �Ұ���!");
                    CancelPendingPlacement();
                    return;
                }

                // 2. ������ ��ġ�� Ȯ���� ��ġ�� ����
                Vector3 oldPosition = transform.position;
                transform.position = _pendingPosition;
                Debug.Log($"[ConfirmPlacement] ��ġ ������Ʈ: {oldPosition} -> {transform.position}");

                // 3. ���� �׸��� ��ġ�� �ʱ� ��ġ ������Ʈ
                Vector2Int oldOriginalGrid = _originalGridPosition;
                Vector3 oldInitialPosition = _initialPosition;

                _originalGridPosition = _pendingGridPosition;
                _initialPosition = _pendingPosition;

                Debug.Log($"[ConfirmPlacement] ���� �׸��� ��ġ ������Ʈ: {oldOriginalGrid} -> {_originalGridPosition}");
                Debug.Log($"[ConfirmPlacement] �ʱ� ��ġ ������Ʈ: {oldInitialPosition} -> {_initialPosition}");

                // 4. ��ġ ��� ���� ����
                _isPendingPlacement = false;
                _canPlaceAtPendingPosition = false;

                Debug.Log($"[ConfirmPlacement] ��ġ ��� ���� ����");

                // 5. �ƿ����� ȿ�� ��Ȱ��ȭ
                EnableOutline(false);

                // 6. UI ��������Ʈ�� �⺻ ���·� ����
                UpdateConfirmButtonSprite(true);

                Debug.Log($"[ConfirmPlacement] ��ġ Ȯ�� �Ϸ�! ���� ��ġ: {transform.position}");
            }
            else
            {
                Debug.LogWarning("[ConfirmPlacement] ��ġ �Ұ����� ��ġ - ���� ��ġ�� ����");
                CancelPendingPlacement();
            }
        }        /// <summary>
                 /// ��ġ ��� ���� ��� (���� ��ġ�� �ǵ���)
                 /// </summary>

        public void CancelPendingPlacement()
        {
            Debug.Log($"[CancelPendingPlacement] ���� - IsPendingPlacement: {_isPendingPlacement}");

            if (!_isPendingPlacement)
            {
                Debug.Log("[CancelPendingPlacement] ��ġ ��� ���°� �ƴϹǷ� ����");
                return;
            }

            Debug.Log($"[CancelPendingPlacement] ���� ��ġ: {transform.position}");
            Debug.Log($"[CancelPendingPlacement] ������ ��ġ: {_initialPosition}");
            Debug.Log($"[CancelPendingPlacement] ���� �׸��� ��ġ: {_originalGridPosition}");

            // ���� ��ġ�� ����
            Vector3 oldPosition = transform.position;
            transform.position = _initialPosition;
            Debug.Log($"[CancelPendingPlacement] ��ġ ����: {oldPosition} -> {transform.position}");

            // ���� �׸��� ��ġ�� �ٽ� ��ġ
            DecorateManager.Instance.PlaceItem(_originalGridPosition, _itemSize);
            Debug.Log($"[CancelPendingPlacement] ���� �׸��� ��ġ�� ���ġ �Ϸ�");

            // ��ġ ��� ���� ����
            _isPendingPlacement = false;
            _canPlaceAtPendingPosition = false;

            // �ƿ����� ȿ�� ��Ȱ��ȭ
            EnableOutline(false);

            // UI ��������Ʈ�� �⺻ ���·� ����
            UpdateConfirmButtonSprite(true);

            // UI ��ġ ������Ʈ
            if (RotateUIParent != null && DecorateManager.Instance.targetItem == this)
            {
                UpdateRotateUIPosition();
            }

            Debug.Log($"[CancelPendingPlacement] ��� �Ϸ� - ���� ��ġ: {transform.position}");
        }
        /// <summary>
        /// �ƿ����� ȿ�� Ȱ��ȭ/��Ȱ��ȭ
        /// </summary>
        private void EnableOutline(bool enable)
        {
            if (spriteRenderer == null) return;

            if (enable)
            {
                // �⺻������ �ʷϻ� �ƿ��������� ����
                spriteRenderer.material = greenOutlineMaterial;
            }
            else
            {
                spriteRenderer.material = _originalMaterial;
            }
        }

        /// <summary>
        /// ��ġ ���� ���ο� ���� �ƿ����� ��Ƽ���� ����
        /// </summary>
        private void UpdateOutlineColor(bool canPlace)
        {
            if (spriteRenderer == null) return;

            // ��ġ ���� ���ο� ���� ������ ��Ƽ����� ��ü
            if (canPlace)
            {
                if (greenOutlineMaterial != null)
                    spriteRenderer.material = greenOutlineMaterial;
            }
            else
            {
                if (redOutlineMaterial != null)
                    spriteRenderer.material = redOutlineMaterial;
            }
        }

        /// <summary>
        /// ��ġ ���� ���ο� ���� Ȯ�� ��ư ��������Ʈ ����
        /// </summary>
        private void UpdateConfirmButtonSprite(bool canPlace)
        {
            if (ConfirmButtonImage == null) return;

            if (canPlace)
            {
                if (confirmActiveSprite != null)
                    ConfirmButtonImage.sprite = confirmActiveSprite;
            }
            else
            {
                if (confirmDeactiveSprite != null)
                    ConfirmButtonImage.sprite = confirmDeactiveSprite;
            }
        }

        public void RotateItem()
        {
            if (!DecorateManager.Instance.IsDecorateMode) return;

            Vector3 oldCenter = GetItemCenterWorldPos(FloorTilemap);

            // ȸ�� �ε��� ���� (���ѵ� ���� ����ŭ)
            _rotationIndex = (_rotationIndex + 1) % rotationLimit;

            // ��������Ʈ ����(ȸ��)
            if (directionSprites != null && directionSprites.Length >= rotationLimit)
            {
                spriteRenderer.sprite = directionSprites[_rotationIndex];
            }

            // ������ ��ȯ (x <-> y)
            _itemSize = new Vector2Int(_itemSize.y, _itemSize.x);

            // ȸ�� �� �߽� ��ġ ����
            Vector3 newCenter = GetItemCenterWorldPos(FloorTilemap);
            transform.position += oldCenter - newCenter;
        }

        private Vector3 GetItemCenterWorldPos(Tilemap tilemap)
        {
            Vector3Int cellPos = tilemap.WorldToCell(transform.position);
            Vector3 cellCenter = tilemap.GetCellCenterWorld(cellPos);
            Vector2 offset = new Vector2((_itemSize.x - 1) / 2f, (_itemSize.y - 1) / 2f);

            return cellCenter + new Vector3(offset.x * tilemap.cellSize.x, offset.y * tilemap.cellSize.y, 0);
        }

        private void UpdateRotateUIPosition()
        {
            if (RotateUIParent == null || !DecorateManager.Instance.IsDecorateMode) return;

            // ���� ��ǥ -> ȭ�� ��ǥ
            Vector2 screenPos = Camera.main.WorldToScreenPoint(transform.position);

            // UI ��ġ ������Ʈ
            RotateUIParent.position = screenPos;
        }
    }
}