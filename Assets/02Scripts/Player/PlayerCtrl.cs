using UnityEngine;
using UnityEngine.UI;

public class PlayerCtrl : MonoBehaviour
{
    [SerializeField] private Image touch_feedback;
    [SerializeField] private Transform coffeeMachine;

    [SerializeField] private float interactionRange;
    private SpriteRenderer spriteRenderer;

    public static PlayerCtrl Instance;

    public SpriteRenderer SpriteRender
    {
        get { return spriteRenderer; }
    }
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

            if (touch.phase == TouchPhase.Began || touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
            {
                ShowTouchFeedback(touch.position);
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                //Debug.Log("손가락을 뗀 것이 감지됨!!");

                // 이동할 수 있는 곳이면
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
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, Camera.main.nearClipPlane));
        worldPosition.z = 0;
        transform.position = worldPosition;

        touch_feedback.enabled = false;
    }

    private void ShowTouchFeedback(Vector2 screenPosition)
    {
        // UI 요소이므로 localPosition을 사용하여 캔버스 내에서 좌표 설정
        touch_feedback.rectTransform.position = screenPosition;
        touch_feedback.enabled = true;
    }



    
}
