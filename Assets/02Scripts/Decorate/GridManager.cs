using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

namespace DalbitCafe.Deco
{// 코드 먼저 (3개스크립트) 이해하고 회전 버튼 연결해야함
    public class GridManager : MonoBehaviour
    {
        public Tilemap tilemap;         // 바닥 
        public Tilemap wallTilemap;     // 벽
        public TileBase storeFloorTile; // 이 타일만 있는 곳에 배치 가능
        public TileBase wallTile;       // 벽 타일(예: spr_tile_wall)
        [SerializeField] private float _tileSize = 0.5f;

        private bool[,] _grid;
        private int _gridWidth;
        private int _gridHeight;
        private Vector3Int _origin; // 타일맵 cellBounds.min 저장
        public Vector2Int TilemapOrigin => new Vector2Int(_origin.x, _origin.y);

        public bool IsOccupied(Vector2Int gridIndex)
        {
            if (gridIndex.x < 0 || gridIndex.y < 0 ||
                gridIndex.x >= _gridWidth || gridIndex.y >= _gridHeight)
            {
                Debug.LogWarning($"[GridManager] IsOccupied: 범위를 벗어난 좌표 ({gridIndex.x}, {gridIndex.y})");
                return true; // 범위 밖은 배치 불가 처리
            }

            return _grid[gridIndex.x, gridIndex.y];
        }


        public static GridManager Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
            InitTile();

        }

        private void OnEnable()
        {
            Debug.Log($"[{GetType().Name}] OnEnable 호출됨");
        }


