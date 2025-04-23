using DalbitCafe.Deco;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

namespace DalbitCafe.Deco
{
    public class DraggableItem : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
    {
        [SerializeField] private Tilemap tilemap;

        private Vector3 _initialPosition; // 드래그 시작 전 위치
        private bool _isDragging = false;
        public Vector2Int _itemSize;  // 아이템 크기 (예: 1x1, 2x1 등)

        private void OnEnable()
        {
            tilemap = GameObject.Find("StoreFloor").GetComponent<Tilemap>();
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            _initialPosition = transform.position;
            _isDragging = true;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (_isDragging)
            {
                // 마우스 위치를 월드 좌표로 변환
                Vector3 newPosition = Camera.main.ScreenToWorldPoint(eventData.position);
                newPosition.z = 0;

                // 셀 기준 위치 계산
                Vector3Int cellPosition = tilemap.WorldToCell(newPosition);
                Vector3 worldCenter = tilemap.GetCellCenterWorld(cellPosition);

                // 셀 중심에 아이템 이동
                transform.position = worldCenter;

                // 배치 가능 여부 확인
                Vector2Int cell2D = new Vector2Int(cellPosition.x, cellPosition.y);
                bool canPlace = GameManager.Instance.DecorateManager.CanPlaceItem(cell2D, _itemSize);

                // 테두리 색상 갱신
                UpdateBorderColor(canPlace);
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            _isDragging = false;

            Vector3Int cellPosition = tilemap.WorldToCell(transform.position);
            Vector2Int cell2D = new Vector2Int(cellPosition.x, cellPosition.y);

            if (GameManager.Instance.DecorateManager.CanPlaceItem(cell2D, _itemSize))
            {
                GameManager.Instance.DecorateManager.PlaceItem(cell2D, _itemSize);
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
    }
}
