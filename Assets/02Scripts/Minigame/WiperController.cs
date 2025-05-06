using JetBrains.Annotations;
using UnityEngine;

public class WiperController : MonoBehaviour
{
    // coordinate
    [SerializeField] RectTransform safeZone;
    [SerializeField] RectTransform startPoint;
    [SerializeField] RectTransform endPoint;
    RectTransform rectTransform;

    private bool isMoving = true;
    private Vector2 target;
    public float baseSpeed = 5f;

    public int currentStage = 1;
    public int maxStage = 5;




    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        rectTransform.anchoredPosition = startPoint.anchoredPosition;
        target = endPoint.anchoredPosition;
    }

    void Update()
    {
        Moving();
        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    // 스페이스 키를 누르면, 이동 멈춤.
        //    isMoving = false;
        //    if (RectTransformUtility.RectangleContainsScreenPoint(safeZone, transform.position))
        //    {
        //        Debug.Log("점수 영역입니다");
        //    }


        //}
    }

    private void Moving()
    {
        if (!isMoving) return;

        float speed = baseSpeed + (currentStage - 1) * 1.5f;
        rectTransform.anchoredPosition = Vector2.MoveTowards(rectTransform.anchoredPosition, target, speed * Time.deltaTime);

        if (Vector2.Distance(rectTransform.anchoredPosition, target) < 0.01f)
        {
            target = (target == startPoint.anchoredPosition) ? endPoint.anchoredPosition : startPoint.anchoredPosition;
        }
 

    }


    public void StopMovement() => isMoving = false;
    public void ResumeMovement() => isMoving = true;



}
