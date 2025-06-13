using DalbitCafe.Deco;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

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

        [Header("�ƿ����� ȿ��")]
        [SerializeField] private Material greenOutlineMaterial; // ��ġ ������ ��ġ�� ��Ƽ����
        [SerializeField] private Material redOutlineMaterial; // ��ġ �Ұ����� ��ġ�� ��Ƽ����
        private Material _originalMaterial; // ���� ��Ƽ���� ����

        [Header("����")]
        public ItemData itemData; // Inspector�� ���� �ʿ�

        // ������Ƽ��
        public bool IsOccupied { get; private set; } = false; // ��� ������
        public bool IsDragging => _isDragging;
        public Vector2Int ItemSize => _itemSize;
        public int RotationIndex => _rotationIndex;
        public int RotationLimit => rotationLimit;
        public Vector3 InitialPosition => _initialPosition;

        private Tilemap FloorTilemap { get; set; }
        private RectTransform RotateUIParent { get; set; }

        public void SetOccupied(bool state)
        {
            IsOccupied = state;
        }

        private void Start()
        {
            RotateUIParent = GameObject.Find("UI_DecorateUIElement")?.GetComponent<RectTransform>();
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
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            // ��ġ��尡 �ƴϸ� �巡�� �Ұ�
            if (!DecorateManager.Instance.IsDecorateMode) return;

            _isDragging = false;

            Vector3Int cellPosition = FloorTilemap.WorldToCell(transform.position);
            Vector2Int cell2D = new Vector2Int(cellPosition.x, cellPosition.y);

            if (DecorateManager.Instance.CanPlaceItem(cell2D, _itemSize))
            {
                // �� ��ġ�� ��ġ
                DecorateManager.Instance.PlaceItem(cell2D, _itemSize);
            }
            else
            {
                // ���� ��ġ�� ���� �� �׸��忡 �ٽ� ��ġ
                transform.position = _initialPosition;
                DecorateManager.Instance.PlaceItem(_originalGridPosition, _itemSize);
            }

            // �ƿ����� ȿ�� ��Ȱ��ȭ
            EnableOutline(false);

            // UI �ٽ� Ȱ��ȭ + ��ġ ������Ʈ
            if (RotateUIParent != null && DecorateManager.Instance.targetItem == this)
            {
                RotateUIParent.gameObject.SetActive(true);
                UpdateRotateUIPosition();
            }
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