using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using DalbitCafe.Deco;

public class CustomerMovement : MonoBehaviour
{
    private DraggableItem assignedSeat; // 이 손님의 좌석
    private float moveSpeed = 2f;

    [Header("시각적 컴포넌트")]
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private float seatYOffset = 0.4f;

    [Header("타일맵 컴포넌트")]
    private Tilemap outdoorTilemap;
    private Tilemap storeTilemap;
    private TileBase outdoorWalkableTile; // sidewalkBlock
    private TileBase storeWalkableTile;   // spr_tile_floor

    // 상태
    private bool isMoving;
    private Action onArrive;

    [Header("경로 설정")]
    [SerializeField] private List<Vector3> path;
    private List<Vector3> debugPath;
    private int pathIndex;
    private CustomerSpawner customerSpawner;
    private PathfindingManager pathfinderManager;
    private CustomerStateMachine stateMachine;


    private void Awake()
    {
        stateMachine = GetComponent<CustomerStateMachine>();
    }

    public void SetSpawner(CustomerSpawner spawner)
    {
        this.customerSpawner = spawner; // customerSpawner에서 설정
    }

    // 전달 필요
    public void SetPathfindManager(PathfindingManager manager)
    {
        this.pathfinderManager = manager;
    }

    public void WalkRandomly()
    {
        if (pathfinderManager == null)
        {
            Debug.LogError("[CustomerMovement] pathfinderManager가 null입니다! SetPathfindManager() 호출 안됨!");
            return;
        }

        Vector3 randomSpawnPosition = customerSpawner.GetRandomStreetPosition(); // 스폰 위치를 랜덤 설정함.
        transform.position = randomSpawnPosition;

        Vector3 entrancePosition = customerSpawner.GetEntrancePosition(); // 입구 위치를 가져옴.

        // path를 꼭 받아와야 A* 실행된다.
        path = pathfinderManager.FindPathInTilemap(
            outdoorTilemap, outdoorWalkableTile, transform.position, entrancePosition);

        debugPath = path; // List<Vector3>

        if (path == null || path.Count == 0)
        {
            Debug.LogWarning("[CustomerMovement] 경로 없음: 거리 -> 입구");
            isMoving = false;
            return;
        }

        pathIndex = 0;
        isMoving = true;
        onArrive = () => customerSpawner.TryEnterCustomer(this); // 다시 들어가기? (체크)
    }

    public void SetTilemapData(Tilemap outdoorTilemap, Tilemap storeTilemap, TileBase outdoorWalkableTile, TileBase storeWalkableTile)
    {
        this.outdoorTilemap = outdoorTilemap;
        this.storeTilemap = storeTilemap;
        this.outdoorWalkableTile = outdoorWalkableTile;
        this.storeWalkableTile = storeWalkableTile;
    }

    public void MoveToEntrance(Action onDone)
    {
        Vector3 entrancePosition = customerSpawner.GetEntrancePosition();
        MoveTo(outdoorTilemap, outdoorWalkableTile, entrancePosition, onDone); // 입구(목적지)로 이동, 이동을 마쳤을 경우 onDone 실행
    }

    public void MoveToCounter(Action onDone)
    {
        Vector3 counterPosition = customerSpawner.GetCounterPosition(); // 카운터 위치를 가져옴
        path = pathfinderManager.FindPathInTilemap(
            storeTilemap, storeWalkableTile, transform.position, counterPosition); // 가게 내에서 카운터까지의 경로를 계산(배치된 아이템 공간 제외)
        debugPath = path;
        SetMovePath(onDone); // 체크. path는 어디에 쓰이는지 봐야 함.
    }

    public void MoveToSeat(Action onDone)
    {
        assignedSeat = customerSpawner.GetAvailableSeat(); // 사용 가능한 좌석 점유
        if (assignedSeat == null)
        {
            Debug.LogWarning("[CustomerMovement] 좌석 없음, 카페 나가기");
            LeaveStore(onDone);
            return;
        }

        Vector3 seatPosition = assignedSeat.transform.position;

        path = pathfinderManager.FindPathInTilemap(
            storeTilemap, storeWalkableTile, transform.position, seatPosition);
        debugPath = path;
        SetMovePath(onDone);
    }

    public void Sit()
    {
        if (assignedSeat == null)
        {
            Debug.Log("[CustomerMovement] assignedSeat가 null입니다! 착석 실패");
            return;
        }

        int seatRotationIndex = assignedSeat.GetComponent<DraggableItem>().RotationIndex;

        // 회전 인덱스에 따른 보정값 적용
        Vector3 seatPosition = assignedSeat.transform.position;
        transform.position = seatPosition + GetSittingOffset(seatRotationIndex);

        animator.SetInteger("Direction", seatRotationIndex);
        animator.SetBool("IsSitting", true);

        ApplySittingFlipX(seatRotationIndex);
        customerSpawner.OnCustomerSeated();
    }

    private Vector3 GetSittingOffset(int seatRotationIndex)
    {
        switch (seatRotationIndex)
        {
            case 0: return new Vector3(0.2f, 0.4f, 0f);     //  오른쪽 아래
            case 1: return new Vector3(-0.26f, 0.34f, 0f);  //  왼쪽 아래
            case 2: return new Vector3(0f, 0.4f, 0f);       //  왼쪽 위
            case 3: return new Vector3(0f, 0.4f, 0f);       //  오른쪽 위
            default: return new Vector3(0f, 0.4f, 0f);      // 기본값
        }
    }

