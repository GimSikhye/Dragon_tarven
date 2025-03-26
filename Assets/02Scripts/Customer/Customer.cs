using UnityEngine;
using Pathfinding;
using System.Collections.Generic;
using NUnit.Framework; // A* Pathfinding 네임스페이스 추가

public class Customer : MonoBehaviour
{
    private AIPath aiPath; // A* 경로 탐색을 위한 AIPath 컴포넌트
    private CustomerPool custerPool;
    private Transform target; // 이동할 목표 지점
    GameObject pathParent;
    Transform[] pathPoints;
    private int currentIndex = 0;

    [Header("주문 관련")]
    [SerializeField] private GameObject speechBalloon;
    [SerializeField] private SpriteRenderer orderMenuSprite;
    private bool isOrdering = false; 

    void Start()
    {
        aiPath = GetComponent<AIPath>(); // AIPath 컴포넌트 가져오기
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
            if(currentIndex == 1 && !isOrdering)
            {
                StartOrdering();
                return;
            }

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

    // 주문을 시작하는 함수
    void StartOrdering()
    {
        isOrdering = true;
        aiPath.canMove = false; // 이동 중지
        speechBalloon.SetActive(true);

        // 모든 커피머신에서 랜덤한 커피 선택
        CoffeeMachine[] coffeeMachines = FindObjectsOfType<CoffeeMachine>();
        if(coffeeMachines.Length > 0 )
        {
            List<CoffeeData> availableCoffees = new List<CoffeeData>();
            foreach(var machine in coffeeMachines)
            {
                if(machine.CurrentCoffee != null)
                {
                    availableCoffees.Add(machine.CurrentCoffee);
                }
            }

            if(availableCoffees.Count > 0)
            {
                CoffeeData randomCoffee = availableCoffees[Random.Range(0, availableCoffees.Count)];
                orderMenuSprite.sprite = randomCoffee.MenuIcon;
            }
            else
            {
                Debug.Log("주문 가능한 커피가 없습니다!");
            }
        }

    }

    // 손님을 터치하면 주문이 완료되고 이동을 재개함
    private void OnMouseDown()
    {
        if(isOrdering)
        {
            FinishOrder();
        }
    }

    void FinishOrder()
    {
        isOrdering = false;
        speechBalloon.SetActive(false);
        aiPath.canMove = true;
        SetNextDestination();
    }
}
