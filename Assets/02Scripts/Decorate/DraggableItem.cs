using DalbitCafe.Deco;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

namespace DalbitCafe.Deco
{
    public class DraggableItem : MonoBehaviour, IPointerDownHandler, IDragHandler, IBeginDragHandler, IEndDragHandler
    {


        [Header("Slot Reference")]
        public InventorySlot sourceSlot; // 이 아이템을 생성한 슬롯 참조

        [Header("아이템 회전")]
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Sprite[] directionSprites; // 0: 오른쪽 아래, 1: 왼쪽 아래, 2: 왼쪽위, 3: 오른쪽 위
        [SerializeField] private int _rotationIndex = 0; // 0, 1, 2, 3 → 0~3 사이에서 회전 방향 인덱스

        [Header("아이템 회전 제한")]
        [SerializeField] private int rotationLimit = 4; // 회전 가능한 방향 수 : 2(좌우), 4(전체)

        [Header("아이템 배치")]
        private Vector3 _initialPosition; // 드래그 시작 전 아이템 위치
        private bool _isDragging = false; // 아이템이 드래그 중인지 검사
        public Vector2Int _itemSize;  // 아이템이 차지하는 크기 (예: 1x1, 2x1 등)
        private Vector2Int _originalGridPosition; // 원래 그리드 위치 저장

        // 배치 확정 시스템을 위한 변수들
        private bool _isPendingPlacement = false; // 배치 대기 중인지
        private Vector3 _pendingPosition; // 배치 대기 중인 위치
        private Vector2Int _pendingGridPosition; // 배치 대기 중인 그리드 위치
        private bool _canPlaceAtPendingPosition = false; // 대기 중인 위치에 배치 가능한지

        // 아이템 상태 추가
        private bool _isPlacedItem = false; // 이미 배치 확정된 아이템인지

        [Header("아웃라인 효과")]
        [SerializeField] private Material greenOutlineMaterial; // 배치 가능한 위치용 머티리얼
        [SerializeField] private Material redOutlineMaterial; // 배치 불가능한 위치용 머티리얼
        [SerializeField] private Material _originalMaterial; // 원본 머티리얼 저장

        [Header("UI 스프라이트 변경")]
        [SerializeField] private Sprite confirmActiveSprite; // 배치 가능할 때 사용할 스프라이트
        [SerializeField] private Sprite confirmDeactiveSprite; // 배치 불가능할 때 사용할 스프라이트 

        [SerializeField] private ItemData defaultItemData;  // inspector용
        private ItemData itemData;  // 내부 변수로만 사용

        // 프로퍼티들
        public bool IsOccupied { get; private set; } = false; // 사용 중인지
        public bool IsDragging => _isDragging;
        public bool IsPendingPlacement => _isPendingPlacement; // 배치 대기 중인지
        public bool IsPlacedItem => _isPlacedItem; // 배치 확정된 아이템인지
        public Vector2Int ItemSize => _itemSize;
        public int RotationIndex => _rotationIndex;
        public int RotationLimit => rotationLimit;
        public Vector3 InitialPosition => _initialPosition;

        private Tilemap FloorTilemap { get; set; }
        private Tilemap[] WallTilemaps { get; set; }  // WallGrid 자식 Tilemap들
        private RectTransform RotateUIParent { get; set; }
        private Image ConfirmButtonImage { get; set; } // UI_DecoConfirmBtn의 Image 컴포넌트

        public ItemData GetItemData() => itemData;
        public ItemCategory Category => itemData != null ? itemData.Category : ItemCategory.Interior;
        public System.Enum SubCategory => itemData?.SubCategory;


        public void SetOccupied(bool state)
        {
            IsOccupied = state;
        }

