using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class CustomerSpawner : MonoSingleton<CustomerSpawner>
{
    [SerializeField] private List<GameObject> customerPrefabs;
    [SerializeField] private Transform[] streetSpawns; // 좌우 거리 위치
    [SerializeField] private Transform counter;
    [SerializeField] private List<Transform> seatPositions;

    private bool isStoreBusy = false; // 뭐하는 변수인지

    public void TryEnterCustomer(CustomerMovement movement)
    {
        if(isStoreBusy)
        {
            movement.LeaveStore(() => Destroy(movement.gameObject));
            return;
        }

        isStoreBusy = true;
        movement.GetComponent<CustomerStateMachine>().SetState(CustomerState.Entering);
    }
    public Vector3 GetRandomStreetPosition()
    {
        Transform t = streetSpawns[Random.Range(0, streetSpawns.Length)];
        Vector3 offset = new Vector3(0f, Random.Range(-1f, 1f), 0f);
        return t.position + offset;
    }

    public Vector3 GetOppositeStreetPosition(Vector3 from) // opposite : 반대
    {
        return streetSpawns[0].position == from ? streetSpawns[1].position : streetSpawns[0].position;
    }

    public Vector3 GetCounterPosition()
    {
        return counter.position;
    }

    public Vector3 GetAvailableSeatPosition()
    {
        foreach(var seat in seatPositions)
        {
            // 자리 비어있는지 체크 피료 (간단하게 비워두고 사용)
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
            if(!isStoreBusy)
            {
                SpawnCustomer();
            }

            yield return new WaitForSeconds(4f);
        }
    }

    private void SpawnCustomer()
    {
        GameObject prefab = customerPrefabs[Random.Range(0, customerPrefabs.Count)];
        GameObject customer = Instantiate(prefab);
        var state = customer.GetComponent<CustomerStateMachine>();
        state.Init();
    }

    public void OonCustomerSeated()
    {
        isStoreBusy = false;
    }
}
