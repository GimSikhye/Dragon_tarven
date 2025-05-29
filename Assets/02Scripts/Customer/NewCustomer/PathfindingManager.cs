using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using DalbitCafe.Deco;
using System.Collections;

public class PathfindingManager : MonoSingleton<PathfindingManager>
{
    public class Node
    {
        public Vector2Int Position;
        public float G; // 실제 비용
        public float H; // 휴리스틱
        public float F => G + H;
        public Node Parent;

        public Node(Vector2Int pos, float g, float h, Node parent)
        {
            Position = pos;
            G = g;
            H = h;
            Parent = parent;
        }
    }

    [SerializeField] private Tilemap tilemap; 
    private TileBase walkableTile;
    private GridManager gridManager;
    private void OnEnable()
    {
        Debug.Log($"[{GetType().Name}] OnEnable 호출됨");
    }


    private void Start()
    {
        StartCoroutine(WaitForGridManager());

    }

    private IEnumerator WaitForGridManager()
    {
        while (GridManager.Instance == null)
        {
            Debug.Log("GridManager 초기화 대기 중...");
            yield return null;
        }

        gridManager = GridManager.Instance;
        walkableTile = tilemap.GetTile(tilemap.cellBounds.min); // 타일 직접 참조
        Debug.Log("PathfindingManager 초기화 완료");
    }
    public List<Vector3> FindPath(Vector3 startWorld, Vector3 endWorld)
    {
        if (tilemap == null)
        {
            Debug.LogError(" PathfindingManager 초기화 오류: tilemap이 null입니다. GameObject '1FFloor'가 존재하지 않거나 Tilemap 컴포넌트가 없습니다.");
            return null;
        }

        if (gridManager == null)
        {
            Debug.LogError(" PathfindingManager 초기화 오류: gridManager가 null입니다. GridManager.Instance가 올바르게 초기화되지 않았습니다.");
            return null;
        }

        if (walkableTile == null)
        {
            Debug.LogError(" PathfindingManager 초기화 오류: walkableTile이 null입니다. 타일이 Resources에서 로드되지 않았거나 경로가 잘못되었습니다.");
            return null;
        }

        Vector3Int startCell = tilemap.WorldToCell(startWorld);
        Debug.Log(($"start Cell Null : { startCell == null}"));
        Vector3Int endCell = tilemap.WorldToCell(endWorld);
        Debug.Log(($"end Cell Null : {endCell == null}"));

        Vector2Int start = new(startCell.x, startCell.y);
        Vector2Int end = new(endCell.x, endCell.y);

        var openSet = new PriorityQueue<Node>();
        var closedSet = new HashSet<Vector2Int>();

        Node startNode = new(start, 0, Heuristic(start, end), null);
        openSet.Enqueue(startNode, startNode.F);

        while (openSet.Count > 0)
        {
            Node current = openSet.Dequeue();
            if (current.Position == end)
            {
                return ReconstructPath(current);
            }

            closedSet.Add(current.Position);

            foreach (var dir in GetNeighbors())
            {
                Vector2Int neighborPos = current.Position + dir;


                if (closedSet.Contains(neighborPos)) continue;

                Vector3Int cell = new(neighborPos.x, neighborPos.y, 0);
                var t = tilemap.GetTile(cell);

                // 여기서 먼저 출력
                Debug.Log($"[A*] 위치 {cell}: {(t == null ? "null" : t.name)} / 기준: {(walkableTile == null ? "null" : walkableTile.name)}");


                if (tilemap.GetTile(cell) != walkableTile) continue;


                Vector2Int localGridIndex = neighborPos - gridManager.TilemapOrigin;

                if (!gridManager.IsInsideGrid(localGridIndex, Vector2Int.one))
                {
                    Debug.LogWarning($"[A*] {neighborPos} → Grid 밖: {localGridIndex}");
                    continue;
                }

                if (gridManager.IsOccupied(localGridIndex))
                {
                    Debug.LogWarning($"[A*] {neighborPos} → 아이템에 막힘");
                    continue;
                }

                float tentativeG = current.G + 1f;
                Node neighbor = new(neighborPos, tentativeG, Heuristic(neighborPos, end), current);
                openSet.Enqueue(neighbor, neighbor.F);
            }
        }

        return null;
    }

    private List<Vector3> ReconstructPath(Node node)
    {
        List<Vector3> path = new();
        Node current = node;

        while (current != null)
        {
            Vector3Int cell = new(current.Position.x, current.Position.y, 0);
            Vector3 worldPos = tilemap.GetCellCenterWorld(cell);
            path.Add(worldPos);
            current = current.Parent;
        }

        path.Reverse();
        return path;
    }

    private float Heuristic(Vector2Int a, Vector2Int b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y); // Manhattan Distance
    }

    private static readonly Vector2Int[] directions =
    {
        Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right
    };

    private IEnumerable<Vector2Int> GetNeighbors() => directions;
}
