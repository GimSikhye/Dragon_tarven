using JetBrains.Annotations;
using UnityEngine;

public class PointerController : MonoBehaviour
{
    // coordinate
    [SerializeField] RectTransform areaRect;
    [SerializeField] RectTransform startPointRect;
    [SerializeField] RectTransform endPointRect;

    private RectTransform rectTransform;

    public float baseSpeed = 150f;
    public float moveSpeed;
    private bool isMoving = true;
    private Vector2 target;

    public int currentStage = 1;
    public int maxStage = 5;


    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        rectTransform.anchoredPosition = startPointRect.anchoredPosition;
        target = endPointRect.anchoredPosition;
    }

    void Update()
    {
        Moving();
       
    }

    private void Moving()
    {
        if (!isMoving) return; 

        moveSpeed = baseSpeed + (currentStage - 1) * 100f; // faster 
        rectTransform.anchoredPosition = Vector2.MoveTowards(rectTransform.anchoredPosition, target, moveSpeed * Time.deltaTime);

        if (Vector2.Distance(rectTransform.anchoredPosition, target) < 0.01f)
        {
            target = (target == startPointRect.anchoredPosition) ? endPointRect.anchoredPosition : startPointRect.anchoredPosition;
        }
 

    }


    public void StopMovement() => isMoving = false;
    public void ResumeMovement() => isMoving = true;



}
