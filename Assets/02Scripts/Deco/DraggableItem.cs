using DalbitCafe.Deco;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

namespace DalbitCafe.Deco
{
    public class DraggableItem : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
    {
        [SerializeField] private Tilemap floorTilemap;

        [Header("������ ȸ��")]
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Sprite[] directionSprites; // 0: �Ʒ�, 1 : ������, 2 : ��, 3: ����(��, ��, ��, ��
        private int _rotationIndex = 0; // 0, 1, 2, 3 �� 0~3 ���̿��� ȸ�� ���� �ε���

        [Header("������ ��ġ")]
        private Vector3 _initialPosition; // �巡�� ���� �� ��ġ
        private bool _isDragging = false; // �巡�� ������ �˻�
        public Vector2Int _itemSize;  // ������ ũ�� (��: 1x1, 2x1 ��)

        public void OnBeginDrag(PointerEventData eventData) // �巡�׸� �������� ��
        {
            _initialPosition = transform.position; // ó�� ��ġ ����
            _isDragging = true;
        }

        public void OnDrag(PointerEventData eventData) // �巡�� ���� ��
        {
            if (_isDragging)
            {
                // ���콺 ��ġ�� ���� ��ǥ�� ��ȯ
                Vector3 worldMousePosition = Camera.main.ScreenToWorldPoint(eventData.position);
                worldMousePosition.z = 0;

                // �� ���� ��ġ ���
                Vector3Int cellPosition = floorTilemap.WorldToCell(worldMousePosition); // ��� ���� �ش��ϴ��� ���
                Vector3 worldCenter = floorTilemap.GetCellCenterWorld(cellPosition); 

                // �� �߽ɿ� ������ �̵�
                transform.position = worldCenter;

                // ��ġ ���� ���� Ȯ��
                Vector2Int cell2D = new Vector2Int(cellPosition.x, cellPosition.y); // ���� �� ��ǥ
                bool canPlace = DecorateManager.Instance.CanPlaceItem(cell2D, _itemSize); // �� ������ _itemSize ũ�⸸ŭ ������ ��� �ֳ���?"�� üũ

                // �׵θ� ���� ����(�Ұ����ϸ� �׵θ� ����������)
                //UpdateBorderColor(canPlace);
            }
        }

        public void OnEndDrag(PointerEventData eventData) // �������� ������ ��
        {
            _isDragging = false;

            Vector3Int cellPosition = floorTilemap.WorldToCell(transform.position);
            Vector2Int cell2D = new Vector2Int(cellPosition.x, cellPosition.y);

            if (DecorateManager.Instance.CanPlaceItem(cell2D, _itemSize))
            {
                DecorateManager.Instance.PlaceItem(cell2D, _itemSize);
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

        public void RotateItem()
        {
            Vector3 oldCenter = GetItemCenterWorldPos(floorTilemap); // floorTilemap�� �߽��� ������?

            // ȸ�� �ε��� ���� (�ð����)
            _rotationIndex = (_rotationIndex + 1) % 4;

            // ��������Ʈ ����(ȸ��)
            if (directionSprites != null && directionSprites.Length == 4)
            {
                spriteRenderer.sprite = directionSprites[_rotationIndex];
            }

            // ������ ��ȯ (x <-> y)
            _itemSize = new Vector2Int(_itemSize.y, _itemSize.x); // ������ �����ϴ� ������ ȸ���� ���� �ٲ�(ȸ���� �ϸ�, ���� �� ���ΰ� �ٲ�� ��찡 ����� ����)

            // ȸ�� �� �߽� ��ġ ����
            Vector3 newCenter = GetItemCenterWorldPos(floorTilemap);
            transform.position += oldCenter - newCenter;
        }

        private Vector3 GetItemCenterWorldPos(Tilemap tilemap)
        {
            Vector3Int cellPos = tilemap.WorldToCell(transform.position); // �������� ��ġ�� cell ��ġ�� �ٲ� (��� ��ġ�� �ִ°�)
            Vector3 cellCenter = tilemap.GetCellCenterWorld(cellPos); // ���ϴ� cell�� ���
            Vector2 offset = new Vector2((_itemSize.x - 1) / 2f, (_itemSize.y - 1) / 2f); // �������� ���� ĭ ũ���� ��� ������ ��ġ �� �� ��ġ�� ���ϴ� ��ġ�� ��. 
            //  ������ ���� �����ϴ� �������� �߽��� ���ϴ� �������κ��� �󸶳� ������ �ִ°����� ����ϴ� ���� // �� ���� �Ÿ�

            return cellCenter + new Vector3(offset.x * tilemap.cellSize.x, offset.y * tilemap.cellSize.y, 0); // Unity ���忡���� ���� ũ�Ⱑ 1�� �ƴ� �� �ִ�.
                                                                                                              // �׷��� offset �� �� ũ��(tile size) �� ����� ��Ȯ�� �Ÿ��� ���� �� �ִ�.
                                                                                                              // return ���ϴ� �� �߽� ��ġ + (������ �߽ɱ��� �̵� �Ÿ�);

        }
    }
}
