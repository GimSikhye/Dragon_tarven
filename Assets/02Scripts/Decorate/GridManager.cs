using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

namespace DalbitCafe.Deco
{// �ڵ� ���� (3����ũ��Ʈ) �����ϰ� ȸ�� ��ư �����ؾ���
    public class GridManager : MonoSingleton<GridManager>
    {
        public Tilemap tilemap;
        public TileBase storeFloorTile; // �� Ÿ�ϸ� �ִ� ���� ��ġ ����
        [SerializeField] private float _tileSize = 0.5f;

        private bool[,] _grid;
        private int _gridWidth;
        private int _gridHeight;
        private Vector3Int _origin; // Ÿ�ϸ� cellBounds.min ����
        public Vector2Int TilemapOrigin => new Vector2Int(_origin.x, _origin.y);

        public bool IsOccupied(Vector2Int gridIndex)
        {
            return _grid[gridIndex.x, gridIndex.y];
        }

        private void OnEnable()
        {
            Debug.Log($"[{GetType().Name}] OnEnable ȣ���");
        }


        protected override void Awake()
        {
            base.Awake();
            InitTile();
        }

        private void InitTile(Scene scene, LoadSceneMode sceneMode)
        {
            if(scene.name == "GameScene")
            {
                Debug.Log("GirdManager �ʱ�ȭ");
                GameObject tile = GameObject.Find("1FFloor");
                tilemap = tile.GetComponent<Tilemap>();
                storeFloorTile = Resources.Load<TileBase>("spr_tile_floor");

                tilemap.CompressBounds(); // �� ���ֱ� (���ʿ��� �� Ÿ�� ��ǥ ����)

                BoundsInt bounds = tilemap.cellBounds;
                _origin = bounds.min; // Ÿ�ϸ� ������ ����
                _gridWidth = bounds.size.x;
                _gridHeight = bounds.size.y;

                _grid = new bool[_gridWidth, _gridHeight];
            }
        }

        private void InitTile()
        {
            Debug.Log("GirdManager �ʱ�ȭ");

            GameObject tileObj = GameObject.Find("1FFloor");
            if (tileObj == null)
            {
                Debug.LogError("[GridManager] 1FFloor GameObject�� ã�� �� �����ϴ�.");
                return;
            }

            tilemap = tileObj.GetComponent<Tilemap>();
            if (tilemap == null)
            {
                Debug.LogError("[GridManager] Tilemap ������Ʈ�� ã�� �� �����ϴ�.");
                return;
            }

            storeFloorTile = Resources.Load<TileBase>("spr_tile_floor");
            if (storeFloorTile == null)
            {
                Debug.LogError("[GridManager] spr_tile_floor Ÿ���� Resources���� �ҷ����� ���߽��ϴ�.");
                return;
            }

            tilemap.CompressBounds();
            BoundsInt bounds = tilemap.cellBounds;
            _origin = bounds.min;
            _gridWidth = bounds.size.x;
            _gridHeight = bounds.size.y;

            _grid = new bool[_gridWidth, _gridHeight];

            Debug.Log("[GridManager] �ʱ�ȭ �Ϸ�");
        }


        /// <summary>
        /// ��ġ �������� Ȯ��
        /// </summary>
        public bool CanPlaceItem(Vector2Int worldCellPos, Vector2Int size)
        {
            Vector3Int vec3 = new Vector3Int(worldCellPos.x, worldCellPos.y, 0);
            Vector2Int localPos = WorldToGridIndex(vec3); // ���޹��� �� ��ǥ�� ���� �ε����� ��ȯ (WorldToGridIndex)

            if (!IsInsideGrid(localPos, size)) return false; // size ��ŭ ������ ���鼭 �� ���� Ÿ���� �ִ���, �ٸ� �������� �̹� �ִ��� Ȯ��

            for (int x = 0; x < size.x; x++)
            {
                for (int y = 0; y < size.y; y++)
                {
                    int checkX = localPos.x + x;
                    int checkY = localPos.y + y;

                    // ��ġ�� Ÿ���� �־�� �� (storeFloorTile�� ���ƾ� ��)
                    Vector3Int cellPos = new Vector3Int(checkX + _origin.x, checkY + _origin.y, 0);
                    if (tilemap.GetTile(cellPos) != storeFloorTile)
                        return false;

                    // �̹� �ٸ� �������� ��ġ�Ǿ� �ִٸ� �Ұ���
                    if (_grid[checkX, checkY])
                        return false;
                }
            }

            return true;
        }
        /// <summary>
        /// ���� ������ ��ġ
        /// </summary>
        public void PlaceItem(Vector2Int worldCellPos, Vector2Int size)
        {
            Vector3Int vec3 = new Vector3Int(worldCellPos.x, worldCellPos.y, 0);
            Vector2Int localPos = WorldToGridIndex(vec3);

            for (int x = 0; x < size.x; x++)
            {
                for (int y = 0; y < size.y; y++)
                {
                    _grid[localPos.x + x, localPos.y + y] = true;
                }
            }
        }

        public void RemoveItem(Vector2Int worldCellPos, Vector2Int size)
        {
            Vector3Int vec3 = new Vector3Int(worldCellPos.x, worldCellPos.y, 0);
            Vector2Int localPos = WorldToGridIndex(vec3);

            for (int x = 0; x < size.x; x++)
            {
                for (int y = 0; y < size.y; y++)
                {
                    _grid[localPos.x + x, localPos.y + y] = false;
                }
            }
        }

        /// <summary>
        /// �� ��ġ�� �迭 �ε����� ��ȯ
        /// </summary>
        public Vector2Int WorldToGridIndex(Vector3Int cellPos)
        {
            return new Vector2Int(cellPos.x - _origin.x, cellPos.y - _origin.y);
        }

        /// <summary>
        /// Ÿ�ϸ� �׸��� �������� Ȯ��
        /// </summary>
        public bool IsInsideGrid(Vector2Int index, Vector2Int size)
        {
            return index.x >= 0 && index.y >= 0 &&
                   index.x + size.x <= _gridWidth &&
                   index.y + size.y <= _gridHeight;
        }



        private void OnDrawGizmos()
        {
            if (_grid == null) return;

            Gizmos.color = Color.green;
            for (int x = 0; x < _gridWidth; x++)
            {
                for (int y = 0; y < _gridHeight; y++)
                {
                    if (_grid[x, y])
                    {
                        Vector3 worldPos = tilemap.CellToWorld(new Vector3Int(x + _origin.x, y + _origin.y, 0)) + Vector3.one * _tileSize / 2f;
                        Gizmos.DrawCube(worldPos, new Vector3(_tileSize, _tileSize, 0.1f));
                    }
                }
            }
        }


    }
}
