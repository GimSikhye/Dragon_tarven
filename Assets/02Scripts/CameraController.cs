using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float dragSpeed = 0.1f; // 드래그 속도 조절
    public Vector2 minBounds; // 최소 좌표(왼쪽 아래)
    public Vector2 maxBounds; // 최대 좌표(오른쪽 위)

    private Vector3 lastTouchPosition;

    void Update()
    {
        HandleCameraDrag();
    }

    private void HandleCameraDrag()
    {
        if(Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);

            if(touch.phase == TouchPhase.Began)
            {
                lastTouchPosition = Camera.main.ScreenToWorldPoint(touch.position);
            }
            else if(touch.phase == TouchPhase.Moved)
            {
                Vector3 currentPosition = Camera.main.ScreenToWorldPoint(touch.position);
                Vector3 direction = currentPosition - lastTouchPosition; // 이동방향 계산
                MoveCamera(-direction);
                lastTouchPosition = currentPosition;
            }
        }
    }

    private void MoveCamera(Vector3 moveDirection)
    {
        Vector3 newPosition = transform.position + moveDirection * dragSpeed; // 새로운 위치 계산

        // 카메라 위치를 제한된 영역 안으로 조정
        newPosition.x = Mathf.Clamp(newPosition.x, minBounds.x, maxBounds.x);
        newPosition.y = Mathf.Clamp(newPosition.y, minBounds.y, maxBounds.y);

        transform.position = newPosition;
    }
}
