using DalbitCafe.Deco;
using UnityEngine.EventSystems;
using UnityEngine;
// 아이템마다 넣어야 이 스크립트들 하나? 흠....
namespace DalbitCafe.Deco
{
    public class DraggableItem : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
    {
        private Vector3 _initialPosition; // 초기 위치 
        private bool _isDragging = false; // 드래그 중인지
        public Vector2Int _itemSize;  // 아이템 크기 (1x1, 2x1 등)

        public void OnBeginDrag(PointerEventData eventData)
        {
            Debug.Log("드래그 시작");
            _initialPosition = transform.position;  // 드래그 시작 위치 저장(아이템의 초기 위치)
            _isDragging = true; // 드래그 중이다
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (_isDragging)
            {
                Vector3 newPosition = Camera.main.ScreenToWorldPoint(eventData.position);
                newPosition.z = 0;  // Z축은 고정

                // 그리드에 맞춰 아이템 이동 (반올림)
                transform.position = new Vector3(Mathf.Round(newPosition.x), Mathf.Round(newPosition.y), 0);

                // 배치 가능한지 확인 ( 배치 가능확인여부에 따라 보더 색깔 달라짐)
                bool canPlace = DecorateManager.Instance.CanPlaceItem(new Vector2Int((int)transform.position.x, (int)transform.position.y), _itemSize);
                UpdateBorderColor(canPlace); 
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            _isDragging = false;
            // 배치할 수 있으면 배치하고, 그리드 업데이트
            if (DecorateManager.Instance.CanPlaceItem(new Vector2Int((int)transform.position.x, (int)transform.position.y), _itemSize))
            {
                DecorateManager.Instance.PlaceItem(new Vector2Int((int)transform.position.x, (int)transform.position.y), _itemSize);
                // 배치 완료 후 꾸미기 UI 숨기기(무브, 보관함 뜨는 버튼들)
                //HideButtons();
            }
            else // 배치할 수 없다면(false)
            {
                // 원래 위치로 돌아가게 처리
                transform.position = _initialPosition;
            }
        }

        private void UpdateBorderColor(bool canPlace)
        {
            // 초록색/빨간색 테두리 업데이트 //스프라이트 테두리에 맞게 테투리가 그려지게 하는방법 없나? 그리고 두께도 설정할수있게 하면 좋겠음
            if (canPlace)
            {
                // 초록색
                // itemBorder.color = Color.green;
            }
            else
            {
                // 빨간색
                // itemBorder.color = Color.red;
            }
        }

        private void HideButtons()
        {
            // 배치 완료 후 버튼 숨기기
        }
    }

}
