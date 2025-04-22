using UnityEngine;
using UnityEngine.AI;
using DalbitCafe.Operations;
using System.Collections.Generic;

namespace DalbitCafe.Customer
{
    public class Customer : MonoBehaviour
    {
        [Header("��ƼŬ ����")]
        [SerializeField] private ParticleSystem coinParticlePrefab;
        [SerializeField] private int particlePoolSize = 5;
        private Queue<ParticleSystem> coinParticlePool = new Queue<ParticleSystem>(); // ���⼭ queue�� ���̴� ����

        [Header("�� ã��")]
        private NavMeshAgent _agent;
        private Vector3 _targetDestination;
        private Transform _cashDesk;
        private Transform _outside; // �ٱ�
        private CustomerPool _customerPool;

        [Header("�ֹ� ����")]
        [SerializeField] private GameObject _speechBalloon;
        [SerializeField] private SpriteRenderer _orderMenuSpriteRenderer;
        [SerializeField] private Sprite _angrySprite;

        private CoffeeData _randomCoffee;
        private CoffeeMachine _orderedMachine; // �ֹ��� �ӽ�
        [SerializeField] private bool _isOrdering = false; // �ֹ� ������
        private Animator _animator;

        // Animator �Ķ���� �ؽ�
        private static readonly int MoveXHash = Animator.StringToHash("MoveX");
        private static readonly int MoveYHash = Animator.StringToHash("MoveY");
        private static readonly int IsMovingHash = Animator.StringToHash("isMoving");

        private void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
            _customerPool = FindAnyObjectByType<CustomerPool>();

            _agent.updateRotation = false;
            _agent.updateUpAxis = false;

            _cashDesk = GameManager.Instance.CashDesk;
            _outside = GameManager.Instance.OutSide;

            _animator = GetComponent<Animator>();

            InitializeParticlePool();
        }

        private void InitializeParticlePool()
        {
            for (int i = 0; i < particlePoolSize; i++)
            {
                ParticleSystem ps = Instantiate(coinParticlePrefab);
                ps.gameObject.SetActive(false);
                coinParticlePool.Enqueue(ps);
            }
        }

        private void OnEnable()
        {
            _speechBalloon.SetActive(false);
            Initialize();
        }

        public void MoveTo(Transform destination)
        {
            _targetDestination = destination.position;
            _agent.SetDestination(_targetDestination);
        }

        private void Initialize()
        {
            MoveTo(_cashDesk);
        }

        private void Update()
        {
            float remaining = _agent.remainingDistance;

            bool isMoving = !_agent.pathPending && remaining > _agent.stoppingDistance;

            _animator.SetBool(IsMovingHash, isMoving);

            if (isMoving)
            {
                Vector3 direction = _agent.desiredVelocity.normalized;
                _animator.SetFloat(MoveXHash, direction.x);
                _animator.SetFloat(MoveYHash, direction.y);
            }

            if (!_agent.pathPending && remaining <= _agent.stoppingDistance)
            {
                if (_targetDestination == _cashDesk.position)
                    StartOrdering();
                else if (_targetDestination == _outside.position)
                    ReturnToPool();
            }
        }

        private void StartOrdering()
        {
            _isOrdering = true;
            _agent.isStopped = true;

            CoffeeMachine[] machines = FindObjectsOfType<CoffeeMachine>();
            List<CoffeeMachine> roastingMachines = new List<CoffeeMachine>();

            foreach (var machine in machines)
            {
                if (machine.CurrentCoffee != null && machine.RemainingMugs > 0)
                    roastingMachines.Add(machine);
            }

            _speechBalloon.SetActive(true);

            if (roastingMachines.Count > 0)
            {
                _orderedMachine = roastingMachines[Random.Range(0, roastingMachines.Count)];
                _randomCoffee = _orderedMachine.CurrentCoffee;
                _orderMenuSpriteRenderer.sprite = _randomCoffee.MenuIcon;
            }
            else
            {
                _orderMenuSpriteRenderer.sprite = _angrySprite;
                LeaveStore();
            }
        }

        private void OnMouseDown()
        {
            if (_isOrdering)
                FinishOrder();
        }

        private void FinishOrder()
        {
            if (_randomCoffee != null && _orderedMachine != null)
            {
                _orderedMachine.SellCoffee();
                PlayCoinEffect();
                _speechBalloon.SetActive(false);
                LeaveStore();
            }
        }

        private void LeaveStore()
        {
            _agent.isStopped = false;
            _isOrdering = false;
            MoveTo(_outside);
        }

        private void ReturnToPool()
        {
            _speechBalloon.SetActive(false);
            _customerPool.ReturnCustomer(this.gameObject);
        }

        public void PlayCoinEffect()
        {
            if (coinParticlePool.Count > 0)
            {
                ParticleSystem ps = coinParticlePool.Dequeue();
                ps.transform.position = transform.position;
                ps.gameObject.SetActive(true);
                ps.Play();
                StartCoroutine(ReturnParticleToPool(ps, 2f));
            }
        }

        private System.Collections.IEnumerator ReturnParticleToPool(ParticleSystem ps, float delay)
        {
            yield return new WaitForSeconds(delay);
            ps.Stop();
            ps.gameObject.SetActive(false);
            coinParticlePool.Enqueue(ps);
        }
    }
}
