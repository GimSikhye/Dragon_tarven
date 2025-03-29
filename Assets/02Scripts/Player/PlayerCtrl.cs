using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using DalbitCafe.UI;
using DalbitCafe.Operations;
namespace DablitCafe.Player
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
        private bool startedOverUI = false; // 터치 시작 시 UI 위 여부 기록
        private Vector3 targetPosition;
        private bool isMoving = false;

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
            OnTouch();
        }

        private void OnTouch()
        {
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);

                // 터치 시작 시 UI 위인지 체크하고 기록
                if (touch.phase == TouchPhase.Began)
                {
                    startedOverUI = UIManager.Instance.IsTouchOverUI(touch);
                }
                if (touch.phase == TouchPhase.Began || touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
                {
                    ShowTouchFeedback(touch.position);
                }
                else if (touch.phase == TouchPhase.Ended)
                {
                    CoffeMachine(touch);

                    // 터치 종료 시, UI에서 시작하지 않은 경우에만 이동 처리
                    if (startedOverUI)
                    {
                        startedOverUI = false; // 초기화
                        return;
                    }

                    {
                        // 터치 포지션이 위치한 곳의 layer가 floor가 아니라면 return
                        Vector2 touchPosition = Camera.main.ScreenToWorldPoint(touch.position);
                        Collider2D hitCollider = Physics2D.OverlapPoint(touchPosition);

                        if (hitCollider == null || hitCollider.gameObject.layer != LayerMask.NameToLayer("Floor")) return;
                    }
                    // 이동 처리
                    OnMove(touch);
                }
            }
        }

        private void CoffeMachine(Touch touch)
        {
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, Camera.main.nearClipPlane)); //터치한 지점
            Collider2D hitCollider = Physics2D.OverlapPoint(worldPosition);

            if (hitCollider != null && hitCollider.transform.CompareTag("Coffee Machine"))
            {
                // 거리도 확인해서 가까울 경우만 팝업 표시
                if (Vector3.Distance(transform.position, hitCollider.transform.position) < interactionRange)
                {
                    CoffeeMachine.SetLastTouchedMachine(hitCollider.GetComponent<CoffeeMachine>());
                    if (hitCollider.gameObject.GetComponent<CoffeeMachine>().IsRoasting == true)
                    {
                        UIManager.Instance.ShowCurrentMenuPopUp();
                        GameObject currentMenuWindow = GameObject.Find("currentMenu Window");
                        currentMenuWindow.GetComponent<CurrentMenuWindow>().UpdateMenuPanel(hitCollider.gameObject.GetComponent<CoffeeMachine>());

                    }
                    else
                    {
                        UIManager.Instance.ShowMakeCoffeePopUp(); // UIManager의 팝업 표시 함수 호출
                                                                  // currentMenuWindow.UpdateMenuPanel(); //커피데이터 넣기
                    }
                }
                else
                {
                    Debug.Log("거리가 너무 멀어요!!");
                    UIManager.Instance.ShowCapitonText();
                }
            }
        }

        private void OnMove(Touch touch)
        {

            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, Camera.main.nearClipPlane));
            worldPosition.z = 0;
            targetPosition = worldPosition; // 이동 목표 위치 저장

            if (!isMoving) // 이동 중이 아닐 때만 이동 시작
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
                SetAnimation(direction); // 이동 방향 애니메이션 설정

                transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
                yield return null;
            }

            transform.position = targetPosition; // 정확한 위치 보정
            isMoving = false;
            animator.SetBool("isMoving", false);

            touch_feedback.enabled = false;
        }

        private void SetAnimation(Vector3 direction)
        {

            // 이동 방향을 Normalize하여 MoveX, MoveY 값 설정
            Vector3 normalizedDirection = direction.normalized;
            animator.SetFloat("MoveX", normalizedDirection.x);
            animator.SetFloat("MoveY", normalizedDirection.y);

        }


        private void ShowTouchFeedback(Vector2 screenPosition)
        {
            // UI 요소이므로 localPosition을 사용하여 캔버스 내에서 좌표 설정
            touch_feedback.rectTransform.position = screenPosition;
            touch_feedback.enabled = true;
        }




    }


}

