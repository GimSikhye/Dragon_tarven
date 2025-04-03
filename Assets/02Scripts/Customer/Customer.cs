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
        private CoffeeMachine _orderedMachine; // 주문한 커피머신 저장
        [SerializeField] private bool _isOrdering = false; // 현재 주문중인지


        private void Awake()
        {
            _agent = GetComponent<NavMeshAgent>(); 
            _customerPool = FindAnyObjectByType<CustomerPool>();
            // NavMesh 2d에서는 회전을 막아야 제대로 동작함
            _agent.updateRotation = false;
            _agent.updateUpAxis = false;

            _cashDesk = GameObject.Find("Cashdesk")?.transform; // "Destination" 게임 오브젝트를 찾아 목표로 설정
            _outside = GameObject.Find("Outside")?.transform;
        }

        private void OnEnable()
        {
            _speechBalloon.SetActive(false);
            Initialize();
        }

        public void MoveTo(Transform destination)
        {
            _targetDestination = destination.position; // 목적지 저장
            _agent.SetDestination(_targetDestination);
        }

        private void Initialize()
        {
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
            CoffeeMachine[] machines = FindObjectsOfType<CoffeeMachine>();
            if (machines.Length > 0)
            {
                List<CoffeeMachine> roastingMachines = new List<CoffeeMachine>();
                foreach (var machine in machines)
                {
                    if (machine.CurrentCoffee != null && machine.RemainingMugs > 0) // 로스팅 중인 머신들만 리스트에 추가  
                    {
                        roastingMachines.Add(machine);
                    }
                }

                if (roastingMachines.Count > 0) // 로스팅 중인 머신들이 한 대 이상이라면
                {
                    _orderedMachine = roastingMachines[Random.Range(0, roastingMachines.Count)];
                    _randomCoffee = _orderedMachine.CurrentCoffee;
                    _speechBalloon.SetActive(true);
                    _orderMenuSpriteRenderer.sprite = _randomCoffee.MenuIcon;
                }
                else // 로스팅 중인 머신이 없다면
                {
                    _speechBalloon.SetActive(true);
                    _orderMenuSpriteRenderer.sprite = _angrySprite;

                    LeaveStore();
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
                if (_orderedMachine != null)
                {
                    _orderedMachine.SellCoffee(); // 커피머신의 잔 수 감소
                    _speechBalloon.SetActive(false);
                    LeaveStore();

                }
            }


        }

        void LeaveStore()
        {
            _agent.isStopped = false; // 이동 재개
            _isOrdering = false;
            //_speechBalloon.SetActive(false);
            MoveTo(_outside);

        }
    }
}
