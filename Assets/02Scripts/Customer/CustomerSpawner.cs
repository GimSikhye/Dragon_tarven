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
    [SerializeField] private Transform[] streetSpawns; // �¿� �Ÿ� ��ġ
    [SerializeField] private Transform counter;
    [SerializeField] private Transform entrance;
    [SerializeField] private List<Transform> seatPositions; // ���� ����Ʈ
    private int maxCustomerCount; // runtime�� ����
    private List<GameObject> activeCustomers = new(); // ���� �� �� �����ϴ� �մ� ���

    private bool isStoreBusy = false; // ���԰� �ٻڸ�(�մ��� ����� �ϰ� ������ �׶��� ��ٸ��� Ÿ���� ���� �ϱ� ���ؼ�, �մ׹� ���� �ʰ� �ϱ�)
    private float spawnInterval = 4f; // ���� ����

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
            Debug.Log("[CustomerSpawner] PathfindingManager �ʱ�ȭ ��� ��...");
            yield return null;
            pathfinder = FindObjectOfType<PathfindingManager>();
        }

        Debug.Log("[CustomerSpawner] �մ� ���� ����");

        // ���⼭ maxCustomerCount ����!
        maxCustomerCount = FindObjectsOfType<DraggableItem>()
            .Count(item =>
                item != null &&
                item.TryGetComponent<ItemMeta>(out var meta) &&
                meta.SubCategory.ToString() == "Chair");

        Debug.Log($"[CustomerSpawner] ��� ������ ���� ��: {maxCustomerCount}");

        StartCoroutine(SpawnLoop());
    }


    // ���� �õ�
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
        Debug.Log($"[�����] �Ա� ��ġ {entranceCell} Ÿ��: {tile?.name}");

        return entrance.position;
    }
    public Vector3 GetRandomStreetPosition()
    {
        for (int i = 0; i < 30; i++) // �ִ� 30�� �õ�
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

        Debug.LogWarning("GetRandomStreetPosition() ����: walkable Ÿ�� �� ã��, �⺻ ��ġ ��ȯ");
        return streetSpawns[0].position;
    }


    public Vector3 GetOppositeStreetPosition(Vector3 from) // opposite : �ݴ���� return
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
                Debug.Log($"[���ڰ˻�] {item.name} - {meta.SubCategory}");
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
            Debug.LogWarning("��� ������ ���ڰ� �����ϴ�.");
            return null;
        }

        int index = Random.Range(0, availableChairs.Count);
        var chosen = availableChairs[index];
        chosen.SetOccupied(true); // ���� ó��

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
            Debug.Log("[Spawner] SpawnLoop ����");

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
            Debug.LogError("[Spawner] customerPrefab�� null�Դϴ�!");
            return;
        }

        GameObject customer = Instantiate(prefab);
        Debug.Log($"[Spawner] �մ� ������: {customer.name}");

        var movement = customer.GetComponent<CustomerMovement>();
        if (movement == null)
        {
            Debug.LogError("[Spawner] CustomerMovement ������Ʈ ����!");
            return;
        }

        var state = customer.GetComponent<CustomerStateMachine>();
        if (state == null)
        {
            Debug.LogError("[Spawner] CustomerStateMachine ������Ʈ ����!");
            return;
        }

        movement.SetTilemapData(outdoorTilemap, storeTilemap, outdoorWalkableTile, storeWalkableTile);
        movement.SetSpawner(this);
        movement.SetPathfinder(pathfinder);

        Debug.Log("[Spawner] Init ȣ�� ��");
        state.Init();
        Debug.Log("[Spawner] Init ȣ�� �Ϸ�");
    }

    public void OnCustomerSeated()
    {
        isStoreBusy = false;
    }
}
