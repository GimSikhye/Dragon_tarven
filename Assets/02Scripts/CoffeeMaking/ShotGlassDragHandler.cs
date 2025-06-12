using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class ShotGlassDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private int shotGlassNumber;
    [SerializeField] private Animator shotGlassAnimator; // 샷잔용 애니메이터
    [SerializeField] CoffeeMakingManager coffeeManager;

    private RectTransform rectTransform;
    private Vector2 originalPosition;
    private bool isDragging = false;
    private Canvas parentCanvas;
    private bool isPouring = false; // 붓는 중인지 확인하는 플래그

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        originalPosition = rectTransform.anchoredPosition;
        parentCanvas = GetComponentInParent<Canvas>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!coffeeManager.CanDragShotGlass(shotGlassNumber)) return;
        if (isPouring) return; // 붓는 중이면 드래그 불가

        isDragging = true;
        coffeeManager.OnShotGlassDragStart(); 
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragging) return;

        Vector2 localPoint;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentCanvas.transform as RectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out localPoint))
        {
            rectTransform.anchoredPosition = localPoint;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!isDragging) return;

        isDragging = false;

        // Mug과의거리 체크
        if (coffeeManager.IsNearMug(rectTransform.position))
        {
            // Mug 위 왼쪽에 위치시키고 기울인 자세로 변경
            coffeeManager.PourShotToMug(shotGlassNumber, this);
        }
        else
        {
            // 원래 위치롤 복귀
            rectTransform.anchoredPosition = originalPosition;
        }
    }

    public void MoveToPourPosition(Vector2 pourPosition)
    {
        Debug.Log("MoveToPourPosition 실행");
        isPouring = true; // 붓는 상태로 변경

        rectTransform.anchoredPosition = pourPosition;

        // 기울어진 자세로 변경
        rectTransform.rotation = Quaternion.Euler(0, 0, -45f);

        // 샷 붓기 애니메이션 실행
        if (shotGlassAnimator != null)
        {
            shotGlassAnimator.SetTrigger("Pour");
            // 애니메이션 완료 후 삭제
            StartCoroutine(WaitForAnimationAndDestroy());
        }
    
    }

    private IEnumerator WaitForAnimationAndDestroy()
    {
        // 애니메이션이 시작될 때까지 잠시 대기
        yield return new WaitForEndOfFrame();

        // "Pour" 애니메이션이 완료될 때까지 대기
        while(shotGlassAnimator.GetCurrentAnimatorStateInfo(0).IsName("Pour") &&
            shotGlassAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
        {
            yield return null;
        }

        // 애니메이션 완료 후 CoffeeMakingManager에 알림
        Debug.Log($"Shot Glass {shotGlassNumber} 애니메이션 완료");
        coffeeManager.OnShotGlassAnimationCompleted(shotGlassNumber);

        // 오브젝트 삭제
        Debug.Log($"Shot Glass {shotGlassNumber} 오브젝트 삭제");
        Destroy(gameObject);

    }
}
