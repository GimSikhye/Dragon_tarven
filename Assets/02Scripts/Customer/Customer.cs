using UnityEngine;
using Pathfinding;
using System.Collections.Generic;
using DalbitCafe.Operations;

namespace DalbitCafe.Customer
{

    public class Customer : MonoBehaviour
    {
        [Header("길 찾기")]
        private AIPath _aiPath; // A* 경로 탐색을 위한 AIPath 컴포넌트
        private CustomerPool _custmorPool;
        private Transform _target; // 이동할 목표 지점
        private GameObject _pathParent;
        private Transform[] _pathPoints;
        private int _currentIndex = 0;

        [Header("주문 관련")]
        [SerializeField] private GameObject _speechBalloon;
        [SerializeField] private SpriteRenderer _orderMenuSprite;
        private CoffeeData _randomCoffee; // 주문한 커피 저장
        private CoffeeMachine _orderedFromMachine; // 주문한 커피머신 저장
        private bool _isOrdering = false;

        void Start()
        {
            _aiPath = GetComponent<AIPath>();
            _custmorPool = FindAnyObjectByType<CustomerPool>();
            SetNextDestination(); // 첫 번째 목표 설정
        }

        void SetNextDestination()
        {
            _pathParent = GameObject.Find("PathPoints");
            if (_pathParent != null)
            {
                _pathPoints = new Transform[_pathParent.transform.childCount];
                for (int i = 0; i < _pathPoints.Length; i++)
                {
                    _pathPoints[i] = _pathParent.transform.GetChild(i);
                }

                if (_pathPoints.Length > 0)
                {
                    _currentIndex = 0; // 시작 시 인덱스 초기화
                    _target = _pathPoints[0]; // 첫 번째 지점을 목표로 설정
                    _aiPath.destination = _target.position; // A* 이동 목표 설정
                }
            }
        }

        void Update()
        {
            if (_aiPath.reachedDestination && _target != null) //목적지에 도달하였다면
            {
                if (_currentIndex == 1 && !_isOrdering)
                {
                    StartOrdering();
                    _currentIndex++;

                    return;
                }

                _currentIndex++;

                if (_currentIndex >= _pathPoints.Length)
                {
                    // 경로를 다 돌았으면 오브젝트 비활성화 후 Pool로 반환
                    _custmorPool.ReturnCustomer(gameObject);
                    return;
                }

                _target = _pathPoints[_currentIndex];
                _aiPath.destination = _target.position;
            }
        }

        // 주문을 시작하는 함수
        void StartOrdering()
        {
            _isOrdering = true;
            _aiPath.canMove = false; // 이동 중지

            // 모든 커피머신에서 랜덤한 커피 선택
            CoffeeMachine[] coffeeMachines = FindObjectsOfType<CoffeeMachine>();
            if (coffeeMachines.Length > 0)
            {
                List<CoffeeMachine> availableMachines = new List<CoffeeMachine>();
                foreach (var machine in coffeeMachines)
                {
                    if (machine.CurrentCoffee != null && machine.RemainingMugs > 0)
                    {
                        availableMachines.Add(machine);
                    }
                }

                if (availableMachines.Count > 0)
                {
                    _orderedFromMachine = availableMachines[Random.Range(0, availableMachines.Count)];
                    _randomCoffee = _orderedFromMachine.CurrentCoffee;
                    _speechBalloon.SetActive(true);
                    _orderMenuSprite.sprite = _randomCoffee.MenuIcon;
                }
                else
                {
                    LeaveStore();
                    Debug.Log("주문 가능한 커피가 없습니다!");
                }
            }
        }
        // 손님을 터치하면 주문이 완료되고 이동을 재개함
        private void OnMouseDown()
        {
            if (_isOrdering)
            {
                FinishOrder();
            }
        }

        void FinishOrder()
        {
            if (_randomCoffee != null)
            {
                if (_orderedFromMachine != null)
                {
                    _orderedFromMachine.SellCoffee(); // 커피머신의 잔 수 감소
                                                      // currentMenu 업데이트하기
                }
            }

            _isOrdering = false;
            _speechBalloon.SetActive(false);
            _aiPath.canMove = true;

        }

        void LeaveStore()
        {
            _isOrdering = false;
            _speechBalloon.SetActive(false);
            _aiPath.canMove = true;

            SetNextDestination();
        }
    }


}

