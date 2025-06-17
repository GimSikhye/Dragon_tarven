using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Tilemaps;
using DalbitCafe.Deco;

public class CustomerSpawner : MonoBehaviour
{
    [SerializeField] private List<GameObject> customerPrefabs;
    [SerializeField] private Transform[] streetSpawns;
    [SerializeField] private Transform counter;
    [SerializeField] private Transform entrance;

    private int maxCustomerCount;
    private List<GameObject> activeCustomers = new();

    private bool isStoreBusy = false;
    private float spawnInterval = 4f;

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
            yield return null;
            pathfinder = FindObjectOfType<PathfindingManager>();
        }

        // 이제 바로 DraggableItem에서 SubCategory 접근
        maxCustomerCount = DecorateManager.Instance.GetPlacedItems()
           .Count(item => item.SubCategory is InteriorType type && type == InteriorType.Chair);

        Debug.Log($"등록된 의자 개수: {maxCustomerCount}");


        StartCoroutine(SpawnLoop());
    }

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

    public Vector3 GetEntrancePosition() => entrance.position;

    public Vector3 GetRandomStreetPosition()
    {
        for (int i = 0; i < 30; i++)
        {
            Transform t = streetSpawns[Random.Range(0, streetSpawns.Length)];
            Vector3 offset = new Vector3(0f, Random.Range(-1f, 1f), 0f);
            Vector3 pos = t.position + offset;
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
            Debug.Log($"[Spawner] DraggableItem: {item.name}, SubCategory: {item.SubCategory}");
        }

        var availableChairs = allItems
            .Where(item =>
                item.SubCategory is InteriorType type && type == InteriorType.Chair &&
                !item.IsOccupied)
            .ToList();

        if (availableChairs.Count == 0)
        {
            Debug.LogWarning("사용 가능한 의자가 없습니다.");
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

    private IEnumerator SpawnLoop()
    {
        while (true)
        {
            Debug.Log("[Spawner] SpawnLoop 실행");
            if (!IsDecorateMode() && activeCustomers.Count < maxCustomerCount)
            {
                Debug.Log("손님 생성");
                SpawnCustomer();
            }
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private bool IsDecorateMode()
    {
        if (DecorateManager.Instance == null) return false;
        return DecorateManager.Instance.IsDecorateMode;
    }

    private void SpawnCustomer()
    {
        GameObject prefab = customerPrefabs[Random.Range(0, customerPrefabs.Count)];
        if (prefab == null) return;

        GameObject customer = Instantiate(prefab, transform);
        var movement = customer.GetComponent<CustomerMovement>();
        if (movement == null) return;

        var state = customer.GetComponent<CustomerStateMachine>();
        if (state == null) return;

        movement.SetTilemapData(outdoorTilemap, storeTilemap, outdoorWalkableTile, storeWalkableTile);
        movement.SetSpawner(this);
        movement.SetPathfinder(pathfinder);

        state.Init();
    }

    public void OnCustomerSeated()
    {
        isStoreBusy = false;
    }
}
