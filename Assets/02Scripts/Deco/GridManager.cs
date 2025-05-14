using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

namespace DalbitCafe.Deco
{// 코드 먼저 (3개스크립트) 이해하고 회전 버튼 연결해야함
    public class GridManager : MonoSingleton<GridManager>
    {
        public Tilemap tilemap;
        [SerializeField] private TileBase storeFloorTile; // 이 타일만 있는 곳에 배치 가능
        [SerializeField] private float _tileSize = 0.5f;

        private bool[,] _grid;
        private int _gridWidth;
        private int _gridHeight;
        private Vector3Int _origin; // 타일맵 cellBounds.min 저장

        private void Start()
        {
           
        }

        private void OnEnable()
        {
            SceneManager.sceneLoaded += InitTile;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= InitTile;

        }

        private void InitTile(Scene scene, LoadSceneMode sceneMode)
        {
            if(scene.name == "GameScene")
            {
                GameObject tile = GameObject.Find("StoreFloor");
                tilemap = tile.GetComponent<Tilemap>();
                storeFloorTile = Resources.Load<TileBase>("spr_tile_floor");

                tilemap.CompressBounds(); // 꼭 해주기 (불필요한 빈 타일 좌표 제거)

                BoundsInt bounds = tilemap.cellBounds;
                _origin = bounds.min; // 타일맵 시작점 저장
                _gridWidth = bounds.size.x;
                _gridHeight = bounds.size.y;

                _grid = new bool[_gridWidth, _gridHeight];
            }
        }

        /// <summary>
        /// 배치 가능한지 확인
        /// </summary>
        public bool CanPlaceItem(Vector2Int worldCellPos, Vector2Int size)
        {
            Vector2Int localPos = WorldToGridIndex(worldCellPos); // 전달받은 셀 좌표를 로컬 인덱스로 변환 (WorldToGridIndex)

            if (!IsInsideGrid(localPos, size)) return false; // size 만큼 루프를 돌면서 그 셀에 타일이 있는지, 다른 아이템이 이미 있는지 확인

            for (int x = 0; x < size.x; x++)
            {
                for (int y = 0; y < size.y; y++)
                {
                    int checkX = localPos.x + x;
                    int checkY = localPos.y + y;

                    // 배치된 타일이 있어야 함 (storeFloorTile과 같아야 함)
                    Vector3Int cellPos = new Vector3Int(checkX + _origin.x, checkY + _origin.y, 0);
                    if (tilemap.GetTile(cellPos) != storeFloorTile)
                        return false;

                    // 이미 다른 아이템이 배치되어 있다면 불가능
                    if (_grid[checkX, checkY])
                        return false;
                }
            }

            return true;
        }
        /// <summary>
        /// 실제 아이템 배치
        /// </summary>
        public void PlaceItem(Vector2Int worldCellPos, Vector2Int size)
        {
            Vector2Int localPos = WorldToGridIndex(worldCellPos);

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
            Vector2Int localPos = WorldToGridIndex(worldCellPos);

            for (int x = 0; x < size.x; x++)
            {
                for (int y = 0; y < size.y; y++)
                {
                    _grid[localPos.x + x, localPos.y + y] = false;
                }
            }
        }

        /// <summary>
        /// 셀 위치를 배열 인덱스로 변환
        /// </summary>
        private Vector2Int WorldToGridIndex(Vector2Int worldCellPos)
        {
            return new Vector2Int(worldCellPos.x - _origin.x, worldCellPos.y - _origin.y);
        }

        /// <summary>
        /// 타일맵 그리드 내부인지 확인
        /// </summary>
        private bool IsInsideGrid(Vector2Int index, Vector2Int size)
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
