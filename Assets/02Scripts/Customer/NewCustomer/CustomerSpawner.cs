using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Tilemaps;
using DalbitCafe.Deco;
using System.Linq;

public class CustomerSpawner : MonoSingleton<CustomerSpawner>
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

    private DraggableItem assignedSeat;

    void Start()
    {
        StartCoroutine(WaitThenSpawn());
    }

    private IEnumerator WaitThenSpawn()
    {
        while (!PathfindingManager.IsInitialized)
        {
            Debug.Log("[CustomerSpawner] PathfindingManager 초기화 대기 중...");
            yield return null;
        }

        // 좌석 수를 기준으로 maxCustomerCount 자동 결정
        maxCustomerCount = FindObjectsOfType<DraggableItem>()
            .Count(item =>
                item != null &&
                item.TryGetComponent<ItemMeta>(out var meta) &&
                meta.SubCategory.ToString() == "Chair");

        Debug.Log($"[CustomerSpawner] 좌석 수 만큼 손님 수 설정됨: {maxCustomerCount}명");

        StartCoroutine(SpawnLoop());
    }


    // 입장 시도
    public void TryEnterCustomer(CustomerMovement movement)
    {
        if(isStoreBusy)
        {
            movement.LeaveStore(() => // 가게를 떠나면
            {
                Destroy(movement.gameObject);
                activeCustomers.Remove(movement.gameObject); // 리스트에서도 제거

            });
            return;
        }
        // 아니라면 가게 들어가기
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
        var availableChairs = allItems
            .Where(item =>
                item != null &&
                item.TryGetComponent<ItemMeta>(out var meta) &&
                meta.SubCategory.ToString() == "Chair" &&
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
