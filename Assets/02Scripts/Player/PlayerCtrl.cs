using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerCtrl : MonoBehaviour
{
    [SerializeField] private Image touch_feedback;
    [SerializeField] private Transform coffeeMachine;

    [SerializeField] private float interactionRange;
    private SpriteRenderer spriteRenderer;

    public static PlayerCtrl Instance;

    public SpriteRenderer SpriteRender { get { return spriteRenderer; } }

    private bool startedOverUI = false; // 터치 시작 시 UI 위 여부 기록

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            spriteRenderer = GetComponent<SpriteRenderer>();
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
            if(touch.phase == TouchPhase.Began)
            {
                startedOverUI = IsTouchOverUI(touch);  
            }
            if (touch.phase == TouchPhase.Began || touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
            {
                ShowTouchFeedback(touch.position);
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                // 터치 종료 시, UI에서 시작하지 않은 경우에만 이동 처리
                if(startedOverUI)
                {
                    Debug.Log("터치 시작이 UI 위에서 이루어졌음");
                    startedOverUI = false; // 초기화
                    return;
                }
                // 이동 처리
                OnMove(touch);


                Vector3 worldPosition = Camera.main.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, Camera.main.nearClipPlane)); //터치한 지점
                Collider2D hitCollider = Physics2D.OverlapPoint(worldPosition);

                if (hitCollider != null && hitCollider.transform == coffeeMachine)
                {
                    // 거리도 확인해서 가까울 경우만 팝업 표시
                    if (Vector3.Distance(transform.position, coffeeMachine.position) < interactionRange)
                    {
                        UIManager.Instance.ShowPopup(); // UIManager의 팝업 표시 함수 호출
                    }
                    else
                    {
                        Debug.Log("거리가 너무 멀어요!!");
                        UIManager.Instance.ShowCapitonText();
                    }
                }
            }
        }
    }

    private void OnMove(Touch touch)
    {
        if (EventSystem.current.IsPointerOverGameObject(touch.fingerId))
        {
            Debug.Log("UI 터치");
            return;
        }

        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, Camera.main.nearClipPlane));
        worldPosition.z = 0;
        transform.position = worldPosition;

        touch_feedback.enabled = false;
    }

    // 터치 위치가 UI 위인지 판단하는 커스텀 함수
    private bool IsTouchOverUI(Touch touch)
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = touch.position;
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }

    private void ShowTouchFeedback(Vector2 screenPosition)
    {
        // UI 요소이므로 localPosition을 사용하여 캔버스 내에서 좌표 설정
        touch_feedback.rectTransform.position = screenPosition;
        touch_feedback.enabled = true;
    }



    
}
