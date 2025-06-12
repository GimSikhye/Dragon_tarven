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
    private bool isPouring = false; // �״� ������ Ȯ���ϴ� �÷���

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        originalPosition = rectTransform.anchoredPosition;
        parentCanvas = GetComponentInParent<Canvas>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!coffeeManager.CanDragShotGlass(shotGlassNumber)) return;
        if (isPouring) return; // �״� ���̸� �巡�� �Ұ�

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

        // Mug���ǰŸ� üũ
        if (coffeeManager.IsNearMug(rectTransform.position))
        {
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
        isPouring = true; // �״� ���·� ����

        rectTransform.anchoredPosition = pourPosition;

        // ������ �ڼ��� ����
        rectTransform.rotation = Quaternion.Euler(0, 0, -45f);

        // �� �ױ� �ִϸ��̼� ����
        if (shotGlassAnimator != null)
        {
            shotGlassAnimator.SetTrigger("Pour");
            // �ִϸ��̼� �Ϸ� �� ����
            StartCoroutine(WaitForAnimationAndDestroy());
        }
    
    }

    private IEnumerator WaitForAnimationAndDestroy()
    {
        // �ִϸ��̼��� ���۵� ������ ��� ���
        yield return new WaitForEndOfFrame();

        // "Pour" �ִϸ��̼��� �Ϸ�� ������ ���
        while(shotGlassAnimator.GetCurrentAnimatorStateInfo(0).IsName("Pour") &&
            shotGlassAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
        {
            yield return null;
        }

        // �ִϸ��̼� �Ϸ� �� CoffeeMakingManager�� �˸�
        Debug.Log($"Shot Glass {shotGlassNumber} �ִϸ��̼� �Ϸ�");
        coffeeManager.OnShotGlassAnimationCompleted(shotGlassNumber);

        // ������Ʈ ����
        Debug.Log($"Shot Glass {shotGlassNumber} ������Ʈ ����");
        Destroy(gameObject);

    }
}
