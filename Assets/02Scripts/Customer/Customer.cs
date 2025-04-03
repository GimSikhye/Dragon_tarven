using UnityEngine;
using UnityEngine.AI;  // NavMeshAgent를 사용하기 위해 추가
using DalbitCafe.Operations;
using System.Collections.Generic;
using UnityEngine.UI;

namespace DalbitCafe.Customer
{
    public class Customer : MonoBehaviour
    {
        [Header("길 찾기")]
        private NavMeshAgent _agent; // NavMeshAgent로 대체
        private Vector3 _targetDestination;  // 목적지 저장 변수
        private Transform _cashDesk; // 이동할 목표 지점
        private Transform _outside;
        private CustomerPool _customerPool;

        [Header("주문 관련")]
        [SerializeField] private GameObject _speechBalloon;
        [SerializeField] private SpriteRenderer _orderMenuSpriteRenderer;
        [SerializeField] private Sprite _angrySprite;

        private CoffeeData _randomCoffee; // 주문한 커피 저장
        private CoffeeMachine _orderedFromMachine; // 주문한 커피머신 저장
        [SerializeField] private bool _isOrdering = false;


        void Start()
        {
            _agent = GetComponent<NavMeshAgent>(); // NavMeshAgent 컴포넌트 가져오기
            _customerPool = FindAnyObjectByType<CustomerPool>();
            _agent.updateRotation = false;
            _agent.updateUpAxis = false;
            Intialize(); // 목표 설정

        }

        public void MoveTo(Transform destination)
        {
            _targetDestination = destination.position; // 목적지 저장
            _agent.SetDestination(_targetDestination);
        }

        private void Intialize()
        {
            _cashDesk = GameObject.Find("Cashdesk")?.transform; // "Destination" 게임 오브젝트를 찾아 목표로 설정
            _outside = GameObject.Find("Outside")?.transform;

            MoveTo(_cashDesk);

        }

        private void Update()
        {
            if (!_agent.pathPending && _agent.remainingDistance <= _agent.stoppingDistance) // 목적지에 도착했을 때 실행
            {
                if (_targetDestination == _cashDesk.position)
                {
                    Debug.Log("계산대에 도착했습니다");
                    StartOrdering();
                }
                else if (_targetDestination == _outside.position)
                {
                    Debug.Log("outside");
                    _speechBalloon.SetActive(false);
                    _customerPool.ReturnCustomer(this.gameObject);
                }
            }
        }
        // 주문을 시작하는 함수
        void StartOrdering()
        {
            _isOrdering = true;
            _agent.isStopped = true; // 이동 중지
            Debug.Log("주문을 시작합니다.");

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
                    _orderMenuSpriteRenderer.sprite = _randomCoffee.MenuIcon;
                }
                else
                {
                    _speechBalloon.SetActive(true);
                    _orderMenuSpriteRenderer.sprite = _angrySprite;

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
                    _isOrdering = false;
                    _speechBalloon.SetActive(false);
                    LeaveStore();

                }
            }


        }

        void LeaveStore()
        {
            _agent.isStopped = false; // 이동 재개
            //_speechBalloon.SetActive(false);
            MoveTo(_outside);

        }
    }
}
