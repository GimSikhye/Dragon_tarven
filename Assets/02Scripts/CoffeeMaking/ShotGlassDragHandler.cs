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

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        originalPosition = rectTransform.anchoredPosition;
        parentCanvas = GetComponentInParent<Canvas>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!coffeeManager.CanDragShotGlass(shotGlassNumber)) return;
        isDragging = true;
        coffeeManager.OnShotGlassDragStart(); // 여기가 실행안되는듯?
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
            Debug.Log("머그컵과 샷잔 가까움-샷잔 기울어짐");
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

        rectTransform.anchoredPosition = pourPosition;

        // 기울어진 자세로 변경
        rectTransform.rotation = Quaternion.Euler(0, 0, -45f);

        // 샷 붓기 애니메이션 실행
        if (shotGlassAnimator != null)
        {
            shotGlassAnimator.SetTrigger("Pour");
        }
    }
    public void ReturnToOriginalPosition()
    {
        StartCoroutine(ReturnCoroutine());
    }

    private IEnumerator ReturnCoroutine()
    {
        // 샷 붓기 애니메이션 완료 대기
        yield return new WaitForSeconds(1.5f);

        // 원래 위치와 회전으로 복귀
        float duration = 0.5f;
        float elapsed = 0f;
        Vector2 startPos = rectTransform.anchoredPosition;
        Quaternion startRot = rectTransform.rotation;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / duration);

            rectTransform.anchoredPosition = Vector2.Lerp(startPos, originalPosition, t);
            rectTransform.rotation = Quaternion.Lerp(startRot, Quaternion.identity, t);

            yield return null;
        }

        rectTransform.anchoredPosition = originalPosition;
        rectTransform.rotation = Quaternion.identity;
    }

}
