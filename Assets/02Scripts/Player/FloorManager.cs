using UnityEngine;
using UnityEngine.Tilemaps;

namespace DalbitCafe.Map
{
    public class FloorManager : MonoBehaviour
    {
        [SerializeField] private Tilemap floorTilemap;

        // ȭ�� ��ǥ -> ���� ��ǥ -> �� ��ǥ -> Ÿ�� ���� ����

        public bool IsFloor(Vector3 worldPos)
        {
            Vector3Int cellPos = floorTilemap.WorldToCell(worldPos);
            return floorTilemap.HasTile(cellPos);
        }
    }
}
