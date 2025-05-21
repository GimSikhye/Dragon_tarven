using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class CustomerSpawner : MonoSingleton<CustomerSpawner>
{
    [SerializeField] private List<GameObject> customerPrefabs;
    [SerializeField] private Transform[] streetSpawns; // 좌우 거리 위치
    [SerializeField] private Transform counter;
    [SerializeField] private Transform entrance;
    [SerializeField] private List<Transform> seatPositions; // 의자 리스트
    [SerializeField] private int maxCustomerCount = 3; // 돌아다니는 최대 손님 수(얘가 문제- 좌석 수에 따라서 손님 수가 늘어나야 함.
    private List<GameObject> activeCustomers = new(); // 현재 씬 내 존재하는 손님 목록

    private bool isStoreBusy = false; // 가게가 바쁘면(손님이 계산을 하고 있으면 그때는 기다리는 타임이 없게 하기 위해서, 손닝미 들어가지 않게 하기)
    private float spawnInterval = 4f; // 생성 간격

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
        return entrance.position;
    }
    public Vector3 GetRandomStreetPosition()
    {
        Transform t = streetSpawns[Random.Range(0, streetSpawns.Length)]; // 반대방향도 넣기
        Vector3 offset = new Vector3(0f, Random.Range(-1f, 1f), 0f);
        return t.position + offset;
    }

    public Vector3 GetOppositeStreetPosition(Vector3 from) // opposite : 반대방향 return
    {
        return streetSpawns[0].position == from ? streetSpawns[1].position : streetSpawns[0].position;
    }

    public Vector3 GetCounterPosition()
    {
        return counter.position;
    }

    public Vector3 GetAvailableSeatPosition() // 가능한 좌석 위치
    {
        foreach(var seat in seatPositions)
        {
            // 자리 비어있는지 체크 필요 (간단하게 비워두고 사용)
            return seat.position;
        }

        // 기본 좌석 반환
        return seatPositions[0]. position;
    }

    void Start()
    {
        StartCoroutine(SpawnLoop());
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
        
        var state = customer.GetComponent<CustomerStateMachine>();
        state.Init();
    }

    public void OnCustomerSeated()
    {
        isStoreBusy = false;
    }
}
