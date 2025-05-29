using DalbitCafe.Customer;
using UnityEditor.Tilemaps;
using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.Tilemaps;

// 이동 + 경로 + Flip + 애니메이션
// 기존 손님이 주문 중/ 자리 이동 중이면 새 손님x
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
        Debug.Log($"[디버그] 고객 스폰 위치 {spawnCell} 타일: {tile?.name}");

        Vector3 entrance = CustomerSpawner.Instance.GetEntrancePosition();

        // path를 꼭 받아와야 A* 실행됩니다!
        path = PathfindingManager.Instance.FindPathInTilemap(
            outdoorTilemap, outdoorWalkableTile, transform.position, entrance);

        debugPath = path;

        if (path == null || path.Count == 0)
        {
            Debug.LogWarning("경로 없음: 거리 → 입구");
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

        // 1단계: 가게 내부 → entrance
        var toEntrance = PathfindingManager.Instance.FindPathInTilemap(
            storeTilemap, storeWalkableTile, transform.position, entrance);

        // 2단계: entrance → 거리 (Outdoor)
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
        // 의자 방향에 따라서
        animator.Play("Front_Idle_Sit");
        //Front_Sit: 앉는 애니메이션
        //Front_Idle_Sit: 앉아있는 애니메이션
    }

    private void MoveTo(Tilemap tilemap, TileBase walkable, Vector3 destination, Action callback)
    {
        onArrive = callback;
        path = PathfindingManager.Instance.FindPathInTilemap(tilemap, walkable, transform.position, destination);
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