        private void InitTile(Scene scene, LoadSceneMode sceneMode)
        {
            if(scene.name == "GameScene")
            {
                Debug.Log("GirdManager 초기화");
                GameObject tile = GameObject.Find("1FFloor");
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

        private void InitTile()
        {
            //Debug.Log("GirdManager 초기화");

            GameObject tileObj = GameObject.Find("1FFloor");
            if (tileObj == null)
            {
                Debug.LogError("[GridManager] 1FFloor GameObject를 찾을 수 없습니다.");
                return;
            }

            tilemap = tileObj.GetComponent<Tilemap>();
            if (tilemap == null)
            {
                Debug.LogError("[GridManager] Tilemap 컴포넌트를 찾을 수 없습니다.");
                return;
            }

            storeFloorTile = Resources.Load<TileBase>("spr_tile_floor");
            if (storeFloorTile == null)
            {
                Debug.LogError("[GridManager] spr_tile_floor 타일을 Resources에서 불러오지 못했습니다.");
                return;
            }

            tilemap.CompressBounds();
            BoundsInt bounds = tilemap.cellBounds;
            _origin = bounds.min;
            _gridWidth = bounds.size.x;
            _gridHeight = bounds.size.y;

            _grid = new bool[_gridWidth, _gridHeight];

            //Debug.Log("[GridManager] 초기화 완료");
        }


        /// <summary>
        /// 배치 가능한지 확인
        /// </summary>
        // 기본 바닥 검사용 (기존 코드와 호환)
        public bool CanPlaceItem(Vector2Int position, Vector2Int size)
        {
            // 기본은 FloorGrid 검사
            return CanPlaceItem(position, size, InteriorType.Decoration);
        }

        // 새 버전 (InteriorType 직접 지정)
        public bool CanPlaceItem(Vector2Int position, Vector2Int size, InteriorType type)
        {
            if (type == InteriorType.WallDecoration)
            {
                return CanPlaceWallItem(position, size); // 벽 전용 검사
            }
            else
            {
                // 기존 FloorGrid 검사 로직
                Vector3Int vec3 = new Vector3Int(position.x, position.y, 0);
                Vector2Int localPos = WorldToGridIndex(vec3);

                if (!IsInsideGrid(localPos, size))
                    return false;

                for (int x = 0; x < size.x; x++)
                {
                    for (int y = 0; y < size.y; y++)
                    {
                        int checkX = localPos.x + x;
                        int checkY = localPos.y + y;

                        // 타일 존재 확인
                        Vector3Int cell = new Vector3Int(checkX + _origin.x, checkY + _origin.y, 0);
                        if (tilemap.GetTile(cell) != storeFloorTile)
                            return false;

                        // 이미 점유된 셀인지 확인
                        if (_grid[checkX, checkY])
                            return false;
                    }
                }
                return true;
            }
        }

        public void PlaceWallItem(Vector2Int worldCellPos, Vector2Int size)
        {
            Debug.Log($"[GridManager] Wall 아이템 배치 기록: {worldCellPos} size {size}");
            // 벽 전용이므로 현재는 단순히 로그만 남기고,
            // 필요하다면 별도의 wallGridOccupied[,] 배열 만들어 기록
        }

        public void RemoveWallItem(Vector2Int worldCellPos, Vector2Int size)
        {
            Debug.Log($"[GridManager] Wall 아이템 제거 기록: {worldCellPos} size {size}");
        }





        /// <summary>
        /// 실제 아이템 배치
        /// </summary>
        public void PlaceItem(Vector2Int worldCellPos, Vector2Int size)
        {
            Vector3Int vec3 = new Vector3Int(worldCellPos.x, worldCellPos.y, 0);
            Vector2Int localPos = WorldToGridIndex(vec3);

            for (int x = 0; x < size.x; x++)
            {
                for (int y = 0; y < size.y; y++)
                {
                    int gx = localPos.x + x;
                    int gy = localPos.y + y;

                    if (gx < 0 || gy < 0 || gx >= _gridWidth || gy >= _gridHeight)
                    {
                        Debug.LogWarning($"[GridManager] PlaceItem: 범위를 벗어난 좌표 ({gx}, {gy}) → 무시");
                        continue;
                    }

                    _grid[gx, gy] = true;
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
                    int gx = localPos.x + x;
                    int gy = localPos.y + y;

                    if (gx < 0 || gy < 0 || gx >= _gridWidth || gy >= _gridHeight)
                    {
                        Debug.LogWarning($"[GridManager] RemoveItem: 범위를 벗어난 좌표 ({gx}, {gy}) → 무시");
                        continue;
                    }

                    _grid[gx, gy] = false;
                }
            }
        }

        /// <summary>
        /// 셀 위치를 배열 인덱스로 변환
        /// </summary>
        public Vector2Int WorldToGridIndex(Vector3Int cellPos)
        {
            return new Vector2Int(cellPos.x - _origin.x, cellPos.y - _origin.y);
        }

        /// <summary>
        /// 타일맵 그리드 내부인지 확인
        /// </summary>
        public bool IsInsideGrid(Vector2Int index, Vector2Int size)
        {
            return index.x >= 0 && index.y >= 0 &&
                   index.x + size.x <= _gridWidth &&
                   index.y + size.y <= _gridHeight;
        }


        // GridManager.cs
        public bool CanPlaceWallItem(Vector2Int worldCellPos, Vector2Int size)
        {
            GameObject wallObj = GameObject.Find("WallGrid");
            if (wallObj == null)
            {
                Debug.LogError("[GridManager] WallGrid 오브젝트를 찾을 수 없습니다.");
                return false;
            }

            Tilemap[] wallTilemaps = wallObj.GetComponentsInChildren<Tilemap>();
            if (wallTilemaps.Length == 0)
            {
                Debug.LogError("[GridManager] WallGrid 자식에 Tilemap이 없습니다.");
                return false;
            }

            // 배치하려는 셀 좌표 기록용
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append("[GridManager] WallGrid 배치 가능 검사 셀들: ");

            for (int x = 0; x < size.x; x++)
            {
                for (int y = 0; y < size.y; y++)
                {
                    Vector3Int cellPos = new Vector3Int(worldCellPos.x + x, worldCellPos.y + y, 0);
                    bool hasTile = false;

                    foreach (var tilemap in wallTilemaps)
                    {
                        if (tilemap.GetTile(cellPos) != null)
                        {
                            hasTile = true;
                            break;
                        }
                    }

                    if (!hasTile)
                    {
                        Debug.Log($"[GridManager] WallGrid의 어떤 Tilemap에도 타일 없음: {cellPos}");
                        return false;
                    }

                    // 이 셀은 배치 가능
                    sb.Append(cellPos + " ");
                }
            }

            Debug.Log(sb.ToString()); // 최종적으로 가능한 셀들 출력
            return true;
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
