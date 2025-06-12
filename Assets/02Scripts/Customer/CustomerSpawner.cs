using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Tilemaps;
using DalbitCafe.Deco;
using System.Linq;

public class CustomerSpawner : MonoBehaviour
{
    [SerializeField] private List<GameObject> customerPrefabs;
    [SerializeField] private Transform[] streetSpawns; // 좌우 거리 위치
    [SerializeField] private Transform counter;
    [SerializeField] private Transform entrance;
    [SerializeField] private List<Transform> seatPositions; // 의자 리스트
    private int maxCustomerCount; // runtime에 결정
    private List<GameObject> activeCustomers = new(); // 현재 씬 내 존재하는 손님 목록

    private bool isStoreBusy = false; // 가게가 바쁘면(손님이 계산을 하고 있으면 그때는 기다리는 타임이 없게 하기 위해서, 손닝미 들어가지 않게 하기)
    private float spawnInterval = 4f; // 생성 간격

    [SerializeField] private Tilemap outdoorTilemap;
    [SerializeField] private Tilemap storeTilemap;
    [SerializeField] private TileBase outdoorWalkableTile;
    [SerializeField] private TileBase storeWalkableTile;
    private PathfindingManager pathfinder;

    private DraggableItem assignedSeat;

    void Start()
    {
        StartCoroutine(WaitThenSpawn());
    }

    private IEnumerator WaitThenSpawn()
    {
        pathfinder = FindObjectOfType<PathfindingManager>();

        while (pathfinder == null || !pathfinder.IsInitialized)
        {
            Debug.Log("[CustomerSpawner] PathfindingManager 초기화 대기 중...");
            yield return null;
            pathfinder = FindObjectOfType<PathfindingManager>();
        }

        Debug.Log("[CustomerSpawner] 손님 생성 시작");

        // 여기서 maxCustomerCount 설정!
        maxCustomerCount = FindObjectsOfType<DraggableItem>()
            .Count(item =>
                item != null &&
                item.TryGetComponent<ItemMeta>(out var meta) &&
                meta.SubCategory.ToString() == "Chair");

        Debug.Log($"[CustomerSpawner] 사용 가능한 의자 수: {maxCustomerCount}");

        StartCoroutine(SpawnLoop());
    }


    // 입장 시도
    public void TryEnterCustomer(CustomerMovement movement)
    {
        if (isStoreBusy)
        {
            movement.LeaveImmediately(() =>
            {
                Destroy(movement.gameObject);
                activeCustomers.Remove(movement.gameObject);
            });
            return;
        }

        isStoreBusy = true;
        movement.GetComponent<CustomerStateMachine>().SetState(CustomerState.Entering);
    }

    public Vector3 GetEntrancePosition()
    {
        Vector3Int entranceCell = outdoorTilemap.WorldToCell(entrance.position);
        TileBase tile = outdoorTilemap.GetTile(entranceCell);
        Debug.Log($"[디버그] 입구 위치 {entranceCell} 타일: {tile?.name}");

        return entrance.position;
    }
    public Vector3 GetRandomStreetPosition()
    {
        for (int i = 0; i < 30; i++) // 최대 30번 시도
        {
            Transform t = streetSpawns[Random.Range(0, streetSpawns.Length)];
            Vector3 offset = new Vector3(0f, Random.Range(-1f, 1f), 0f);
            Vector3 pos = t.position + offset;

            Vector3Int cell = outdoorTilemap.WorldToCell(pos);
            TileBase tile = outdoorTilemap.GetTile(cell);

            if (tile == outdoorWalkableTile)
            {
                return pos;
            }
        }

        Debug.LogWarning("GetRandomStreetPosition() 실패: walkable 타일 못 찾음, 기본 위치 반환");
        return streetSpawns[0].position;
    }


    public Vector3 GetOppositeStreetPosition(Vector3 from) // opposite : 반대방향 return
    {
        return streetSpawns[0].position == from ? streetSpawns[1].position : streetSpawns[0].position;
    }

    public Vector3 GetCounterPosition()
    {
        return counter.position;
    }

    public DraggableItem GetAvailableSeat()
    {
        var allItems = FindObjectsOfType<DraggableItem>();
        foreach (var item in allItems)
        {
            if (item.TryGetComponent<ItemMeta>(out var meta))
                Debug.Log($"[의자검사] {item.name} - {meta.SubCategory}");
        }

        var availableChairs = allItems
     .Where(item =>
         item != null &&
         item.TryGetComponent<ItemMeta>(out var meta) &&
         meta.SubCategory is InteriorType type && type == InteriorType.Chair &&
         !item.IsOccupied)
     .ToList();


        if (availableChairs.Count == 0)
        {
            Debug.LogWarning("사용 가능한 의자가 없습니다.");
            return null;
        }

        int index = Random.Range(0, availableChairs.Count);
        var chosen = availableChairs[index];
        chosen.SetOccupied(true); // 점유 처리

        return chosen;
    }



    public Vector3 GetAvailableSeatPosition()
    {
        var seat = GetAvailableSeat();
        return seat != null ? seat.transform.position : entrance.position;
    }

    public DraggableItem GetAssignedSeat() => assignedSeat;

    private IEnumerator SpawnLoop()
    {

        while (true)
        {
            Debug.Log("[Spawner] SpawnLoop 실행");

            if (activeCustomers.Count < maxCustomerCount)
            {
                SpawnCustomer();
            }

            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void SpawnCustomer()
    {
        GameObject prefab = customerPrefabs[Random.Range(0, customerPrefabs.Count)];
        if (prefab == null)
        {
            Debug.LogError("[Spawner] customerPrefab이 null입니다!");
            return;
        }

        GameObject customer = Instantiate(prefab);
        Debug.Log($"[Spawner] 손님 생성됨: {customer.name}");

        var movement = customer.GetComponent<CustomerMovement>();
        if (movement == null)
        {
            Debug.LogError("[Spawner] CustomerMovement 컴포넌트 없음!");
            return;
        }

        var state = customer.GetComponent<CustomerStateMachine>();
        if (state == null)
        {
            Debug.LogError("[Spawner] CustomerStateMachine 컴포넌트 없음!");
            return;
        }

        movement.SetTilemapData(outdoorTilemap, storeTilemap, outdoorWalkableTile, storeWalkableTile);
        movement.SetSpawner(this);
        movement.SetPathfinder(pathfinder);

        Debug.Log("[Spawner] Init 호출 전");
        state.Init();
        Debug.Log("[Spawner] Init 호출 완료");
    }

    public void OnCustomerSeated()
    {
        isStoreBusy = false;
    }
}
