using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DalbitCafe.UI;
using DalbitCafe.Operations;
using UnityEngine.SceneManagement;

namespace DalbitCafe.Player
{
    public class PlayerCtrl : MonoBehaviour
    {
        [Header("싱글톤")]
        public static PlayerCtrl Instance;
        [Space(10)]
        [Header("터치 UI")]
        [SerializeField] private Image touch_feedback;
        [Header("커피머신 로직")]
        [SerializeField] private float interactionRange;
        [Header("플레이어 이동")]
        private float moveSpeed = 3f;

        private SpriteRenderer spriteRenderer;
        public SpriteRenderer SpriteRender { get { return spriteRenderer; } }
        private Animator animator;
        private bool startedOverUI = false;
        private Vector3 targetPosition;
        private bool isMoving = false;

        private bool canControl = true; //  조작 가능 여부

        public Vector3 savedPosition;

        public void SavePosition()
        {
            savedPosition = transform.position;
        }

        public void RestorePosition()
        {
            transform.position = savedPosition;
        }

        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name == "DialogueScene")
            {
                canControl = false; //  조작만 막음
            }
            else
            {
                canControl = true;  //  다시 조작 허용
            }
        }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                spriteRenderer = GetComponent<SpriteRenderer>();
                animator = GetComponent<Animator>();
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Update()
        {
            if (!canControl) return; // 조작 제한
            OnTouch();
        }

        private void OnTouch()
        {
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);

                if (touch.phase == TouchPhase.Began)
                {
                    startedOverUI = GameManager.Instance.UIManager.IsTouchOverUI(touch);
                }

                if (touch.phase == TouchPhase.Began || touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
                {
                    ShowTouchFeedback(touch.position);
                }
                else if (touch.phase == TouchPhase.Ended)
                {
                    TouchCoffeeMachine(touch);

                    if (startedOverUI)
                    {
                        startedOverUI = false;
                        return;
                    }

                    Vector2 touchPosition = Camera.main.ScreenToWorldPoint(touch.position);
                    Collider2D hitCollider = Physics2D.OverlapPoint(touchPosition);

                    if (hitCollider == null || hitCollider.gameObject.layer != LayerMask.NameToLayer("Floor")) return;

                    OnMove(touch);
                }
            }
        }

        private void TouchCoffeeMachine(Touch touch)
        {
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, Camera.main.nearClipPlane));
            int coffeeMachineLayer = LayerMask.GetMask("CoffeeMachine");
            Collider2D hitCollider = Physics2D.OverlapPoint(worldPosition, coffeeMachineLayer);

            if (hitCollider != null && hitCollider.transform.CompareTag("CoffeeMachine"))
            {
                if (Vector3.Distance(transform.position, hitCollider.transform.position) < interactionRange)
                {
                    CoffeeMachine.SetLastTouchedMachine(hitCollider.GetComponent<CoffeeMachine>());

                    if (hitCollider.gameObject.GetComponent<CoffeeMachine>().IsRoasting)
                    {
                        GameManager.Instance.UIManager.ShowCurrentMenuPopUp();
                        GameObject currentMenuWindow = GameObject.Find("Panel_CurrentMenu");
                        currentMenuWindow.GetComponent<CurrentMenuWindow>().UpdateMenuPanel(hitCollider.gameObject.GetComponent<CoffeeMachine>());
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
        }

        private void OnMove(Touch touch)
        {
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, Camera.main.nearClipPlane));
            worldPosition.z = 0;
            targetPosition = worldPosition;

            if (!isMoving)
            {
                StartCoroutine(MoveToTarget());
            }
        }

        private IEnumerator MoveToTarget()
        {
            isMoving = true;
            animator.SetBool("isMoving", true);

            while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
            {
                Vector3 direction = (targetPosition - transform.position).normalized;
                SetAnimation(direction);

                transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
                yield return null;
            }

            transform.position = targetPosition;
            isMoving = false;
            animator.SetBool("isMoving", false);

            touch_feedback.enabled = false;
        }

        private void SetAnimation(Vector3 direction)
        {
            Vector3 normalizedDirection = direction.normalized;
            animator.SetFloat("MoveX", normalizedDirection.x);
            animator.SetFloat("MoveY", normalizedDirection.y);
        }

        private void ShowTouchFeedback(Vector2 screenPosition)
        {
            touch_feedback.rectTransform.position = screenPosition;
            touch_feedback.enabled = true;
           
        }
    }
}
