using DalbitCafe.Deco;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

namespace DalbitCafe.Deco
{
    public class DraggableItem : MonoBehaviour, IPointerClickHandler, IDragHandler, IBeginDragHandler, IEndDragHandler
    {
        [SerializeField] private Tilemap floorTilemap;

        [Header("아이템 회전")]
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Sprite[] directionSprites; // 0: 아래, 1 : 오른쪽, 2 : 위, 3: 왼쪽(하, 우, 상, 좌
        private int _rotationIndex = 0; // 0, 1, 2, 3 → 0~3 사이에서 회전 방향 인덱스
        private RectTransform rotateUIParent;

        [Header("아이템 배치")]
        private Vector3 _initialPosition; // 드래그 시작 전 위치
        private bool _isDragging = false; // 드래그 중인지 검사
        public Vector2Int _itemSize;  // 아이템 크기 (예: 1x1, 2x1 등)

        private void Start()
        {
            rotateUIParent = GameObject.Find("UI_DecoButtons").GetComponent<RectTransform>(); // 회전 버튼 부모 오브젝트 이름에 따라 변경
            UpdateRotateUIPosition();
        }

        private void Update()
        {
            if(!_isDragging && DecorateManager.Instance.targetItem == this)
            {
                UpdateRotateUIPosition();
            }
        }

        private void OnEnable()
        {
            floorTilemap = GameObject.Find("StoreFloor").GetComponent<Tilemap>();   
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            DecorateManager.Instance.targetItem = this;
        }

        public void OnBeginDrag(PointerEventData eventData) // 드래그를 시작했을 때
        {
            _initialPosition = transform.position; // 처음 위치 저장
            _isDragging = true;

            if(rotateUIParent != null) rotateUIParent.gameObject.SetActive(false);

        }

        public void OnDrag(PointerEventData eventData) // 드래그 중일 때
        {
            if (_isDragging)
            {
                // 마우스 위치를 월드 좌표로 변환
                Vector3 worldMousePosition = Camera.main.ScreenToWorldPoint(eventData.position);
                worldMousePosition.z = 0;

                // 셀 기준 위치 계산
                Vector3Int cellPosition = floorTilemap.WorldToCell(worldMousePosition); // 어느 셀에 해당하는지 계산
                Vector3 worldCenter = floorTilemap.GetCellCenterWorld(cellPosition); 

                // 셀 중심에 아이템 이동
                transform.position = worldCenter;

                // 배치 가능 여부 확인
                Vector2Int cell2D = new Vector2Int(cellPosition.x, cellPosition.y); // 시작 셀 좌표
                bool canPlace = DecorateManager.Instance.CanPlaceItem(cell2D, _itemSize); // 이 셀부터 _itemSize 크기만큼 공간이 비어 있나요?"를 체크

                // 테두리 색상 갱신(불가능하면 테두리 빨간색으로)
                //UpdateBorderColor(canPlace);
            }
        }

        public void OnEndDrag(PointerEventData eventData) // 아이템을 놓았을 때
        {
            _isDragging = false;

            Vector3Int cellPosition = floorTilemap.WorldToCell(transform.position);
            Vector2Int cell2D = new Vector2Int(cellPosition.x, cellPosition.y);

            if (DecorateManager.Instance.CanPlaceItem(cell2D, _itemSize))
            {
                DecorateManager.Instance.PlaceItem(cell2D, _itemSize);
                // 배치 완료 후 UI 숨기기 (예: Move/보관함 버튼 등)
                // HideButtons();
            }
            else
            {
                // 원래 위치로 복귀
                transform.position = _initialPosition;
            }

            // UI 다시 활성화 + 위치 업데이트
            if(rotateUIParent != null && DecorateManager.Instance.targetItem == this)
            {
                rotateUIParent.gameObject.SetActive(true);
                UpdateRotateUIPosition();
            }
        }

        private void UpdateBorderColor(bool canPlace)
        {
            // 테두리 색상 처리 (스프라이트 테두리 등과 연동 가능)
            if (canPlace)
            {
                // itemBorder.color = Color.green;
            }
            else
            {
                // itemBorder.color = Color.red;
            }
        }

        private void HideButtons()
        {
            // 꾸미기 완료 시 UI 버튼 숨기기 처리
        }

        public void RotateItem()
        {
            Vector3 oldCenter = GetItemCenterWorldPos(floorTilemap); // floorTilemap의 중심을 가져옴?

            // 회전 인덱스 갱신 (시계방향)
            _rotationIndex = (_rotationIndex + 1) % 4;

            // 스프라이트 변경(회전)
            if (directionSprites != null && directionSprites.Length == 4)
            {
                spriteRenderer.sprite = directionSprites[_rotationIndex];
            }

            // 사이즈 전환 (x <-> y)
            _itemSize = new Vector2Int(_itemSize.y, _itemSize.x); // 셀에서 차지하는 공간도 회전에 따라 바뀜(회전을 하면, 가로 ↔ 세로가 바뀌는 경우가 생기기 때문)
            // 0도(↓), 180도(↑): 원래 사이즈 유지 // 90도(→), 270도(←): x ↔ y 교환 필요
            // 그래서 회전할 때마다 x와 y를 한 번씩 교체해주면 결국 4회전 이후 원래대로 돌아오게 됩니다!

            // 회전 시 중심 위치 보정
            Vector3 newCenter = GetItemCenterWorldPos(floorTilemap);
            transform.position += oldCenter - newCenter;
        }

        private Vector3 GetItemCenterWorldPos(Tilemap tilemap)
        {
            Vector3Int cellPos = tilemap.WorldToCell(transform.position); // 아이템의 위치를 cell 위치로 바꿈 (어디에 위치해 있는가)
            Vector3 cellCenter = tilemap.GetCellCenterWorld(cellPos); // 좌하단 cell의 가운데
            Vector2 offset = new Vector2((_itemSize.x - 1) / 2f, (_itemSize.y - 1) / 2f); // 아이템이 여러 칸 크기일 경우 아이템 설치 시 그 위치는 좌하단 위치가 됨. 
            //  “여러 셀을 차지하는 아이템의 중심이 좌하단 기준으로부터 얼마나 떨어져 있는가”를 계산하는 공식 // 셀 단위 거리

            return cellCenter + new Vector3(offset.x * tilemap.cellSize.x, offset.y * tilemap.cellSize.y, 0); // Unity 월드에서는 셀의 크기가 1이 아닐 수 있다.

        }

        private void UpdateRotateUIPosition()
        {
            if (rotateUIParent == null) return;

            // 월드 좌표-> 화면 좌표
            Vector2 screenPos = Camera.main.WorldToScreenPoint(transform.position); 

            // UI 위치 업데이트
            rotateUIParent.position = screenPos;
        }

        // 디버깅 용도
        void OnDrawGizmos()
        {
            for (int x = -2; x <= 2; x++)
            {
                for (int y = -2; y <= 2; y++)
                {
                    Vector3 worldPos = floorTilemap.GetCellCenterWorld(new Vector3Int(x, y, 0));
                    UnityEditor.Handles.Label(worldPos, $"({x},{y})");
                }
            }
        }


    }
}