        public void Initialize(ItemData itemdata)
        {
            itemData = itemdata;
        }
        private void Awake()
        {
            if (itemData == null && defaultItemData != null)
                itemData = defaultItemData;
        }
        private void Start()
        {
            RotateUIParent = GameObject.Find("UI_DecorateUIElement")?.GetComponent<RectTransform>();

            // UI_DecoConfirmBtn의 Image 컴포넌트 찾기
            GameObject confirmBtn = GameObject.Find("UI_DecoConfirmBtn");
            if (confirmBtn != null)
            {
                ConfirmButtonImage = confirmBtn.GetComponent<Image>();
            }

            UpdateRotateUIPosition();

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

            GameObject wallObj = GameObject.Find("WallGrid");
            if (wallObj != null)
            {
                WallTilemaps = wallObj.GetComponentsInChildren<Tilemap>();
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            // 배치모드가 아니면 드래그 불가
            if (!DecorateManager.Instance.IsDecorateMode) return;

            // 다른 아이템이 배치 대기 중이라면 해당 아이템을 원래 위치로 되돌림
            if (DecorateManager.Instance.targetItem != null &&
                DecorateManager.Instance.targetItem != this &&
                DecorateManager.Instance.targetItem.IsPendingPlacement)
            {
                DecorateManager.Instance.targetItem.CancelPendingPlacement();
            }

            Debug.Log("타겟 아이템 지정됨");
            DecorateManager.Instance.targetItem = this;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (!DecorateManager.Instance.IsDecorateMode) return;

            _initialPosition = transform.position;
            _isDragging = true;

            Tilemap currentMap = GetCurrentTilemap();
            Vector3Int cellPosition = currentMap.WorldToCell(transform.position);
            _originalGridPosition = new Vector2Int(cellPosition.x, cellPosition.y);

            DecorateManager.Instance.RemoveItem(_originalGridPosition, _itemSize);

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
            Vector3 worldMousePosition = Camera.main.ScreenToWorldPoint(eventData.position);
            worldMousePosition.z = 0;

            Tilemap currentMap = GetCurrentTilemap();
            Vector3Int cellPosition = currentMap.WorldToCell(worldMousePosition);
            Vector3 worldCenter = currentMap.GetCellCenterWorld(cellPosition);

            transform.position = worldCenter;

            Vector2Int cell2D = new Vector2Int(cellPosition.x, cellPosition.y);
            bool canPlace = DecorateManager.Instance.CanPlaceItem(cell2D, _itemSize, (InteriorType)SubCategory);

            UpdateOutlineColor(canPlace);
            UpdateConfirmButtonSprite(canPlace);
        }


        public void OnEndDrag(PointerEventData eventData)
        {
            if (!DecorateManager.Instance.IsDecorateMode) return;
            _isDragging = false;

            Tilemap currentMap = GetCurrentTilemap();
            Vector3Int cellPosition = currentMap.WorldToCell(transform.position);
            Vector2Int cell2D = new Vector2Int(cellPosition.x, cellPosition.y);

            _isPendingPlacement = true;
            _pendingPosition = transform.position;
            _pendingGridPosition = cell2D;
            _canPlaceAtPendingPosition = DecorateManager.Instance.CanPlaceItem(cell2D, _itemSize, (InteriorType)SubCategory);

            UpdateOutlineColor(_canPlaceAtPendingPosition);
            UpdateConfirmButtonSprite(_canPlaceAtPendingPosition);

            if (RotateUIParent != null && DecorateManager.Instance.targetItem == this)
            {
                RotateUIParent.gameObject.SetActive(true);
                UpdateRotateUIPosition();
            }
        }


        /// <summary>
        /// 배치 확정 (Confirm 버튼을 눌렀을 때 호출)
        /// </summary>
        /// <summary>
        /// 배치 확정 (Confirm 버튼을 눌렀을 때 호출)
        /// </summary>
        public void ConfirmPlacement()
        {
            if (!_isPendingPlacement)
                return;

            if (_canPlaceAtPendingPosition)
            {
                // 1. 먼저 그리드에 아이템 배치 가능 여부 확인
                bool placementResult = DecorateManager.Instance.CanPlaceItem(
                    _pendingGridPosition,
                    _itemSize,
                    (InteriorType)SubCategory
                );

                if (placementResult)
                {
                    // 타입에 따라 올바른 PlaceItem 호출
                    InteriorType type = (InteriorType)SubCategory;

                    if (type == InteriorType.WallDecoration)
                    {
                        // 벽 전용
                        DecorateManager.Instance.PlaceItem(_pendingGridPosition, _itemSize, InteriorType.WallDecoration);
                        Debug.Log($"[ConfirmPlacement] WallDecoration 배치 완료 at {_pendingGridPosition}");
                    }
                    else
                    {
                        // 바닥 전용
                        DecorateManager.Instance.PlaceItem(_pendingGridPosition, _itemSize);
                        Debug.Log($"[ConfirmPlacement] Floor 아이템 배치 완료 at {_pendingGridPosition}");
                    }
                }
                else
                {
                    CancelPendingPlacement();
                    return;
                }

                // 2. 아이템 위치를 확정된 위치로 설정
                transform.position = _pendingPosition;

                // 3. 원래 그리드 위치와 초기 위치 업데이트
                _originalGridPosition = _pendingGridPosition;
                _initialPosition = _pendingPosition;

                // 4. 슬롯에 배치 확정 알림 (수량 차감)
                if (sourceSlot != null)
                    sourceSlot.OnItemPlacementConfirmed();

                // 5. 상태 업데이트
                _isPendingPlacement = false;
                _canPlaceAtPendingPosition = false;
                _isPlacedItem = true;
                sourceSlot = null;
                spriteRenderer.material = _originalMaterial;

                // 6. 아웃라인/버튼 UI 원상복구
                EnableOutline(false);
                UpdateConfirmButtonSprite(true);

                // 7. 정렬 순서 갱신
                UpdateSortingOrder();

                Debug.Log($"[ConfirmPlacement] 최종 배치 완료! 위치: {transform.position}");
            }
            else
            {
                CancelPendingPlacement();
            }

            // 등록
            DecorateManager.Instance.RegisterPlacedItem(this);
        }


        private void UpdateSortingOrder()
        {
            if (spriteRenderer != null)
            {
                spriteRenderer.sortingOrder = -(int)(transform.position.y * 100);
            }
        }


        /// <summary>
        /// 배치 대기 상태 취소 (원래 위치로 되돌림)
        /// </summary>
        public void CancelPendingPlacement()
        {
            //Debug.Log($"[CancelPendingPlacement] 시작 - IsPendingPlacement: {_isPendingPlacement}, IsPlacedItem: {_isPlacedItem}");

            if (!_isPendingPlacement)
            {
                //Debug.Log("[CancelPendingPlacement] 배치 대기 상태가 아니므로 종료");
                return;
            }

            //Debug.Log($"[CancelPendingPlacement] 현재 위치: {transform.position}");
            //Debug.Log($"[CancelPendingPlacement] 복귀할 위치: {_initialPosition}");
            //Debug.Log($"[CancelPendingPlacement] 원래 그리드 위치: {_originalGridPosition}");

            // 원래 위치로 복귀
            Vector3 oldPosition = transform.position;
            transform.position = _initialPosition;
            //Debug.Log($"[CancelPendingPlacement] 위치 복귀: {oldPosition} -> {transform.position}");

            // 원래 그리드 위치에 다시 배치
            DecorateManager.Instance.PlaceItem(_originalGridPosition, _itemSize);
            //Debug.Log($"[CancelPendingPlacement] 원래 그리드 위치에 재배치 완료");

            // 배치 대기 상태 해제
            _isPendingPlacement = false;
            _canPlaceAtPendingPosition = false;

            // 아웃라인 효과 비활성화
            EnableOutline(false);

            // UI 스프라이트를 기본 상태로 복원
            UpdateConfirmButtonSprite(true);

            // UI 위치 업데이트
            if (RotateUIParent != null && DecorateManager.Instance.targetItem == this)
            {
                UpdateRotateUIPosition();
            }

            // 처리 방식 분기: 신규 아이템 vs 기존 배치된 아이템
            if (_isPlacedItem)
            {
                // 이미 배치 확정된 아이템인 경우: 원래 위치로 복귀만 하고 파괴하지 않음
                //Debug.Log("[CancelPendingPlacement] 배치 확정된 아이템 - 원래 위치로 복귀 완료");
            }
            else
            {
                // 인벤토리에서 새로 생성된 아이템인 경우: 슬롯에 수량 복구 후 파괴
                if (sourceSlot != null)
                {
                    sourceSlot.RestoreItemQuantity();
                    //Debug.Log("[CancelPendingPlacement] 신규 아이템 - 슬롯 수량 복구 후 파괴");
                }
                else
                {
                    //Debug.LogWarning("[CancelPendingPlacement] sourceSlot이 null이어서 수량 복구 불가");
                }

                // 오브젝트 파괴
                Destroy(gameObject);
            }

            //Debug.Log($"[CancelPendingPlacement] 취소 완료 - 최종 위치: {transform.position}");
        }

        /// <summary>
        /// 아이템을 배치 대기 상태로 시작 (인벤토리에서 새로 생성된 아이템용)
        /// </summary>
        public void StartPendingPlacement()
        {
            _initialPosition = transform.position;
            _isPlacedItem = false;

            Tilemap currentMap = GetCurrentTilemap();
            Vector3Int cellPosition = currentMap.WorldToCell(transform.position);
            _originalGridPosition = new Vector2Int(cellPosition.x, cellPosition.y);

            Vector3 worldCenter = currentMap.GetCellCenterWorld(cellPosition);
            transform.position = worldCenter;
            _initialPosition = worldCenter;

            _isPendingPlacement = true;
            _pendingPosition = transform.position;
            _pendingGridPosition = _originalGridPosition;

            _canPlaceAtPendingPosition = DecorateManager.Instance.CanPlaceItem(
                _pendingGridPosition,
                _itemSize,
                (InteriorType)SubCategory
            );

            UpdateOutlineColor(_canPlaceAtPendingPosition);
            UpdateConfirmButtonSprite(_canPlaceAtPendingPosition);

            if (RotateUIParent != null)
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

        /// <summary>
        /// 배치 가능 여부에 따라 확인 버튼 스프라이트 변경
        /// </summary>
        private void UpdateConfirmButtonSprite(bool canPlace)
        {
            if (ConfirmButtonImage == null) return;

            Button confirmBtn = ConfirmButtonImage.GetComponent<Button>();
            if (confirmBtn != null)
                confirmBtn.interactable = canPlace;  // 버튼 활성/비활성 제어

            ConfirmButtonImage.sprite = canPlace ? confirmActiveSprite : confirmDeactiveSprite;
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
                Debug.Log("스프라이트 바뀜");
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

        // 어떤 Tilemap을 기준으로 할지 선택
        private Tilemap GetCurrentTilemap()
        {
            if (SubCategory is InteriorType interiorType && interiorType == InteriorType.WallDecoration)
            {
                if (WallTilemaps != null && WallTilemaps.Length > 0)
                    return WallTilemaps[0]; // 첫 번째 자식 Tilemap 기준 (원하면 이름별 선택도 가능)
            }
            return FloorTilemap;
        }
    }
}