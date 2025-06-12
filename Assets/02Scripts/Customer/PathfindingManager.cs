using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using DalbitCafe.Deco;

public class PathfindingManager : MonoBehaviour
{
    public class Node
    {
        public Vector2Int Position;
        public float G; // ���� ���
        public float H; // �޸���ƽ
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
    [SerializeField] private TileBase walkableTile;

    private GridManager gridManager;

    private int maxIteration = 10000;
    public bool IsInitialized { get; private set; } = false;

    private void Start()
    {
        StartCoroutine(WaitForGridManager());
    }

    private IEnumerator WaitForGridManager()
    {
        while (GridManager.Instance == null || !GridReady())
        {
            Debug.Log("[PathfindingManager] GridManager �ʱ�ȭ ��� ��...");
            yield return null;
        }

        gridManager = GridManager.Instance;
        tilemap = gridManager.tilemap;
        walkableTile = gridManager.storeFloorTile;

        if (tilemap == null || walkableTile == null)
        {
            Debug.LogError("[PathfindingManager] �ʵ� ����!");
        }

        Debug.Log("[PathfindingManager] �ʱ�ȭ �Ϸ�");
        IsInitialized = true;
    }

    private bool GridReady()
    {
        return GridManager.Instance.tilemap != null && GridManager.Instance.TilemapOrigin != null;
    }

    public List<Vector3> FindPathInTilemap(Tilemap map, TileBase walkable, Vector3 startWorld, Vector3 endWorld)
    {
        Debug.Log($"[A*] Ž�� ����: {startWorld} �� {endWorld}");

        if (map == null || walkable == null)
        {
            Debug.LogError("[A*] Ÿ�ϸ� �Ǵ� Walkable Ÿ���� null�Դϴ�.");
            return null;
        }

        Vector3Int startCell = map.WorldToCell(startWorld);
        Vector3Int endCell = map.WorldToCell(endWorld);

        Vector2Int start = new(startCell.x, startCell.y);
        Vector2Int end = new(endCell.x, endCell.y);

        var openSet = new PriorityQueue<Node>();
        var closedSet = new HashSet<Vector2Int>();

        Node startNode = new(start, 0, Heuristic(start, end), null);
        openSet.Enqueue(startNode, startNode.F);

        int iteration = 0;

        while (openSet.Count > 0)
        {
            if (++iteration > maxIteration)
            {
                Debug.LogError("[A*] ���� ���� ����: �ִ� Ž�� Ƚ�� �ʰ�");
                break;
            }

            Node current = openSet.Dequeue();

            if (current.Position == end)
                return ReconstructPath(current, map);

            closedSet.Add(current.Position);

            foreach (var dir in GetNeighbors())
            {
                Vector2Int neighborPos = current.Position + dir;
                Vector3Int cell = new(neighborPos.x, neighborPos.y, 0);

                if (closedSet.Contains(neighborPos)) continue;

                TileBase currentTile = map.GetTile(cell);
                if (currentTile != walkable) continue;

                float tentativeG = current.G + 1f;
                Node neighbor = new(neighborPos, tentativeG, Heuristic(neighborPos, end), current);
                openSet.Enqueue(neighbor, neighbor.F);
            }
        }

        Debug.LogWarning("[A*] ��θ� ã�� ���߽��ϴ�.");
        return null;
    }

    private List<Vector3> ReconstructPath(Node node, Tilemap map)
    {
        List<Vector3> path = new();
        Node current = node;

        while (current != null)
        {
            Vector3Int cell = new(current.Position.x, current.Position.y, 0);
            Vector3 worldPos = map.GetCellCenterWorld(cell);
            path.Add(worldPos);
            current = current.Parent;
        }

        path.Reverse();
        return path;
    }

    private float Heuristic(Vector2Int a, Vector2Int b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }

    private static readonly Vector2Int[] directions =
    {
        Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right
    };

    private IEnumerable<Vector2Int> GetNeighbors() => directions;
}
