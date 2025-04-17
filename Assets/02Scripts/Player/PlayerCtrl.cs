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
        [SerializeField] private Image touch_feedback; // 터치 피드백

        [Header("커피머신 로직")]
        [SerializeField] private float interactionRange; // 상호작용

        [Header("플레이어 이동")]
        [SerializeField] private float moveSpeed = 3f;

        private SpriteRenderer spriteRenderer;
        public SpriteRenderer SpriteRender => spriteRenderer;
        private Animator animator;

        private bool startedOverUI = false; // 터치를 UI위에서 시작했는지
        private Vector3 targetPosition;
        private bool isMoving = false;
        private bool canMoveControl = true;

        public Vector3 savedPosition;

        public void SavePosition()
        {
            savedPosition = transform.position;
        }

        public void RestorePosition() // 위치 복원
        {
            transform.position = savedPosition;
        }

        private void Start()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            animator = GetComponent<Animator>();
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
            canMoveControl = scene.name != "DialogueScene";
        }

        public void HandleTouchBegan(Vector2 screenPos)
        {
            if (!canMoveControl) return;
            startedOverUI = GameManager.Instance.UIManager.IsTouchOverUIPosition(screenPos);
        }

        public void HandleTouchMoved(Vector2 screenPos)
        {
            if (!canMoveControl) return;
        }

        public void HandleTouchEnded(Vector2 screenPos) // 터치를 똈을 때 움직임
        {
            if (!canMoveControl || startedOverUI) 
            {
                startedOverUI = false;  // 리셋
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
            if (Vector3.Distance(transform.position, machine.transform.position) < interactionRange)
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
            targetPosition = targetPos;

            if (!isMoving)
            {
                StartCoroutine(MoveToTarget());
            }
        }

        private IEnumerator MoveToTarget() // 그 지점으로 이동
        {
            isMoving = true;
            animator.SetBool("isMoving", true);

            while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
            {
                Vector3 direction = (targetPosition - transform.position).normalized; // 애니메이션 용도
                SetAnimation(direction);

                transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
                yield return null;
            }

            // 다 이동했다면 
            transform.position = targetPosition;
            isMoving = false;
            animator.SetBool("isMoving", false);

            touch_feedback.enabled = false; // 이것도 UIManager로 옮겨야함
        }

        private void SetAnimation(Vector3 direction)
        {
            Vector3 normalizedDirection = direction.normalized;
            animator.SetFloat("MoveX", normalizedDirection.x);
            animator.SetFloat("MoveY", normalizedDirection.y);
        }


    }
}
