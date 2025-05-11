using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DalbitCafe.UI;
using DalbitCafe.Operations;
using DalbitCafe.Inputs;
using DalbitCafe.Map;
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

        // �̵� ���� ������
        private bool _touchOnUI = false; // ��ġ�� UI������ �����ߴ���
        private Vector3 _targetPosition; // �̵��� ��ġ
        private bool _isMoving = false;
        private bool _canMoveControl = true;

        public Vector3 _savedPosition;

        public void SavePosition()
        {
            _savedPosition = transform.position; // ��ġ ����
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

            if (TouchInputManager.Instance != null)
            {
                TouchInputManager.Instance.OnTouchBegan += HandleTouchBegan; // ��ġ �Ŵ����� ����?
                TouchInputManager.Instance.OnTouchMoved += HandleTouchMoved;
                TouchInputManager.Instance.OnTouchEnded += HandleTouchEnded;
            }
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;

            if (TouchInputManager.Instance != null)
            {
                TouchInputManager.Instance.OnTouchBegan -= HandleTouchBegan;
                TouchInputManager.Instance.OnTouchMoved -= HandleTouchMoved;
                TouchInputManager.Instance.OnTouchEnded -= HandleTouchEnded;
            }
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode) // ���� �ٲ� �� DialgoueScene�� �ƴϸ� ������ �� ����.
        {
            _canMoveControl = scene.name != "DialogueScene";
        }

        public void HandleTouchBegan(Vector2 screenPos)
        {
            if (!_canMoveControl) return;
        }

        public void HandleTouchMoved(Vector2 screenPos)
        {
            if (!_canMoveControl) return;
        }

        public void HandleTouchEnded(Vector2 screenPos) // ��ġ�� �I�� �� ������
        {
             _touchOnUI = UIManager.Instance.IsTouchOverUIPosition(screenPos); // ��ġ UI ǥ��?

            if (!_canMoveControl || _touchOnUI) 
            {
                _touchOnUI = false;  // ����
                return;
            }

            Vector3 worldPos = Camera.main.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, Camera.main.nearClipPlane)); // camera.main.nearclipplane
            worldPos.z = 0;

            // Ŀ�Ǹӽ� ��ǥ ������� ã��
            var machine = CoffeeMachineManager.Instance.GetMachineAtPosition(worldPos); // GetMachingAtPosition ��ġ�� ���� �ӽ��� ������ �ӽ��� ������
            if (machine != null) // ��ġ�� ���� �ӽ��̶��
            {
                TouchCoffeeMachine(machine);
            }
            else if (FloorManager.Instance.IsFloor(worldPos))// ��ġ�� ���� �ٴ��̶��///////////////
            {
                OnMove(worldPos);
            }
        }

        private void TouchCoffeeMachine(CoffeeMachine machine) // ���� �˻�
        {
            if (Vector3.Distance(transform.position, machine.transform.position) <= _interactionRange)
            {
                CoffeeMachine.SetLastTouchedMachine(machine); // �ش� �ӽ��� ���������� ��ġ�� �ӽ����� ����

                if (machine.IsRoasting)
                {
                    UIManager.Instance.ShowCurrentMenuPopUp(); // ���� ����� �ִ� �޴� �˾�
                    GameObject currentMenuWindow = GameObject.Find("Panel_CurrentMenu");
                    currentMenuWindow.GetComponent<CurrentMenuWindow>().UpdateMenuPanel(machine);
                }
                else
                {
                    UIManager.Instance.ShowMakeCoffeePopUp();
                }
            }
            else
            {
                UIManager.Instance.ShowCapitonText(); // �ʹ� �־��
            }
        }
        private void OnMove(Vector3 targetPos)
        {
            _targetPosition = targetPos;

            if (!_isMoving)
            {
                StartCoroutine(MoveToTarget()); /// Animator
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

            if(_touchFeedback!=null)
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