    private void ApplySittingFlipX(int seatRotationIndex)
    {
        switch (seatRotationIndex)
        {
            case 0: // 오른쪽 아래
                spriteRenderer.flipX = true;
                break;
            case 1: // 왼쪽 아래
                spriteRenderer.flipX = false;
                break;
            case 2: // 왼쪽 위
                spriteRenderer.flipX = true;
                break;
            case 3: // 오른쪽 위
                spriteRenderer.flipX = false;
                break;
            default: // 기본 값
                spriteRenderer.flipX = false;
                break;
        }
    }

    public void ReleaseSeat() // 좌석 해제
    {
        if (assignedSeat != null)
        {
            assignedSeat.SetOccupied(false); // 해당 좌석의 점유 false로 바꿈
            assignedSeat = null; // 할당된 좌석 해제
        }
    }

    public void LeaveStore(Action onDone)
    {
        Vector3 entrancePosition = customerSpawner.GetEntrancePosition();
        Vector3 streetExitPosition = customerSpawner.GetRandomStreetPosition();

        // 1단계: 가게 내부 → entrance
        var pathToEntrance = pathfinderManager.FindPathInTilemap(
            storeTilemap, storeWalkableTile, transform.position, entrancePosition);

        // 2단계: entrance → 거리 (Outdoor)
        var pathToStreet = pathfinderManager.FindPathInTilemap(
            outdoorTilemap, outdoorWalkableTile, entrancePosition, streetExitPosition);

        path = new List<Vector3>();
        if (pathToEntrance != null) path.AddRange(pathToEntrance);
        if (pathToStreet != null) path.AddRange(pathToStreet);

        debugPath = path;
        SetMovePath(onDone);
    }

    public void PlayIdleAnimation()
    {
        animator.Play("Back_Idle_Stand");
    }

    private void MoveTo(Tilemap tilemap, TileBase walkableTile, Vector3 destination, Action callback)
    {
        onArrive = callback;
        path = pathfinderManager.FindPathInTilemap(tilemap, walkableTile, transform.position, destination);
        debugPath = path;

        if (path == null || path.Count == 0)
        {
            Debug.LogWarning($"[CustomerMovement] 경로 없음! 시작: {transform.position} → 도착: {destination}");
            isMoving = false;
            return;
        }

        pathIndex = 0;
        isMoving = true;
    }

    private void Update()
    {
        if (!isMoving || path == null || pathIndex >= path.Count) return;

        Vector3 currentTarget = path[pathIndex];
        Vector3 direction = (currentTarget - transform.position).normalized;
        transform.position += direction * moveSpeed * Time.deltaTime;

        if (Vector3.Distance(transform.position, currentTarget) < 0.05f) // next에 가까워졌다면
        {
            pathIndex++;
            if (pathIndex >= path.Count)
            {
                isMoving = false;
                onArrive?.Invoke();
                return;
            }
        }

        if (isMoving && stateMachine.CurrentState is CustomerState.WalkingAround
        or CustomerState.Entering or CustomerState.MovingToSeat)
        {
            UpdateAnimation(direction);
        }

    }

    // 입장 거절된 손님이면, 그냥 쭉 반대 방향으로 지나가기
    public void LeaveImmediately(Action onDone) // 즉시 떠나기
    {
        Vector3 exit = customerSpawner.GetOppositeStreetPosition(transform.position);

        path = pathfinderManager.FindPathInTilemap(
            outdoorTilemap, outdoorWalkableTile, transform.position, exit);

        debugPath = path;

        if (path == null || path.Count == 0)
        {
            Debug.LogWarning("[CustomerMovement] 즉시 퇴장 경로 없음");
            onDone?.Invoke();
            return;
        }

        SetMovePath(onDone);
    }

    private void UpdateAnimation(Vector3 direction)
    {
        if (direction.y > 0)
        {
            animator.Play("Back_Walk"); // 뒷면이 보이게 걷기
            spriteRenderer.flipX = (direction.x > 0 ? false : true);
        }
        else
        {
            animator.Play("Front_Walk"); // 앞면이 보이게 걷기
            spriteRenderer.flipX = (direction.x > 0 ? true : false);
        }
    }

    private void SetMovePath(Action onDone) // 경로 설정
    {
        if (path == null || path.Count == 0)
        {
            Debug.LogWarning("[CustomerMovement] 경로 없음!");
            isMoving = false;
            return;
        }

        pathIndex = 0;
        isMoving = true;
        onArrive = onDone;
    }

    private void OnDrawGizmos()
    {
        //if (debugPath == null || debugPath.Count < 2) return;

        //Gizmos.color = Color.cyan; // 하늘색

        //for (int i = 0; i < debugPath.Count - 1; i++)
        //{
        //    Gizmos.DrawLine(debugPath[i], debugPath[i + 1]);
        //    Gizmos.DrawSphere(debugPath[i], 0.05f); // 0.05f 반지름의 원을 그림.
        //}
    }
}
