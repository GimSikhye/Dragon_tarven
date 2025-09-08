using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Tilemaps;
using DalbitCafe.Deco;

public class CustomerSpawner : MonoBehaviour
{
    [Header("손님 프리팹")]
    [SerializeField] private List<GameObject> customerPrefabs;

    [Header("목적지 Transform")]
    [SerializeField] private Transform[] streetSpawns;
    [SerializeField] private Transform counter;
    [SerializeField] private Transform entrance;

    // 손님 관련 변수들
    private int maxCustomerCount;
    private List<GameObject> activeCustomers = new();
    private float spawnInterval = 4f; // 생성 간격

    private bool isStoreBusy = false;

    [Header("경로 타일맵들")]
    [SerializeField] private Tilemap outdoorTilemap;
    [SerializeField] private Tilemap storeTilemap;
    [SerializeField] private TileBase outdoorWalkableTile;
    [SerializeField] private TileBase storeWalkableTile;

    private PathfindingManager pathfindManager;
    private DraggableItem assignedSeat;

    void Start()
    {
        StartCoroutine(InitializeSpawner());
    }

    private IEnumerator InitializeSpawner()
    {
        Debug.Log("[CustomerSpawner] WaitThenSpawn 실행");
        pathfindManager = FindFirstObjectByType<PathfindingManager>();

        while (pathfindManager == null || !pathfindManager.IsInitialized)
        {
            yield return null; // 다음 프레임에 실행됨
            pathfindManager = FindFirstObjectByType<PathfindingManager>();
        }

        // 이제 바로 DraggableItem에서 SubCategory 접근
        maxCustomerCount = DecorateManager.Instance.GetPlacedItems()
           .Count(item => item.SubCategory is InteriorType type && type == InteriorType.Chair);

        //Debug.Log($"등록된 의자 개수: {maxCustomerCount}");

        StartCoroutine(SpawnLoop());
    }
    private IEnumerator SpawnLoop()
    {
        while (true)
        {
            if (!IsDecorateMode() && activeCustomers.Count < maxCustomerCount)
            {
                //Debug.Log("손님 생성");
                SpawnCustomer();
            }
            yield return new WaitForSeconds(spawnInterval);
        }
    }
    
    private void SpawnCustomer()
    {
        GameObject prefab = customerPrefabs[Random.Range(0, customerPrefabs.Count)];
        if (prefab == null) return;

        GameObject customer = Instantiate(prefab, transform);
        CustomerMovement movement = customer.GetComponent<CustomerMovement>();
        if (movement == null) return;

        CustomerStateMachine state = customer.GetComponent<CustomerStateMachine>();
        if (state == null) return;

        movement.SetTilemapData(outdoorTilemap, storeTilemap, outdoorWalkableTile, storeWalkableTile);
        movement.SetSpawner(this);
        movement.SetPathfindManager(pathfindManager);

        state.Init();
    }

    public void TryEnterCustomer(CustomerMovement movement) // 입구 들어가려고 시도함.
    {
        if (isStoreBusy)
        {
            movement.LeaveImmediately(() => // Done Action에 할당
            {
                Destroy(movement.gameObject);
                activeCustomers.Remove(movement.gameObject);
            });
            return;
        }

        isStoreBusy = true; // 손님이 들어가는 중엔 다른 손님이 들어오지 못하도록 함.
        movement.GetComponent<CustomerStateMachine>().SetState(CustomerState.Entering);
    }

    public Vector3 GetEntrancePosition() => entrance.position;

    public Vector3 GetRandomStreetPosition()
    {
        for (int i = 0; i < 30; i++)
        {
            Transform randomStreetSpawn = streetSpawns[Random.Range(0, streetSpawns.Length-1)];
            Vector3 yOffset = new Vector3(0f, Random.Range(-1f, 1f), 0f);
            Vector3 pos = randomStreetSpawn.position + yOffset; //
            Vector3Int cell = outdoorTilemap.WorldToCell(pos);
            TileBase tile = outdoorTilemap.GetTile(cell);

            if (tile == outdoorWalkableTile)
                return pos;
        }
        return streetSpawns[0].position;
    }

    public Vector3 GetOppositeStreetPosition(Vector3 from)
    {
        return streetSpawns[0].position == from ? streetSpawns[1].position : streetSpawns[0].position;
    }

    public Vector3 GetCounterPosition() => counter.position;

    public DraggableItem GetAvailableSeat()
    {
        var allItems = FindObjectsOfType<DraggableItem>();

        foreach (var item in allItems)
        {
            //Debug.Log($"[Spawner] DraggableItem: {item.name}, SubCategory: {item.SubCategory}");
        }

        var availableChairs = allItems
            .Where(item =>
                item.SubCategory is InteriorType type && type == InteriorType.Chair &&
                !item.IsOccupied)
            .ToList();

        if (availableChairs.Count == 0)
        {
            //Debug.LogWarning("사용 가능한 의자가 없습니다.");
            return null;
        }

        int index = Random.Range(0, availableChairs.Count);
        var chosen = availableChairs[index];
        chosen.SetOccupied(true);
        return chosen;
    }

    public Vector3 GetAvailableSeatPosition()
    {
        var seat = GetAvailableSeat();
        return seat != null ? seat.transform.position : entrance.position;
    }

    public DraggableItem GetAssignedSeat() => assignedSeat;


    private bool IsDecorateMode()
    {
        if (DecorateManager.Instance == null) return false;
        return DecorateManager.Instance.IsDecorateMode;
    }

 
    public void OnCustomerSeated()
    {
        isStoreBusy = false;
    }
}
