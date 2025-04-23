using DalbitCafe.Deco;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

namespace DalbitCafe.Deco
{
    public class DraggableItem : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
    {
        [SerializeField] private Tilemap tilemap;

        private Vector3 _initialPosition; // �巡�� ���� �� ��ġ
        private bool _isDragging = false;
        public Vector2Int _itemSize;  // ������ ũ�� (��: 1x1, 2x1 ��)

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
                // ���콺 ��ġ�� ���� ��ǥ�� ��ȯ
                Vector3 newPosition = Camera.main.ScreenToWorldPoint(eventData.position);
                newPosition.z = 0;

                // �� ���� ��ġ ���
                Vector3Int cellPosition = tilemap.WorldToCell(newPosition);
                Vector3 worldCenter = tilemap.GetCellCenterWorld(cellPosition);

                // �� �߽ɿ� ������ �̵�
                transform.position = worldCenter;

                // ��ġ ���� ���� Ȯ��
                Vector2Int cell2D = new Vector2Int(cellPosition.x, cellPosition.y);
                bool canPlace = GameManager.Instance.DecorateManager.CanPlaceItem(cell2D, _itemSize);

                // �׵θ� ���� ����
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
                // ��ġ �Ϸ� �� UI ����� (��: Move/������ ��ư ��)
                // HideButtons();
            }
            else
            {
                // ���� ��ġ�� ����
                transform.position = _initialPosition;
            }
        }

        private void UpdateBorderColor(bool canPlace)
        {
            // �׵θ� ���� ó�� (��������Ʈ �׵θ� ��� ���� ����)
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
            // �ٹ̱� �Ϸ� �� UI ��ư ����� ó��
        }
    }
}
