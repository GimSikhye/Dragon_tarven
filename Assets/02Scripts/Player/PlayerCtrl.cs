using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DalbitCafe.UI;
using DalbitCafe.Operations;
using DalbitCafe.Inputs;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.EnhancedTouch;
//�ڵ��б�
namespace DalbitCafe.Player
{
    public class PlayerCtrl : MonoSingleton<PlayerCtrl>
    {

        [Header("��ġ UI")]
        [SerializeField] private Image _touchFeedback; // ��ġ �ǵ��

        [Header("Ŀ�Ǹӽ� ����")]
        [SerializeField] private float _interactionRange; // ��ȣ�ۿ�

        [Header("�÷��̾� �̵�")]
        [SerializeField] private float _moveSpeed = 3f;

        private SpriteRenderer _spriteRenderer;
        public SpriteRenderer SpriteRender => _spriteRenderer;
        private Animator _animator;

        private bool _startedOverUI = false; // ��ġ�� UI������ �����ߴ���
        private Vector3 _targetPosition; // �̵��� ��ġ
        private bool _isMoving = false;
        private bool _canMoveControl = true;

        public Vector3 _savedPosition;

        public void SavePosition()
        {
            _savedPosition = transform.position;
        }

        public void RestorePosition() // ��ġ ����
        {
            transform.position = _savedPosition;
        }

        private void Start()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _animator = GetComponent<Animator>();
        }

        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded; // �� �ٲ� ��

            if (GameManager.Instance.TouchInputManager != null)
            {
                GameManager.Instance.TouchInputManager.OnTouchBegan += HandleTouchBegan;
                GameManager.Instance.TouchInputManager.OnTouchMoved += HandleTouchMoved;
                GameManager.Instance.TouchInputManager.OnTouchEnded += HandleTouchEnded;
            }
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;

            if (GameManager.Instance.TouchInputManager != null)
            {
                GameManager.Instance.TouchInputManager.OnTouchBegan -= HandleTouchBegan;
                GameManager.Instance.TouchInputManager.OnTouchMoved -= HandleTouchMoved;
                GameManager.Instance.TouchInputManager.OnTouchEnded -= HandleTouchEnded;
            }
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            _canMoveControl = scene.name != "DialogueScene";
        }

        public void HandleTouchBegan(Vector2 screenPos)
        {
            if (!_canMoveControl) return;
            _startedOverUI = GameManager.Instance.UIManager.IsTouchOverUIPosition(screenPos);
        }

        public void HandleTouchMoved(Vector2 screenPos)
        {
            if (!_canMoveControl) return;
        }

        public void HandleTouchEnded(Vector2 screenPos) // ��ġ�� �I�� �� ������
        {
            if (!_canMoveControl || _startedOverUI) 
            {
                _startedOverUI = false;  // ����
                return;
            }


            Vector3 worldPos = Camera.main.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, Camera.main.nearClipPlane));
            worldPos.z = 0;

            // Ŀ�Ǹӽ� ��ǥ ������� ã��
            var machine = GameManager.Instance.CoffeeMachineManager.GetMachineAtPosition(worldPos);
            if (machine != null)
            {
                TouchCoffeeMachine(machine);
            }
            else if (GameManager.Instance.FloorManager.IsFloor(worldPos)) 
            {
                OnMove(worldPos);
            }
        }

        private void TouchCoffeeMachine(CoffeeMachine machine)
        {
            if (Vector3.Distance(transform.position, machine.transform.position) < _interactionRange)
            {
                CoffeeMachine.SetLastTouchedMachine(machine);

                if (machine.IsRoasting)
                {
                    GameManager.Instance.UIManager.ShowCurrentMenuPopUp();
                    GameObject currentMenuWindow = GameObject.Find("Panel_CurrentMenu");
                    currentMenuWindow.GetComponent<CurrentMenuWindow>().UpdateMenuPanel(machine);
                }
                else
                {
                    GameManager.Instance.UIManager.ShowMakeCoffeePopUp();
                }
            }
            else
            {
                GameManager.Instance.UIManager.ShowCapitonText();
            }
        }
        private void OnMove(Vector3 targetPos)
        {
            _targetPosition = targetPos;

            if (!_isMoving)
            {
                StartCoroutine(MoveToTarget());
            }
        }

        private IEnumerator MoveToTarget() // �� �������� �̵�
        {
            _isMoving = true;
            _animator.SetBool("isMoving", true);

            while (Vector3.Distance(transform.position, _targetPosition) > 0.1f)
            {
                Vector3 direction = (_targetPosition - transform.position).normalized; // �ִϸ��̼� �뵵
                SetAnimation(direction);

                transform.position = Vector3.MoveTowards(transform.position, _targetPosition, _moveSpeed * Time.deltaTime);
                yield return null;
            }

            // �� �̵��ߴٸ� 
            transform.position = _targetPosition;
            _isMoving = false;
            _animator.SetBool("isMoving", false);

            _touchFeedback.enabled = false; // �̰͵� UIManager�� �Űܾ���
        }

        private void SetAnimation(Vector3 direction)
        {
            Vector3 normalizedDirection = direction.normalized;
            _animator.SetFloat("MoveX", normalizedDirection.x);
            _animator.SetFloat("MoveY", normalizedDirection.y);
        }


    }
}
