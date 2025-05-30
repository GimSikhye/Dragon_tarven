using DalbitCafe.Customer;
using UnityEditor.Tilemaps;
using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using DalbitCafe.Deco;

// �̵� + ��� + Flip + �ִϸ��̼�
// ���� �մ��� �ֹ� ��/ �ڸ� �̵� ���̸� �� �մ�x
public class CustomerMovement : MonoBehaviour
{
    private DraggableItem mySeat; // �� �մ��� �¼�

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
    private CustomerSpawner spawner;
    private PathfindingManager pathfinder;

    public void SetSpawner(CustomerSpawner spawner)
    {
        this.spawner = spawner;
    }


    public void WalkRandomly()
    {
        if (pathfinder == null)
        {
            Debug.LogError("[CustomerMovement] pathfinder�� null�Դϴ�! SetPathfinder() ȣ�� �ȵ�!");
            return;
        }

        Vector3 spawnPos = spawner.GetRandomStreetPosition();
        transform.position = spawnPos;

        Vector3Int spawnCell = outdoorTilemap.WorldToCell(spawnPos);
        TileBase tile = outdoorTilemap.GetTile(spawnCell);
        Debug.Log($"[�����] �� ���� ��ġ {spawnCell} Ÿ��: {tile?.name}");

        Vector3 entrance = spawner.GetEntrancePosition();

        // path�� �� �޾ƿ;� A* ����˴ϴ�!
        path = pathfinder.FindPathInTilemap(
            outdoorTilemap, outdoorWalkableTile, transform.position, entrance);

        debugPath = path;

        if (path == null || path.Count == 0)
        {
            Debug.LogWarning("��� ����: �Ÿ� -> �Ա�");
            isMoving = false;
            return;
        }

        pathIndex = 0;
        isMoving = true;
        onArrive = () => spawner.TryEnterCustomer(this);
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
        Vector3 entrancePos = spawner.GetEntrancePosition();
        MoveTo(outdoorTilemap, outdoorWalkableTile, entrancePos, onDone);
    }

    public void MoveToCounter(Action onDone)
    {
        Vector3 target = spawner.GetCounterPosition();
        path = pathfinder.FindPathInTilemap(
            storeTilemap, storeWalkableTile, transform.position, target);
        debugPath = path;
        SetMovePath(onDone);
    }

    public void MoveToSeat(Action onDone)
    {
        mySeat = spawner.GetAvailableSeat();
        if (mySeat == null)
        {
            Debug.LogWarning("[CustomerMovement] �¼� ����, �Ա��� �̵�");
            LeaveStore(onDone);
            return;
        }

        Vector3 seatPos = mySeat.transform.position;

        path = pathfinder.FindPathInTilemap(
            storeTilemap, storeWalkableTile, transform.position, seatPos);
        debugPath = path;
        SetMovePath(onDone);
    }

    // ���� �ʿ�
    public void SetPathfinder(PathfindingManager manager)
    {
        this.pathfinder = manager;
    }


    public void LeaveStore(Action onDone)
    {
        Vector3 entrance = spawner.GetEntrancePosition();
        Vector3 exit = spawner.GetRandomStreetPosition();

        // 1�ܰ�: ���� ���� �� entrance
        var toEntrance = pathfinder.FindPathInTilemap(
            storeTilemap, storeWalkableTile, transform.position, entrance);

        // 2�ܰ�: entrance �� �Ÿ� (Outdoor)
        var toStreet = pathfinder.FindPathInTilemap(
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
        if (mySeat == null) return;

        var meta = mySeat.GetComponent<ItemMeta>();
        if (meta == null) return;

        int index = mySeat.GetComponent<DraggableItem>().DirectionIndex();

        switch (index)
        {
            case 0:
                animator.Play("Front_Idle_Sit");
                spriteRenderer.flipX = true;
                break;
            case 1:
                animator.Play("Front_Idle_Sit");
                spriteRenderer.flipX = false;
                break;
            case 2:
                animator.Play("Back_Idle_Sit");
                spriteRenderer.flipX = true;
                break;
            case 3:
                animator.Play("Back_Idle_Sit");
                spriteRenderer.flipX = false;
                break;
            default:
                animator.Play("Front_Idle_Sit");
                break;
        }

        spawner.OnCustomerSeated(); // isStoreBusy = false ���ֱ�
    }
    public void ReleaseSeat()
    {
        if (mySeat != null)
        {
            mySeat.SetOccupied(false);
            mySeat = null;
        }
    }


    private void MoveTo(Tilemap tilemap, TileBase walkable, Vector3 destination, Action callback)
    {
        onArrive = callback;
        path = pathfinder.FindPathInTilemap(tilemap, walkable, transform.position, destination);
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

    // ���� ������ �մ��� �׳� �� �ݴ� �������� ��������
    public void LeaveImmediately(Action onDone)
    {
        Vector3 exit = spawner.GetOppositeStreetPosition(transform.position);

        path = pathfinder.FindPathInTilemap(
            outdoorTilemap, outdoorWalkableTile, transform.position, exit);

        debugPath = path;

        if (path == null || path.Count == 0)
        {
            Debug.LogWarning("[Customer] ��� ���� ��� ����");
            onDone?.Invoke();
            return;
        }

        SetMovePath(onDone);
    }



    private void UpdateAnimation(Vector3 dir)
    {
        if (dir.y > 0)
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
