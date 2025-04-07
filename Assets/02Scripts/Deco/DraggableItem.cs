using DalbitCafe.Deco;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.Tilemaps;
//
namespace DalbitCafe.Deco
{
    public class DraggableItem : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
    {
        [SerializeField] private Tilemap tilemap;

        private Vector3 _initialPosition; // �ʱ� ��ġ 
        private bool _isDragging = false; // �巡�� ������
        public Vector2Int _itemSize;  // ������ ũ�� (1x1, 2x1 ��)

        private void OnEnable()
        {
            tilemap = GameObject.Find("StoreFloor").GetComponent<Tilemap>();
        }
        public void OnBeginDrag(PointerEventData eventData)
        {
            Debug.Log("�巡�� ����");
            _initialPosition = transform.position;  // �巡�� ���� ��ġ ����(�������� �ʱ� ��ġ)
            _isDragging = true; // �巡�� ���̴�
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (_isDragging)
            {
                Vector3 newPosition = Camera.main.ScreenToWorldPoint(eventData.position);
                newPosition.z = 0;  // Z���� ����

                Vector3Int cellPosition = tilemap.WorldToCell(newPosition);
                Vector3 worldCenter = tilemap.GetCellCenterWorld(cellPosition);

                transform.position = worldCenter;
                // �׸��忡 ���� ������ �̵� (�ݿø�)
                //transform.position = new Vector3(Mathf.Round(newPosition.x), Mathf.Round(newPosition.y), 0);

                // ��ġ �������� Ȯ�� ( ��ġ ����Ȯ�ο��ο� ���� ���� ���� �޶���)
                bool canPlace = DecorateManager.Instance.CanPlaceItem(new Vector2Int((int)transform.position.x, (int)transform.position.y), _itemSize);
                UpdateBorderColor(canPlace); 
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            _isDragging = false;
            // ��ġ�� �� ������ ��ġ�ϰ�, �׸��� ������Ʈ
            if (DecorateManager.Instance.CanPlaceItem(new Vector2Int((int)transform.position.x, (int)transform.position.y), _itemSize))
            {
                DecorateManager.Instance.PlaceItem(new Vector2Int((int)transform.position.x, (int)transform.position.y), _itemSize);
                // ��ġ �Ϸ� �� �ٹ̱� UI �����(����, ������ �ߴ� ��ư��)
                //HideButtons();
            }
            else // ��ġ�� �� ���ٸ�(false)
            {
                // ���� ��ġ�� ���ư��� ó��
                transform.position = _initialPosition;
            }
        }

        private void UpdateBorderColor(bool canPlace)
        {
            // �ʷϻ�/������ �׵θ� ������Ʈ //��������Ʈ �׵θ��� �°� �������� �׷����� �ϴ¹�� ����? �׸��� �β��� �����Ҽ��ְ� �ϸ� ������
            if (canPlace)
            {
                // �ʷϻ�
                // itemBorder.color = Color.green;
            }
            else
            {
                // ������
                // itemBorder.color = Color.red;
            }
        }

        private void HideButtons()
        {
            // ��ġ �Ϸ� �� ��ư �����
        }
    }

}
