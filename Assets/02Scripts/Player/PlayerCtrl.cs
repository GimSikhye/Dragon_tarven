using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DalbitCafe.UI;
using DalbitCafe.Operations;
using DalbitCafe.Inputs;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.EnhancedTouch;
//코드읽기
namespace DalbitCafe.Player
{
    public class PlayerCtrl : MonoSingleton<PlayerCtrl>
    {

        [Header("터치 UI")]
        [SerializeField] private Image _touchFeedback; // 터치 피드백

        [Header("커피머신 로직")]
        [SerializeField] private float _interactionRange; // 상호작용

        [Header("플레이어 이동")]
        [SerializeField] private float _moveSpeed = 3f;

        private SpriteRenderer _spriteRenderer;
        public SpriteRenderer SpriteRender => _spriteRenderer;
        private Animator _animator;

        private bool _startedOverUI = false; // 터치를 UI위에서 시작했는지
        private Vector3 _targetPosition; // 이동할 위치
        private bool _isMoving = false;
        private bool _canMoveControl = true;

        public Vector3 _savedPosition;

        public void SavePosition()
        {
            _savedPosition = transform.position;
        }

        public void RestorePosition() // 위치 복원
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
            SceneManager.sceneLoaded += OnSceneLoaded; // 씬 바뀔 때

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

        public void HandleTouchEnded(Vector2 screenPos) // 터치를 똈을 때 움직임
        {
            if (!_canMoveControl || _startedOverUI) 
            {
                _startedOverUI = false;  // 리셋
                return;
            }


            Vector3 worldPos = Camera.main.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, Camera.main.nearClipPlane));
            worldPos.z = 0;

            // 커피머신 좌표 기반으로 찾기
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

        private IEnumerator MoveToTarget() // 그 지점으로 이동
        {
            _isMoving = true;
            _animator.SetBool("isMoving", true);

            while (Vector3.Distance(transform.position, _targetPosition) > 0.1f)
            {
                Vector3 direction = (_targetPosition - transform.position).normalized; // 애니메이션 용도
                SetAnimation(direction);

                transform.position = Vector3.MoveTowards(transform.position, _targetPosition, _moveSpeed * Time.deltaTime);
                yield return null;
            }

            // 다 이동했다면 
            transform.position = _targetPosition;
            _isMoving = false;
            _animator.SetBool("isMoving", false);

            _touchFeedback.enabled = false; // 이것도 UIManager로 옮겨야함
        }

        private void SetAnimation(Vector3 direction)
        {
            Vector3 normalizedDirection = direction.normalized;
            _animator.SetFloat("MoveX", normalizedDirection.x);
            _animator.SetFloat("MoveY", normalizedDirection.y);
        }


    }
}
