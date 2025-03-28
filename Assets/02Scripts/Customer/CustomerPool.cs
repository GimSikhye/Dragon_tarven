using System.Collections.Generic;
using UnityEngine;

public class CustomerPool : MonoBehaviour
{
    [SerializeField] private GameObject _customerPrefab1;
    [SerializeField] private GameObject _customerPrefab2;
    [SerializeField] private int _poolSize = 5;  // 미리 생성할 손님 수
    [SerializeField] private Transform _spawnPoint; // 손님 스폰 위치
    [SerializeField] private Transform _parent;
    private Queue<GameObject> _customerPool = new Queue<GameObject>();
    
    private void Start()
    {
        InitializePool();
    }

    private void InitializePool()
    {
        for (int i = 0; i < _poolSize; i++)
        {
            GameObject customer = Instantiate(Random.value > 0.5f ? _customerPrefab1 : _customerPrefab2, _spawnPoint.position, Quaternion.identity, _parent);
            customer.SetActive(false);
            _customerPool.Enqueue(customer);
        }
    }

    public GameObject SpawnCustomer()
    {
        if (_customerPool.Count > 0)
        {
            GameObject customer = _customerPool.Dequeue();
            customer.SetActive(true);
            customer.transform.position = _spawnPoint.position;
            return customer;
        }
        return null;
    }

    public void ReturnCustomer(GameObject customer)
    {
        customer.SetActive(false);
        _customerPool.Enqueue(customer);
    }
}
