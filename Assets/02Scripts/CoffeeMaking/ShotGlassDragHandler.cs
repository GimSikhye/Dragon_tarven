using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class ShotGlassDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private int shotGlassNumber;
    [SerializeField] private Animator shotGlassAnimator; // ���ܿ� �ִϸ�����
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
        coffeeManager.OnShotGlassDragStart(); // ���Ⱑ ����ȵǴµ�?
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
        // Mug���ǰŸ� üũ
        if (coffeeManager.IsNearMug(rectTransform.position))
        {
            Debug.Log("�ӱ��Ű� ���� �����-���� ������");
            // Mug �� ���ʿ� ��ġ��Ű�� ����� �ڼ��� ����
            coffeeManager.PourShotToMug(shotGlassNumber, this);
        }
        else
        {
            // ���� ��ġ�� ����
            rectTransform.anchoredPosition = originalPosition;
        }
    }

    public void MoveToPourPosition(Vector2 pourPosition)
    {
        Debug.Log("MoveToPourPosition ����");

        rectTransform.anchoredPosition = pourPosition;

        // ������ �ڼ��� ����
        rectTransform.rotation = Quaternion.Euler(0, 0, -45f);

        // �� �ױ� �ִϸ��̼� ����
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
        // �� �ױ� �ִϸ��̼� �Ϸ� ���
        yield return new WaitForSeconds(1.5f);

        // ���� ��ġ�� ȸ������ ����
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
