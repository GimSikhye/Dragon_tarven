using UnityEngine;
using UnityEngine.AI;  // NavMeshAgent�� ����ϱ� ���� �߰�
using DalbitCafe.Operations;
using System.Collections.Generic;
using UnityEngine.UI;

namespace DalbitCafe.Customer
{
    public class Customer : MonoBehaviour
    {
        [SerializeField] private ParticleSystem coinParticlePrefab;

        [Header("�� ã��")]
        private NavMeshAgent _agent; // NavMeshAgent�� ��ü
        private Vector3 _targetDestination;  // ������ ���� ����
        private Transform _cashDesk; // �̵��� ��ǥ ����
        private Transform _outside;
        private CustomerPool _customerPool;

        [Header("�ֹ� ����")]
        [SerializeField] private GameObject _speechBalloon;
        [SerializeField] private SpriteRenderer _orderMenuSpriteRenderer;
        [SerializeField] private Sprite _angrySprite;

        private CoffeeData _randomCoffee; // �ֹ��� Ŀ�� ����
        private CoffeeMachine _orderedMachine; // �ֹ��� Ŀ�Ǹӽ� ����
        [SerializeField] private bool _isOrdering = false; // ���� �ֹ�������
        private Animator _animator; 

        private void Awake()
        {
            _agent = GetComponent<NavMeshAgent>(); 
            _customerPool = FindAnyObjectByType<CustomerPool>();
            // NavMesh 2d������ ȸ���� ���ƾ� ����� ������
            _agent.updateRotation = false;
            _agent.updateUpAxis = false;

            _cashDesk = GameObject.Find("Cashdesk")?.transform; // "Destination" ���� ������Ʈ�� ã�� ��ǥ�� ����
            _outside = GameObject.Find("Outside")?.transform;

            _animator = GetComponent<Animator>();
        }

        private void OnEnable()
        {
            _speechBalloon.SetActive(false);
            Initialize();
        }

        public void MoveTo(Transform destination)
        {
            _targetDestination = destination.position; // ������ ����
            _agent.SetDestination(_targetDestination);
        }

        private void Initialize()
        {
            MoveTo(_cashDesk);
        }

        private void Update()
        {
            // �̵� ������ ������� �ִϸ��̼� ���� ������Ʈ
            Vector3 velocity = _agent.velocity;

            if (velocity.magnitude > 0.1f)
            {
                // �ӵ� ������ ����ȭ
                Vector3 direction = velocity.normalized;

                _animator.SetFloat("MoveX", direction.x);
                _animator.SetFloat("MoveY", direction.y);
                _animator.SetBool("isMoving", true);
            }
            else
            {
                _animator.SetBool("isMoving", false);
            }

            // ������ ���� üũ
            if (!_agent.pathPending && _agent.remainingDistance <= _agent.stoppingDistance)
            {
                if (_targetDestination == _cashDesk.position)
                {
                    StartOrdering();
                }
                else if (_targetDestination == _outside.position)
                {
                    _speechBalloon.SetActive(false);
                    _customerPool.ReturnCustomer(this.gameObject);
                }
            }
        }

        // �ֹ��� �����ϴ� �Լ�
        void StartOrdering()
        {
            _isOrdering = true;
            _agent.isStopped = true; // �̵� ����

            // ��� Ŀ�Ǹӽſ��� ������ Ŀ�� ����
            CoffeeMachine[] machines = FindObjectsOfType<CoffeeMachine>();
            if (machines.Length > 0)
            {
                List<CoffeeMachine> roastingMachines = new List<CoffeeMachine>();
                foreach (var machine in machines)
                {
                    if (machine.CurrentCoffee != null && machine.RemainingMugs > 0) // �ν��� ���� �ӽŵ鸸 ����Ʈ�� �߰�  
                    {
                        roastingMachines.Add(machine);
                    }
                }

                if (roastingMachines.Count > 0) // �ν��� ���� �ӽŵ��� �� �� �̻��̶��
                {
                    _orderedMachine = roastingMachines[Random.Range(0, roastingMachines.Count)];
                    _randomCoffee = _orderedMachine.CurrentCoffee;
                    _speechBalloon.SetActive(true);
                    _orderMenuSpriteRenderer.sprite = _randomCoffee.MenuIcon;
                }
                else // �ν��� ���� �ӽ��� ���ٸ�
                {
                    _speechBalloon.SetActive(true);
                    _orderMenuSpriteRenderer.sprite = _angrySprite;

                    LeaveStore();
                }
            }
        }

        // �մ��� ��ġ�ϸ� �ֹ��� �Ϸ�ǰ� �̵��� �簳��
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
                    _orderedMachine.SellCoffee(); // Ŀ�Ǹӽ��� �� �� ����
                    PlayCoinEffect();
                    _speechBalloon.SetActive(false);
                    LeaveStore();

                }
            }


        }

        void LeaveStore()
        {
            _agent.isStopped = false; // �̵� �簳
            _isOrdering = false;
            //_speechBalloon.SetActive(false);
            MoveTo(_outside);

        }

        public void PlayCoinEffect()
        {
            ParticleSystem ps = Instantiate(coinParticlePrefab, transform.position, Quaternion.identity);
            ps.Play();
            Destroy(ps.gameObject, 2f); // ��ƼŬ �� ������ ����
        }
    }
}
