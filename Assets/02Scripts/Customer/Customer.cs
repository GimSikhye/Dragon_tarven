using UnityEngine;
using System.Collections;

public class Customer : MonoBehaviour
{
    public enum CustomerState { Entering, Ordering, Exiting, Despawning }
    private CustomerState state;

    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private Transform[] pathPoints; // 이동 경로 (입구 → 계산대 → 출구)
    private int currentPointIndex = 0;

    private CustomerPool customerPool;

    private void Start()
    {
        customerPool = FindObjectOfType<CustomerPool>(); // 손님 풀 찾기
        StartCoroutine(CustomerRoutine());
    }

    private IEnumerator CustomerRoutine()
    {
        // 1. 가게 입구로 이동
        state = CustomerState.Entering;
        yield return MoveToNextPoint();

        // 2. 계산대에서 주문 (랜덤 대기 시간)
        state = CustomerState.Ordering;
        yield return new WaitForSeconds(Random.Range(2f, 5f));

        // 3. 출구로 이동
        state = CustomerState.Exiting;
        yield return MoveToNextPoint();

        // 4. 화면 밖으로 나가기 (이동 후 풀로 반환)
        state = CustomerState.Despawning;
        yield return MoveToNextPoint();
        customerPool.ReturnCustomer(gameObject);
    }

    private IEnumerator MoveToNextPoint()
    {
        Vector3 target = pathPoints[currentPointIndex].position;
        while (Vector3.Distance(transform.position, target) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);
            yield return null;
        }

        currentPointIndex++;
    }
}
