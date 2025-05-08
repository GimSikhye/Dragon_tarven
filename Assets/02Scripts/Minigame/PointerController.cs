using JetBrains.Annotations;
using UnityEngine;

public class PointerController : MonoBehaviour
{
    // coordinate
    [SerializeField] RectTransform areaRect;
    [SerializeField] RectTransform startPointRect;
    [SerializeField] RectTransform endPointRect;

    private RectTransform pointerRect;

    public float baseSpeed = 150f;
    public float moveSpeed;
    private bool isMoving = true;
    private Vector2 target;

    public int currentStage = 1;
    public int maxStage = 5;


    void Start()
    {
        pointerRect = GetComponent<RectTransform>();
        pointerRect.anchoredPosition = startPointRect.anchoredPosition;
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
        pointerRect.anchoredPosition = Vector2.MoveTowards(pointerRect.anchoredPosition, target, moveSpeed * Time.deltaTime);

        if (Vector2.Distance(pointerRect.anchoredPosition, target) < 0.01f) // shuttle
        {
            target = (target == startPointRect.anchoredPosition) ? endPointRect.anchoredPosition : startPointRect.anchoredPosition;
        }
 

    }


    public void StopMovement() => isMoving = false;
    public void ResumeMovement() => isMoving = true;



}
