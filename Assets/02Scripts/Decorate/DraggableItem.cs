using DalbitCafe.Deco;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

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

        // 배치 확정 시스템을 위한 변수들
        private bool _isPendingPlacement = false; // 배치 대기 중인지
        private Vector3 _pendingPosition; // 배치 대기 중인 위치
        private Vector2Int _pendingGridPosition; // 배치 대기 중인 그리드 위치
        private bool _canPlaceAtPendingPosition = false; // 대기 중인 위치에 배치 가능한지

        [Header("아웃라인 효과")]
        [SerializeField] private Material greenOutlineMaterial; // 배치 가능한 위치용 머티리얼
        [SerializeField] private Material redOutlineMaterial; // 배치 불가능한 위치용 머티리얼
        private Material _originalMaterial; // 원본 머티리얼 저장

        [Header("UI 스프라이트 변경")]
        [SerializeField] private Sprite confirmActiveSprite; // 배치 가능할 때 사용할 스프라이트
        [SerializeField] private Sprite confirmDeactiveSprite; // 배치 불가능할 때 사용할 스프라이트 

        [Header("참조")]
        public ItemData itemData; // Inspector에 연결 필요

        // 프로퍼티들
        public bool IsOccupied { get; private set; } = false; // 사용 중인지
        public bool IsDragging => _isDragging;
        public bool IsPendingPlacement => _isPendingPlacement; // 배치 대기 중인지
        public Vector2Int ItemSize => _itemSize;
        public int RotationIndex => _rotationIndex;
        public int RotationLimit => rotationLimit;
        public Vector3 InitialPosition => _initialPosition;

        private Tilemap FloorTilemap { get; set; }
        private RectTransform RotateUIParent { get; set; }
        private Image ConfirmButtonImage { get; set; } // UI_DecoConfirmBtn의 Image 컴포넌트

        public void SetOccupied(bool state)
        {
            IsOccupied = state;
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
            // 배치모드가 아니면 드래그 불가
            if (!DecorateManager.Instance.IsDecorateMode) return;

            _initialPosition = transform.position;
            _isDragging = true;

            // 현재 그리드 위치 저장 및 해당 위치에서 아이템 제거
            Vector3Int cellPosition = FloorTilemap.WorldToCell(transform.position);
            _originalGridPosition = new Vector2Int(cellPosition.x, cellPosition.y);
            DecorateManager.Instance.RemoveItem(_originalGridPosition, _itemSize);

            // 기존 배치 대기 상태 취소
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

            // UI 스프라이트 갱신
            UpdateConfirmButtonSprite(canPlace);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            // 배치모드가 아니면 드래그 불가
            if (!DecorateManager.Instance.IsDecorateMode) return;

            _isDragging = false;

            Vector3Int cellPosition = FloorTilemap.WorldToCell(transform.position);
            Vector2Int cell2D = new Vector2Int(cellPosition.x, cellPosition.y);

            Debug.Log($"[OnEndDrag] 드래그 종료 위치: {transform.position}");
            Debug.Log($"[OnEndDrag] 셀 위치: {cellPosition}");
            Debug.Log($"[OnEndDrag] 2D 셀 위치: {cell2D}");

            // 드래그 종료 시 항상 배치 대기 상태로 전환
            _isPendingPlacement = true;
            _pendingPosition = transform.position;
            _pendingGridPosition = cell2D;
            _canPlaceAtPendingPosition = DecorateManager.Instance.CanPlaceItem(cell2D, _itemSize);

            Debug.Log($"[OnEndDrag] 배치 대기 위치: {_pendingPosition}");
            Debug.Log($"[OnEndDrag] 배치 대기 그리드 위치: {_pendingGridPosition}");
            Debug.Log($"[OnEndDrag] 배치 가능 여부: {_canPlaceAtPendingPosition}");

            // 아웃라인 효과 유지 (배치 가능 여부에 따라)
            UpdateOutlineColor(_canPlaceAtPendingPosition);

            // UI 스프라이트 유지
            UpdateConfirmButtonSprite(_canPlaceAtPendingPosition);

            // UI 다시 활성화 + 위치 업데이트
            if (RotateUIParent != null && DecorateManager.Instance.targetItem == this)
            {
                RotateUIParent.gameObject.SetActive(true);
                UpdateRotateUIPosition();
            }

            Debug.Log($"[OnEndDrag] 최종 상태 - IsPendingPlacement: {_isPendingPlacement}, CanPlace: {_canPlaceAtPendingPosition}");
        }
        // <summary>
        /// 배치 확정 (Confirm 버튼을 눌렀을 때 호출)
        /// </summary>
        public void ConfirmPlacement()
        {
            Debug.Log($"[ConfirmPlacement] 시작 - IsPendingPlacement: {_isPendingPlacement}");

            if (!_isPendingPlacement)
            {
                Debug.LogError("[ConfirmPlacement] 배치 대기 상태가 아닙니다!");
                return;
            }

            Debug.Log($"[ConfirmPlacement] 현재 Transform 위치: {transform.position}");
            Debug.Log($"[ConfirmPlacement] 대기 중인 위치: {_pendingPosition}");
            Debug.Log($"[ConfirmPlacement] 대기 중인 그리드 위치: {_pendingGridPosition}");
            Debug.Log($"[ConfirmPlacement] 배치 가능 여부: {_canPlaceAtPendingPosition}");

            if (_canPlaceAtPendingPosition)
            {
                Debug.Log($"[ConfirmPlacement] 배치 확정 진행 중...");

                // 1. 먼저 그리드에 아이템 배치
                bool placementResult = DecorateManager.Instance.GridManager.CanPlaceItem(_pendingGridPosition, _itemSize);
                Debug.Log($"[ConfirmPlacement] 그리드 매니저 배치 가능 재확인: {placementResult}");

                if (placementResult)
                {
                    DecorateManager.Instance.PlaceItem(_pendingGridPosition, _itemSize);
                    Debug.Log($"[ConfirmPlacement] 그리드에 아이템 배치 완료");
                }
                else
                {
                    Debug.LogError("[ConfirmPlacement] 그리드 매니저에서 배치 불가능!");
                    CancelPendingPlacement();
                    return;
                }

                // 2. 아이템 위치를 확정된 위치로 설정
                Vector3 oldPosition = transform.position;
                transform.position = _pendingPosition;
                Debug.Log($"[ConfirmPlacement] 위치 업데이트: {oldPosition} -> {transform.position}");

                // 3. 원래 그리드 위치와 초기 위치 업데이트
                Vector2Int oldOriginalGrid = _originalGridPosition;
                Vector3 oldInitialPosition = _initialPosition;

                _originalGridPosition = _pendingGridPosition;
                _initialPosition = _pendingPosition;

                Debug.Log($"[ConfirmPlacement] 원래 그리드 위치 업데이트: {oldOriginalGrid} -> {_originalGridPosition}");
                Debug.Log($"[ConfirmPlacement] 초기 위치 업데이트: {oldInitialPosition} -> {_initialPosition}");

                // 4. 배치 대기 상태 해제
                _isPendingPlacement = false;
                _canPlaceAtPendingPosition = false;

                Debug.Log($"[ConfirmPlacement] 배치 대기 상태 해제");

                // 5. 아웃라인 효과 비활성화
                EnableOutline(false);

                // 6. UI 스프라이트를 기본 상태로 복원
                UpdateConfirmButtonSprite(true);

                Debug.Log($"[ConfirmPlacement] 배치 확정 완료! 최종 위치: {transform.position}");
            }
            else
            {
                Debug.LogWarning("[ConfirmPlacement] 배치 불가능한 위치 - 원래 위치로 복귀");
                CancelPendingPlacement();
            }
        }        /// <summary>
                 /// 배치 대기 상태 취소 (원래 위치로 되돌림)
                 /// </summary>

        public void CancelPendingPlacement()
        {
            Debug.Log($"[CancelPendingPlacement] 시작 - IsPendingPlacement: {_isPendingPlacement}");

            if (!_isPendingPlacement)
            {
                Debug.Log("[CancelPendingPlacement] 배치 대기 상태가 아니므로 종료");
                return;
            }

            Debug.Log($"[CancelPendingPlacement] 현재 위치: {transform.position}");
            Debug.Log($"[CancelPendingPlacement] 복귀할 위치: {_initialPosition}");
            Debug.Log($"[CancelPendingPlacement] 원래 그리드 위치: {_originalGridPosition}");

            // 원래 위치로 복귀
            Vector3 oldPosition = transform.position;
            transform.position = _initialPosition;
            Debug.Log($"[CancelPendingPlacement] 위치 복귀: {oldPosition} -> {transform.position}");

            // 원래 그리드 위치에 다시 배치
            DecorateManager.Instance.PlaceItem(_originalGridPosition, _itemSize);
            Debug.Log($"[CancelPendingPlacement] 원래 그리드 위치에 재배치 완료");

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

            Debug.Log($"[CancelPendingPlacement] 취소 완료 - 최종 위치: {transform.position}");
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