using DalbitCafe.Customer;
using UnityEditor.Tilemaps;
using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using DalbitCafe.Deco;

// 이동 + 경로 + Flip + 애니메이션
// 기존 손님이 주문 중/ 자리 이동 중이면 새 손님x
public class CustomerMovement : MonoBehaviour
{
    private DraggableItem mySeat; // 이 손님의 좌석

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
            Debug.LogError("[CustomerMovement] pathfinder가 null입니다! SetPathfinder() 호출 안됨!");
            return;
        }

        Vector3 spawnPos = spawner.GetRandomStreetPosition();
        transform.position = spawnPos;

        Vector3Int spawnCell = outdoorTilemap.WorldToCell(spawnPos);
        TileBase tile = outdoorTilemap.GetTile(spawnCell);
        Debug.Log($"[디버그] 고객 스폰 위치 {spawnCell} 타일: {tile?.name}");

        Vector3 entrance = spawner.GetEntrancePosition();

        // path를 꼭 받아와야 A* 실행됩니다!
        path = pathfinder.FindPathInTilemap(
            outdoorTilemap, outdoorWalkableTile, transform.position, entrance);

        debugPath = path;

        if (path == null || path.Count == 0)
        {
            Debug.LogWarning("경로 없음: 거리 -> 입구");
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
            Debug.LogWarning("[CustomerMovement] 좌석 없음, 입구로 이동");
            LeaveStore(onDone);
            return;
        }

        Vector3 seatPos = mySeat.transform.position;

        path = pathfinder.FindPathInTilemap(
            storeTilemap, storeWalkableTile, transform.position, seatPos);
        debugPath = path;
        SetMovePath(onDone);
    }

    // 전달 필요
    public void SetPathfinder(PathfindingManager manager)
    {
        this.pathfinder = manager;
    }


    public void LeaveStore(Action onDone)
    {
        Vector3 entrance = spawner.GetEntrancePosition();
        Vector3 exit = spawner.GetRandomStreetPosition();

        // 1단계: 가게 내부 → entrance
        var toEntrance = pathfinder.FindPathInTilemap(
            storeTilemap, storeWalkableTile, transform.position, entrance);

        // 2단계: entrance → 거리 (Outdoor)
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

        spawner.OnCustomerSeated(); // isStoreBusy = false 해주기
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
            Debug.LogWarning($"[Customer] 경로 없음! 시작: {transform.position} → 도착: {destination}");
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

    // 입장 거절된 손님이 그냥 쭉 반대 방향으로 지나가기
    public void LeaveImmediately(Action onDone)
    {
        Vector3 exit = spawner.GetOppositeStreetPosition(transform.position);

        path = pathfinder.FindPathInTilemap(
            outdoorTilemap, outdoorWalkableTile, transform.position, exit);

        debugPath = path;

        if (path == null || path.Count == 0)
        {
            Debug.LogWarning("[Customer] 즉시 퇴장 경로 없음");
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
            Debug.LogWarning("경로 없음!");
            isMoving = false;
            return;
        }

        // 경로 시각화
        for (int i = 0; i < path.Count - 1; i++)
        {
            Debug.DrawLine(path[i], path[i + 1], Color.red, 5f); // 마지막 인자는 duration (Scene에 몇 초 동안 그릴지)
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
