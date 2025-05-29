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


    private int maxIteration = 10000;
    [SerializeField] private Tilemap tilemap;
    private TileBase walkableTile;
    private GridManager gridManager;
    public static bool IsInitialized { get; private set; } = false;

    private void OnEnable()
    {
        Debug.Log($"[{GetType().Name}] OnEnable ȣ���");
    }


    private void Start()
    {
        StartCoroutine(WaitForGridManager());

    }

    private IEnumerator WaitForGridManager()
    {
        while (GridManager.Instance == null || !GridReady())
        {
            Debug.Log("GridManager �ʱ�ȭ ��� ��...");
            yield return null;
        }

        gridManager = GridManager.Instance;
        tilemap = gridManager.tilemap;
        walkableTile = gridManager.storeFloorTile;

        if (walkableTile == null)
        {
            Debug.LogError("[PathfindingManager] walkableTile�� null�Դϴ�.");
        }

        Debug.Log("PathfindingManager �ʱ�ȭ �Ϸ�");
        IsInitialized = true;
    }



    private bool GridReady()
    {
        var gm = GridManager.Instance;
        return gm.tilemap != null && gm.TilemapOrigin != null;
    }
    public List<Vector3> FindPathInTilemap(Tilemap map, TileBase walkable, Vector3 startWorld, Vector3 endWorld)
    {
        Debug.Log($"[A*] Ž�� ����! Start: {startWorld}, End: {endWorld}");

        int iteration = 0;
        if (map == null)
        {
            Debug.LogError("Ÿ�ϸ��� null�Դϴ�.");
            return null;
        }

        if (walkable == null)
        {
            Debug.LogError("walkableTile�� null�Դϴ�.");
            return null;
        }

        Vector3Int startCell = map.WorldToCell(startWorld);
        Vector3Int endCell = map.WorldToCell(endWorld);

        LogSurroundingTiles(map, startCell, "���� ����");
        LogSurroundingTiles(map, endCell, "��ǥ ����");


        Debug.LogWarning($"[A*] ���� ��: {startCell}, Ÿ��: {map.GetTile(startCell)?.name}");
        Debug.LogWarning($"[A*] �� ��: {endCell}, Ÿ��: {map.GetTile(endCell)?.name}");

        if (map.GetTile(startCell) != walkable || map.GetTile(endCell) != walkable)
        {
            Debug.LogWarning($"[A*] ����({startCell}) �Ǵ� ��({endCell})�� walkable Ÿ���� �����ϴ�.");
            //return null;
        }

        Vector2Int start = new(startCell.x, startCell.y);
        Vector2Int end = new(endCell.x, endCell.y);


        var openSet = new PriorityQueue<Node>();
        var closedSet = new HashSet<Vector2Int>();

        Node startNode = new(start, 0, Heuristic(start, end), null);
        openSet.Enqueue(startNode, startNode.F);

        while (openSet.Count > 0)
        {
            if (++iteration > maxIteration)
            {
                Debug.LogError("[A*] ���� ���� ����: �ִ� Ž�� Ƚ�� �ʰ�");
                break;
            }

            Node current = openSet.Dequeue();
            if (current.Position == end)
            {
                return ReconstructPath(current, map);
            }

            closedSet.Add(current.Position);

            foreach (var dir in GetNeighbors())
            {
                Vector2Int neighborPos = current.Position + dir;
                Vector3Int cell = new(neighborPos.x, neighborPos.y, 0);

                if (closedSet.Contains(neighborPos)) continue;

                TileBase currentTile = map.GetTile(cell);
                Debug.Log($"[A*] �˻� ��: {cell} / Ÿ��: {(currentTile == null ? "null" : currentTile.name)}");
                if (currentTile != walkable) continue;

                float tentativeG = current.G + 1f;
                Node neighbor = new(neighborPos, tentativeG, Heuristic(neighborPos, end), current);
                openSet.Enqueue(neighbor, neighbor.F);
            }
        }

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

    private void LogSurroundingTiles(Tilemap map, Vector3Int center, string context)
    {
        Debug.Log($"[�����] �ֺ� Ÿ�� �˻� ({context})");

        for (int y = 1; y >= -1; y--)
        {
            string row = "";
            for (int x = -1; x <= 1; x++)
            {
                Vector3Int pos = new Vector3Int(center.x + x, center.y + y, 0);
                var t = map.GetTile(pos);
                row += t != null ? t.name.Substring(0, Mathf.Min(6, t.name.Length)) + "\t" : "null\t";
            }
            Debug.Log(row);
        }
    }


    private IEnumerable<Vector2Int> GetNeighbors() => directions;
}
