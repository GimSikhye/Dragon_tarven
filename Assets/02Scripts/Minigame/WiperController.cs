using JetBrains.Annotations;
using UnityEngine;

public class WiperController : MonoBehaviour
{
    // coordinate
    [SerializeField] RectTransform safeZone;
    [SerializeField] Transform startPoint;
    [SerializeField] Transform endPoint;

    private bool isMoving = true;
    private Vector3 target;
    public float baseSpeed = 5f;

    public int currentStage = 1;
    public int maxStage = 5;




    void Start()
    {
        transform.position = startPoint.position;
        target = endPoint.position;
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
        transform.position = Vector2.MoveTowards(transform.position, target, speed * Time.deltaTime);

        if (Vector2.Distance(transform.position, target) < 0.01f)
        {
            target = (target == startPoint.position) ? endPoint.position : startPoint.position;
        }
 

    }


    public void StopMovement() => isMoving = false;
    public void ResumeMovement() => isMoving = true;



}
