using UnityEngine;


[RequireComponent(typeof(CharacterAnimatorController))]
// 터치 입력 감지 & 이동 처리
public class TouchMover : MonoBehaviour
{
    private Vector3 targetPosition;
    private float moveSpeed = 3f;

    private CharacterAnimatorController animatorController;

    private void Start()
    {
        animatorController = GetComponent<CharacterAnimatorController>();
    }

    private void Update()
    {
        HandleTouch();

        // 현재 위치 -> 목표 위치로 이동
        if(targetPosition != Vector3.zero)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

            if(Vector3.Distance(transform.position, targetPosition) < 0.05f)
            {
                targetPosition = Vector3.zero; // 이동 종료
                animatorController.PlayIdle();
            }
        }

        


    }

    private void HandleTouch()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Ended)
            {
                Vector3 worldPos = Camera.main.ScreenToWorldPoint(touch.position);
                worldPos.z = 0f;

                // 목표 갱신 및 방향 재계산
                targetPosition = worldPos;
                animatorController.UpdateDirectionAndPlay(transform.position, targetPosition);


            }

        }
    }
}
