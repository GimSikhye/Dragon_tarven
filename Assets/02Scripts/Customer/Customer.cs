using UnityEngine;
using Pathfinding; // A* Pathfinding 네임스페이스 추가

public class Customer : MonoBehaviour
{
    private Transform target; // 이동할 목표 지점
    private AIPath aiPath; // A* 경로 탐색을 위한 AIPath 컴포넌트

    void Start()
    {
        aiPath = GetComponent<AIPath>(); // AIPath 컴포넌트 가져오기
        if (aiPath == null)
        {
            Debug.LogError("AIPath 컴포넌트가 없습니다!");
            return;
        }

        SetNextDestination(); // 첫 번째 목표 설정
    }

    void SetNextDestination()
    {
        GameObject pathParent = GameObject.Find("PathPoints");
        if (pathParent != null)
        {
            Transform[] pathPoints = new Transform[pathParent.transform.childCount];
            for (int i = 0; i < pathPoints.Length; i++)
            {
                pathPoints[i] = pathParent.transform.GetChild(i);
            }

            if (pathPoints.Length > 0)
            {
                target = pathPoints[0]; // 첫 번째 지점을 목표로 설정
                aiPath.destination = target.position; // A* 이동 목표 설정
            }
        }
    }

    void Update()
    {
        // 목표 지점에 도착하면 다음 지점으로 변경
        if (aiPath.reachedDestination && target != null)
        {
            target = target.GetChild(0); // 다음 목표 지점
            aiPath.destination = target.position;
        }
    }
}
