using DalbitCafe.Customer;
using UnityEditor.Tilemaps;
using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.Tilemaps;

// �̵� + ��� + Flip + �ִϸ��̼�
// ���� �մ��� �ֹ� ��/ �ڸ� �̵� ���̸� �� �մ�x
public class CustomerMovement : MonoBehaviour
{
     private float moveSpeed = 2f;
     [SerializeField] private Animator animator;
     [SerializeField] private SpriteRenderer spriteRenderer;

     private Tilemap outdoorTilemap;
     private Tilemap storeTilemap;
     private TileBase outdoorWalkableTile; // spr_tile_brick
     private TileBase storeWalkableTile;   // spr_tile_floor


    private Vector3 target;
    private bool isMoving;
    private Action onArrive;
    [SerializeField] private List<Vector3> path;
    private int pathIndex;
    private List<Vector3> debugPath;


    public void WalkRandomly()
    {
        Vector3 spawnPos = CustomerSpawner.Instance.GetRandomStreetPosition();
        transform.position = spawnPos;

        Vector3Int spawnCell = outdoorTilemap.WorldToCell(spawnPos);
        TileBase tile = outdoorTilemap.GetTile(spawnCell);
        Debug.Log($"[�����] �� ���� ��ġ {spawnCell} Ÿ��: {tile?.name}");

        Vector3 entrance = CustomerSpawner.Instance.GetEntrancePosition();

        // path�� �� �޾ƿ;� A* ����˴ϴ�!
        path = PathfindingManager.Instance.FindPathInTilemap(
            outdoorTilemap, outdoorWalkableTile, transform.position, entrance);

        debugPath = path;

        if (path == null || path.Count == 0)
        {
            Debug.LogWarning("��� ����: �Ÿ� �� �Ա�");
            isMoving = false;
            return;
        }

        pathIndex = 0;
        isMoving = true;
        onArrive = () => CustomerSpawner.Instance.TryEnterCustomer(this);
    }

    public void SetTilemapData(Tilemap outdoor, Tilemap store, TileBase outdoorTile, TileBase storeTile)
    {
        outdoorTilemap = outdoor;
        storeTilemap = store;
        outdoorWalkableTile = outdoorTile;
        storeWalkableTile = storeTile;
    }

    public void MoveToEntrance(Action onDone)
    {
        Vector3 entrancePos = CustomerSpawner.Instance.GetEntrancePosition();
        MoveTo(outdoorTilemap, outdoorWalkableTile, entrancePos, onDone);
    }

    public void MoveToCounter(Action onDone)
    {
        Vector3 target = CustomerSpawner.Instance.GetCounterPosition();
        path = PathfindingManager.Instance.FindPathInTilemap(
            storeTilemap, storeWalkableTile, transform.position, target);
        debugPath = path;
        SetMovePath(onDone);
    }

    public void MoveToSeat(Action onDone)
    {
        Vector3 seat = CustomerSpawner.Instance.GetAvailableSeatPosition();
        path = PathfindingManager.Instance.FindPathInTilemap(
            storeTilemap, storeWalkableTile, transform.position, seat);
        debugPath = path;
        SetMovePath(onDone);
    }

    public void LeaveStore(Action onDone)
    {
        Vector3 entrance = CustomerSpawner.Instance.GetEntrancePosition();
        Vector3 exit = CustomerSpawner.Instance.GetRandomStreetPosition();

        // 1�ܰ�: ���� ���� �� entrance
        var toEntrance = PathfindingManager.Instance.FindPathInTilemap(
            storeTilemap, storeWalkableTile, transform.position, entrance);

        // 2�ܰ�: entrance �� �Ÿ� (Outdoor)
        var toStreet = PathfindingManager.Instance.FindPathInTilemap(
            outdoorTilemap, outdoorWalkableTile, entrance, exit);

        path = new List<Vector3>();
        if (toEntrance != null) path.AddRange(toEntrance);
        if (toStreet != null) path.AddRange(toStreet);

        debugPath = path;
        SetMovePath(onDone);
    }


    public void PlayIdleAnimation()
    {
        animator.Play("Back_Idle_Stand");
        //spriteRenderer.flipX = true;
    }

    public void Sit()
    {
        // ���� ���⿡ ����
        animator.Play("Front_Idle_Sit");
        //Front_Sit: �ɴ� �ִϸ��̼�
        //Front_Idle_Sit: �ɾ��ִ� �ִϸ��̼�
    }

    private void MoveTo(Tilemap tilemap, TileBase walkable, Vector3 destination, Action callback)
    {
        onArrive = callback;
        path = PathfindingManager.Instance.FindPathInTilemap(tilemap, walkable, transform.position, destination);
        debugPath = path;

        if (path == null || path.Count == 0)
        {
            Debug.LogWarning($"[Customer] ��� ����! ����: {transform.position} �� ����: {destination}");
            isMoving = false;
            return;
        }

        pathIndex = 0;
        isMoving = true;
    }




    private void Update()
    {
        if (!isMoving || path == null || pathIndex >= path.Count) return;

        Vector3 next = path[pathIndex];
        Vector3 dir = (next - transform.position).normalized;
        transform.position += dir * moveSpeed * Time.deltaTime;

        if (Vector3.Distance(transform.position, next) < 0.05f)
        {
            pathIndex++;
            if (pathIndex >= path.Count)
            {
                isMoving = false;
                onArrive?.Invoke();
            }
        }

        UpdateAnimation(dir);
    }


    private void UpdateAnimation(Vector3 dir)
    {
        if(dir.y > 0)
        {
            animator.Play("Back_Walk");
            spriteRenderer.flipX = (dir.x > 0 ? false : true);
        }
        else
        {
            animator.Play("Front_Walk");
            spriteRenderer.flipX = (dir.x > 0 ? true : false);

        }
    }

    private void SetMovePath(Action onDone)
    {
        if (path == null || path.Count == 0)
        {
            Debug.LogWarning("��� ����!");
            isMoving = false;
            return;
        }

        // ��� �ð�ȭ
        for (int i = 0; i < path.Count - 1; i++)
        {
            Debug.DrawLine(path[i], path[i + 1], Color.red, 5f); // ������ ���ڴ� duration (Scene�� �� �� ���� �׸���)
        }

        pathIndex = 0;
        isMoving = true;
        onArrive = onDone;
    }


    private void OnDrawGizmos()
    {
        if (debugPath == null || debugPath.Count < 2) return;

        Gizmos.color = Color.cyan;
        for (int i = 0; i < debugPath.Count - 1; i++)
        {
            Gizmos.DrawLine(debugPath[i], debugPath[i + 1]);
            Gizmos.DrawSphere(debugPath[i], 0.05f);
        }
    }
}
