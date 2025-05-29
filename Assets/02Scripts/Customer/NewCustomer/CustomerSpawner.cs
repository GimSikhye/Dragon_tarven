using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Tilemaps;

public class CustomerSpawner : MonoSingleton<CustomerSpawner>
{
    [SerializeField] private List<GameObject> customerPrefabs;
    [SerializeField] private Transform[] streetSpawns; // �¿� �Ÿ� ��ġ
    [SerializeField] private Transform counter;
    [SerializeField] private Transform entrance;
    [SerializeField] private List<Transform> seatPositions; // ���� ����Ʈ
    [SerializeField] private int maxCustomerCount = 3; // ���ƴٴϴ� �ִ� �մ� ��(�갡 ����- �¼� ���� ���� �մ� ���� �þ�� ��.
    private List<GameObject> activeCustomers = new(); // ���� �� �� �����ϴ� �մ� ���

    private bool isStoreBusy = false; // ���԰� �ٻڸ�(�մ��� ����� �ϰ� ������ �׶��� ��ٸ��� Ÿ���� ���� �ϱ� ���ؼ�, �մ׹� ���� �ʰ� �ϱ�)
    private float spawnInterval = 4f; // ���� ����

    [SerializeField] private Tilemap outdoorTilemap;
    [SerializeField] private Tilemap storeTilemap;
    [SerializeField] private TileBase outdoorWalkableTile;
    [SerializeField] private TileBase storeWalkableTile;


    void Start()
    {
        StartCoroutine(WaitThenSpawn());
    }

    private IEnumerator WaitThenSpawn()
    {
        while (!PathfindingManager.IsInitialized)
        {
            Debug.Log("[CustomerSpawner] PathfindingManager �ʱ�ȭ ��� ��...");
            yield return null;
        }

        StartCoroutine(SpawnLoop());
    }

    // ���� �õ�
    public void TryEnterCustomer(CustomerMovement movement)
    {
        if(isStoreBusy)
        {
            movement.LeaveStore(() => // ���Ը� ������
            {
                Destroy(movement.gameObject);
                activeCustomers.Remove(movement.gameObject); // ����Ʈ������ ����

            });
            return;
        }
        // �ƴ϶�� ���� ����
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

    public Vector3 GetAvailableSeatPosition()
    {
        if (seatPositions == null || seatPositions.Count == 0)
        {
            Debug.LogError("[CustomerSpawner] �¼� ����Ʈ�� ��� �ֽ��ϴ�!");
            return entrance.position; // fallback
        }

        return seatPositions[0].position;
    }


    private IEnumerator SpawnLoop()
    {
        while(true)
        {
            if(activeCustomers.Count < maxCustomerCount) 
            {
                SpawnCustomer();
            }

            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void SpawnCustomer()
    {
        GameObject prefab = customerPrefabs[Random.Range(0, customerPrefabs.Count)];
        GameObject customer = Instantiate(prefab);

        var movement = customer.GetComponent<CustomerMovement>();
        movement.SetTilemapData(outdoorTilemap, storeTilemap, outdoorWalkableTile, storeWalkableTile);

        var state = customer.GetComponent<CustomerStateMachine>();
        state.Init();
    }


    public void OnCustomerSeated()
    {
        isStoreBusy = false;
    }
}
