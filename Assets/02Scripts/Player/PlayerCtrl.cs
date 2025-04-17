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

            Collider2D hit = Physics2D.OverlapPoint(worldPos, LayerMask.GetMask("CoffeeMachine")); // 커피 머신 레이어마스크 필터링
            if (hit != null && hit.CompareTag("CoffeeMachine")) // 콜라이더 한 물체의 태그가 커피머신이라면
            {
                TouchCoffeeMachine(hit.gameObject); // 해당 커피머신 넘겨주기
            }
            else // 커피머신이 아니라면
            {
                Collider2D floorCheck = Physics2D.OverlapPoint(worldPos, LayerMask.GetMask("Floor"));
                if (floorCheck != null) //바닥타일맵인지 체크
                {
                    OnMove(worldPos); // 바닥 타일맵이면 이동가능
                }
            }
        }

        private void TouchCoffeeMachine(GameObject machine)
        {
            if (Vector3.Distance(transform.position, machine.transform.position) < interactionRange) // 플레이어 거리와 커피머신의 거리가 상호작용거리 내라면
            {
                CoffeeMachine coffeeMachine = machine.GetComponent<CoffeeMachine>();
                CoffeeMachine.SetLastTouchedMachine(coffeeMachine); // 마지막으로 터치한 커피머신으로 저장

                if (coffeeMachine.IsRoasting) // 해당 커피머신이 로스팅 중이라면
                {
                    GameManager.Instance.UIManager.ShowCurrentMenuPopUp();
                    GameObject currentMenuWindow = GameObject.Find("Panel_CurrentMenu"); // Find
                    currentMenuWindow.GetComponent<CurrentMenuWindow>().UpdateMenuPanel(coffeeMachine);
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
