using DalbitCafe.Deco;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

namespace DalbitCafe.Deco
{
    public class DraggableItem : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
    {
        [SerializeField] private Tilemap floorTilemap;

        private Vector3 _initialPosition; // 드래그 시작 전 위치
        private bool _isDragging = false; // 드래그 중인지 검사
        public Vector2Int _itemSize;  // 아이템 크기 (예: 1x1, 2x1 등)

        // 아이템 회전
        private int _rotationIndex = 0; // 0, 1, 2, 3 → 0~3 사이에서 회전 방향 인덱스

        public void OnBeginDrag(PointerEventData eventData) // 드래그를 시작했을 때
        {
            _initialPosition = transform.position; // 처음 위치 저장
            _isDragging = true;
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
            Vector3 oldCenter = GetItemCenterWorldPos(floorTilemap);

            _rotationIndex = (_rotationIndex + 1) % 4;
            transform.rotation = Quaternion.Euler(0, 0, _rotationIndex * -90f); // -90도는 시계방향

            _itemSize = new Vector2Int(_itemSize.y, _itemSize.x);

            Vector3 newCenter = GetItemCenterWorldPos(floorTilemap);
            transform.position += oldCenter - newCenter;
        }
        private Vector3 GetItemCenterWorldPos(Tilemap tilemap)
        {
            Vector3Int cellPos = tilemap.WorldToCell(transform.position);
            Vector3 cellCenter = tilemap.GetCellCenterWorld(cellPos);
            Vector2 offset = new Vector2((_itemSize.x - 1) / 2f, (_itemSize.y - 1) / 2f);

            return cellCenter + new Vector3(offset.x * tilemap.cellSize.x, offset.y * tilemap.cellSize.y, 0);
        }
    }
}
