using UnityEngine;
using Pathfinding; // A* Pathfinding 네임스페이스 추가

public class Customer : MonoBehaviour
{
    private Transform target; // 이동할 목표 지점
    private AIPath aiPath; // A* 경로 탐색을 위한 AIPath 컴포넌트
    GameObject pathParent;
    Transform[] pathPoints;
    private int currentIndex = 0;
    private CustomerPool custerPool;

    void Start()
    {
        aiPath = GetComponent<AIPath>(); // AIPath 컴포넌트 가져오기
        if (aiPath == null)
        {
            Debug.LogError("AIPath 컴포넌트가 없습니다!");
            return;
        }
        
        custerPool = FindAnyObjectByType<CustomerPool>();
        SetNextDestination(); // 첫 번째 목표 설정
    }

    void SetNextDestination()
    {
        pathParent = GameObject.Find("PathPoints");
        if (pathParent != null)
        {
            pathPoints = new Transform[pathParent.transform.childCount];
            for (int i = 0; i < pathPoints.Length; i++)
            {
                pathPoints[i] = pathParent.transform.GetChild(i);
            }

            if (pathPoints.Length > 0)
            {
                currentIndex = 0; // 시작 시 인덱스 초기화
                target = pathPoints[0]; // 첫 번째 지점을 목표로 설정
                aiPath.destination = target.position; // A* 이동 목표 설정
            }
        }
    }

    void Update() 
    {
        if (aiPath.reachedDestination && target != null)
        {
            currentIndex++;

            if (currentIndex >= pathPoints.Length)
            {
                // 경로를 다 돌았으면 오브젝트 비활성화 후 Pool로 반환
                custerPool.ReturnCustomer(gameObject);
                return;
            }

            target = pathPoints[currentIndex];
            aiPath.destination = target.position;
        }
    }
}
