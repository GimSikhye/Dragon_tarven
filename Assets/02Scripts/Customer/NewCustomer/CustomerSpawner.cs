using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class CustomerSpawner : MonoSingleton<CustomerSpawner>
{
    [SerializeField] private List<GameObject> customerPrefabs;
    [SerializeField] private Transform[] streetSpawns; // �¿� �Ÿ� ��ġ
    [SerializeField] private Transform counter;
    [SerializeField] private List<Transform> seatPositions;

    private bool isStoreBusy = false; // ���ϴ� ��������

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

    public Vector3 GetOppositeStreetPosition(Vector3 from) // opposite : �ݴ�
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
            // �ڸ� ����ִ��� üũ �Ƿ� (�����ϰ� ����ΰ� ���)
            return seat.position;
        }

        // �⺻ �¼� ��ȯ
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
