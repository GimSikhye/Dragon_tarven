using DalbitCafe.Deco;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

namespace DalbitCafe.Deco
{
    public class DraggableItem : MonoBehaviour, IPointerDownHandler, IDragHandler, IBeginDragHandler, IEndDragHandler
    {
        [Header("아이템 회전")]
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Sprite[] directionSprites; // 0: 오른쪽 아래, 1: 왼쪽 아래, 2: 왼쪽위, 3: 오른쪽 위
        private int _rotationIndex = 0; // 0, 1, 2, 3 → 0~3 사이에서 회전 방향 인덱스

        [Header("아이템 회전 제한")]
        [SerializeField] private int rotationLimit = 4; // 회전 가능한 방향 수 : 2(좌우), 4(전체)

        [Header("아이템 배치")]
        private Vector3 _initialPosition; // 드래그 시작 전 아이템 위치
        private bool _isDragging = false; // 아이템이 드래그 중인지 검사
        public Vector2Int _itemSize;  // 아이템이 차지하는 크기 (예: 1x1, 2x1 등)
        private Vector2Int _originalGridPosition; // 원래 그리드 위치 저장

        [Header("아웃라인 효과")]
        [SerializeField] private Material greenOutlineMaterial; // 배치 가능한 위치용 머티리얼
        [SerializeField] private Material redOutlineMaterial; // 배치 불가능한 위치용 머티리얼
        private Material _originalMaterial; // 원본 머티리얼 저장

        [Header("참조")]
        public ItemData itemData; // Inspector에 연결 필요

        // 프로퍼티들
        public bool IsOccupied { get; private set; } = false; // 사용 중인지
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

            // 원본 머티리얼 저장
            if (spriteRenderer != null)
            {
                _originalMaterial = spriteRenderer.material;
            }

            // 아웃라인 머티리얼들이 Inspector에서 설정되지 않은 경우 경고
            if (greenOutlineMaterial == null || redOutlineMaterial == null)
            {
                Debug.LogWarning($"[DraggableItem] {gameObject.name}의 아웃라인 머티리얼들이 설정되지 않았습니다. Inspector에서 GreenOutlineMaterial과 RedOutlineMaterial을 할당해주세요.");
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
            // 배치모드가 아니면 드래그 불가
            if (!DecorateManager.Instance.IsDecorateMode) return;

            Debug.Log("타겟 아이템 지정됨");
            DecorateManager.Instance.targetItem = this;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            // 배치모드가 아니면 드래그 불가
            if (!DecorateManager.Instance.IsDecorateMode) return;

            _initialPosition = transform.position;
            _isDragging = true;

            // 현재 그리드 위치 저장 및 해당 위치에서 아이템 제거
            Vector3Int cellPosition = FloorTilemap.WorldToCell(transform.position);
            _originalGridPosition = new Vector2Int(cellPosition.x, cellPosition.y);
            DecorateManager.Instance.RemoveItem(_originalGridPosition, _itemSize);

            if (RotateUIParent != null)
                RotateUIParent.gameObject.SetActive(false);
        }

        public void OnDrag(PointerEventData eventData)
        {
            // 배치모드가 아니면 드래그 불가
            if (!DecorateManager.Instance.IsDecorateMode || !_isDragging) return;

            // 마우스 위치를 월드 좌표로 변환
            Vector3 worldMousePosition = Camera.main.ScreenToWorldPoint(eventData.position);
            worldMousePosition.z = 0;

            // 셀 기준 위치 계산
            Vector3Int cellPosition = FloorTilemap.WorldToCell(worldMousePosition);
            Vector3 worldCenter = FloorTilemap.GetCellCenterWorld(cellPosition);

            // 셀 중심에 아이템 이동
            transform.position = worldCenter;

            // 배치 가능 여부 확인
            Vector2Int cell2D = new Vector2Int(cellPosition.x, cellPosition.y);
            bool canPlace = DecorateManager.Instance.CanPlaceItem(cell2D, _itemSize);

            // 아웃라인 색상 갱신
            UpdateOutlineColor(canPlace);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            // 배치모드가 아니면 드래그 불가
            if (!DecorateManager.Instance.IsDecorateMode) return;

            _isDragging = false;

            Vector3Int cellPosition = FloorTilemap.WorldToCell(transform.position);
            Vector2Int cell2D = new Vector2Int(cellPosition.x, cellPosition.y);

            if (DecorateManager.Instance.CanPlaceItem(cell2D, _itemSize))
            {
                // 새 위치에 배치
                DecorateManager.Instance.PlaceItem(cell2D, _itemSize);
            }
            else
            {
                // 원래 위치로 복귀 및 그리드에 다시 배치
                transform.position = _initialPosition;
                DecorateManager.Instance.PlaceItem(_originalGridPosition, _itemSize);
            }

            // 아웃라인 효과 비활성화
            EnableOutline(false);

            // UI 다시 활성화 + 위치 업데이트
            if (RotateUIParent != null && DecorateManager.Instance.targetItem == this)
            {
                RotateUIParent.gameObject.SetActive(true);
                UpdateRotateUIPosition();
            }
        }

        /// <summary>
        /// 아웃라인 효과 활성화/비활성화
        /// </summary>
        private void EnableOutline(bool enable)
        {
            if (spriteRenderer == null) return;

            if (enable)
            {
                // 기본적으로 초록색 아웃라인으로 시작
                spriteRenderer.material = greenOutlineMaterial;
            }
            else
            {
                spriteRenderer.material = _originalMaterial;
            }
        }

        /// <summary>
        /// 배치 가능 여부에 따라 아웃라인 머티리얼 변경
        /// </summary>
        private void UpdateOutlineColor(bool canPlace)
        {
            if (spriteRenderer == null) return;

            // 배치 가능 여부에 따라 적절한 머티리얼로 교체
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

            // 회전 인덱스 갱신 (제한된 방향 수만큼)
            _rotationIndex = (_rotationIndex + 1) % rotationLimit;

            // 스프라이트 변경(회전)
            if (directionSprites != null && directionSprites.Length >= rotationLimit)
            {
                spriteRenderer.sprite = directionSprites[_rotationIndex];
            }

            // 사이즈 전환 (x <-> y)
            _itemSize = new Vector2Int(_itemSize.y, _itemSize.x);

            // 회전 시 중심 위치 보정
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

            // 월드 좌표 -> 화면 좌표
            Vector2 screenPos = Camera.main.WorldToScreenPoint(transform.position);

            // UI 위치 업데이트
            RotateUIParent.position = screenPos;
        }
    }
}