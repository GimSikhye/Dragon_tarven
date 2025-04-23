using System.Collections;
using UnityEngine;

namespace DalbitCafe.Customer
{
    public class CustomerSpawner : MonoBehaviour
    {
        [SerializeField] private CustomerPool customerPool;
        [SerializeField] private float minSpawnTime = 5f;
        [SerializeField] private float maxSpawnTime = 10f;

        private void Start()
        {
            StartCoroutine(SpawnCustomers());
        }

        private IEnumerator SpawnCustomers()
        {
            while (true)
            {
                yield return new WaitForSeconds(Random.Range(minSpawnTime, maxSpawnTime));
                GameObject customer = customerPool.SpawnCustomer();
            }
        }
    }

}
