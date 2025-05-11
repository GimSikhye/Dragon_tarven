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

        [Header("터치 UI")]
        [SerializeField] private Image _touchFeedback; // 터치 피드백

        [Header("커피머신 로직")]
        [SerializeField] private float _interactionRange; // 상호작용

        [Header("플레이어 이동")]
        [SerializeField] private float _moveSpeed = 3f;

        private SpriteRenderer _spriteRenderer;
        public SpriteRenderer SpriteRender => _spriteRenderer;
        private Animator _animator;

        // 이동 관련 변수들
        private bool _touchOnUI = false; // 터치를 UI위에서 시작했는지
        private Vector3 _targetPosition; // 이동할 위치
        private bool _isMoving = false;
        private bool _canMoveControl = true;

        public Vector3 _savedPosition;

        public void SavePosition()
        {
            _savedPosition = transform.position; // 위치 저장
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

            if (TouchInputManager.Instance != null)
            {
                TouchInputManager.Instance.OnTouchBegan += HandleTouchBegan; // 터치 매니저의 역할?
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

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode) // 씬이 바뀔 때 DialgoueScene이 아니면 움직일 수 있음.
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

        public void HandleTouchEnded(Vector2 screenPos) // 터치를 똈을 때 움직임
        {
             _touchOnUI = UIManager.Instance.IsTouchOverUIPosition(screenPos); // 터치 UI 표시?

            if (!_canMoveControl || _touchOnUI) 
            {
                _touchOnUI = false;  // 리셋
                return;
            }

            Vector3 worldPos = Camera.main.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, Camera.main.nearClipPlane)); // camera.main.nearclipplane
            worldPos.z = 0;

            // 커피머신 좌표 기반으로 찾기
            var machine = CoffeeMachineManager.Instance.GetMachineAtPosition(worldPos); // GetMachingAtPosition 터치한 곳에 머신이 있으면 머신을 가져옴
            if (machine != null) // 터치한 곳이 머신이라면
            {
                TouchCoffeeMachine(machine);
            }
            else if (FloorManager.Instance.IsFloor(worldPos))// 터치한 곳이 바닥이라면///////////////
            {
                OnMove(worldPos);
            }
        }

        private void TouchCoffeeMachine(CoffeeMachine machine) // 여기 검사
        {
            if (Vector3.Distance(transform.position, machine.transform.position) <= _interactionRange)
            {
                CoffeeMachine.SetLastTouchedMachine(machine); // 해당 머신을 마지막으로 터치한 머신으로 설정

                if (machine.IsRoasting)
                {
                    UIManager.Instance.ShowCurrentMenuPopUp(); // 현재 만들고 있는 메뉴 팝업
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
                UIManager.Instance.ShowCapitonText(); // 너무 멀어요
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

            if(_touchFeedback!=null)
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
