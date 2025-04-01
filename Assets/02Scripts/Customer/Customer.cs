using UnityEngine;
using UnityEngine.AI;  // NavMeshAgent를 사용하기 위해 추가
using DalbitCafe.Operations;
using System.Collections.Generic;

namespace DalbitCafe.Customer
{
    public class Customer : MonoBehaviour
    {
        [Header("길 찾기")]
        private NavMeshAgent _navMeshAgent; // NavMeshAgent로 대체
        private CustomerPool _custmorPool;
        private Transform _target; // 이동할 목표 지점

        [Header("주문 관련")]
        [SerializeField] private GameObject _speechBalloon;
        [SerializeField] private SpriteRenderer _orderMenuSprite;
        private CoffeeData _randomCoffee; // 주문한 커피 저장
        private CoffeeMachine _orderedFromMachine; // 주문한 커피머신 저장
        private bool _isOrdering = false;

        void Start()
        {
            _navMeshAgent = GetComponent<NavMeshAgent>(); // NavMeshAgent 컴포넌트 가져오기
            _custmorPool = FindAnyObjectByType<CustomerPool>();
            SetDestination(); // 목표 설정
        }

        void SetDestination()
        {
            _target = GameObject.Find("Cashdesk")?.transform; // "Destination" 게임 오브젝트를 찾아 목표로 설정

            if (_target != null)
            {
                _navMeshAgent.SetDestination(_target.position); // NavMeshAgent로 이동 목표 설정
            }
        }

        void Update()
        {
            if (!_navMeshAgent.pathPending && _navMeshAgent.remainingDistance <= _navMeshAgent.stoppingDistance)
            {
                if (_target != null) //목적지에 도달하였다면
                {
                    if (!_isOrdering)
                    {
                        StartOrdering();
                    }
                }
            }
        }

        // 주문을 시작하는 함수
        void StartOrdering()
        {
            _isOrdering = true;
            _navMeshAgent.isStopped = true; // 이동 중지

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
            _navMeshAgent.isStopped = false; // 이동 재개
        }

        void LeaveStore()
        {
            _isOrdering = false;
            _speechBalloon.SetActive(false);
            _navMeshAgent.isStopped = false; // 이동 재개
        }
    }
}
