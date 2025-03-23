using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraController : MonoBehaviour
{
    [Header("화면 이동")]
    public float dragSpeed = 0.1f; // 드래그 속도 조절
    public Vector2 minBounds; // 최소 좌표(왼쪽 아래)
    public Vector2 maxBounds; // 최대 좌표(오른쪽 위)
    [Space(10)]
    [Header("줌 이동")]
    public float zoomSpeed = 0.05f; // 줌 속도 조절
    public float minZoom = 3f; // 최소 줌(최대로 확대)
    public float maxZoom = 10f; // 최대 줌(최대로 축소)


    private Vector3 lastTouchPosition;
    private float lastTouchDistance;
    private bool isTouchingUI = false; // UI 터치 여부 저장

    void Update()
    {
        HandleCameraDrag();
        HandlePinchZoom();
    }

    private void HandleCameraDrag()
    {
        if(Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);

            if(touch.phase == TouchPhase.Began)
            {
                // 터치 시작 시 UI 위에 있는지 확인
                isTouchingUI = EventSystem.current.IsPointerOverGameObject(touch.fingerId);
                lastTouchPosition = Camera.main.ScreenToWorldPoint(touch.position);
            }
            else if(touch.phase == TouchPhase.Moved)
            {
                // 드래그 중에도 UI 위인지 확인하여 UI에서 시작했으면 화면 이동 차단
                if (isTouchingUI) return;

                Vector3 currentPosition = Camera.main.ScreenToWorldPoint(touch.position);
                Vector3 direction = currentPosition - lastTouchPosition; // 이동방향 계산
                MoveCamera(-direction);
                lastTouchPosition = currentPosition;
            }
            else if(touch.phase == TouchPhase.Ended)
            {
                // 터치가 끝나면 UI 터치 여부 초기화
                isTouchingUI = false;
            }
        }
    }

    private void HandlePinchZoom()
    {
        if (Input.touchCount == 2)
        {
            Touch touch1 = Input.GetTouch(0);
            Touch touch2 = Input.GetTouch(1);

            // 두 손가랅 중 하나라도 UI를 터치했으면 줌X
            if (EventSystem.current.IsPointerOverGameObject(touch1.fingerId) || EventSystem.current.IsPointerOverGameObject(touch2.fingerId))
                return;

            // 현재 두 손가락 거리 계산(계속 갱신)
            float currentDistance = Vector2.Distance(touch1.position, touch2.position);

            if (touch1.phase == TouchPhase.Began || touch2.phase == TouchPhase.Began)
            {
                lastTouchDistance = currentDistance; // 초기 거리 저장
            }
            else if (touch1.phase == TouchPhase.Moved || touch2.phase == TouchPhase.Moved)
            {
                float deltaDistance = currentDistance - lastTouchDistance; // 거리 변화 계산 
                ZoomCamera(deltaDistance * zoomSpeed);
                lastTouchDistance = currentDistance;
                lastTouchDistance = currentDistance; // 다음 프레임을 위해 저장
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

    private void ZoomCamera(float zoomAmount)
    {
        // 카메라 줌 변경
        Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize - zoomAmount, minZoom, maxZoom);

        // 줌을 하면 카메라의 화면 범위가 바뀌므로 minBOunds와 maxBounds를 다시 적용
        AdjustCameraBounds();
    }

    private void AdjustCameraBounds()
    {
        float camHeight = Camera.main.orthographicSize * 2f;
        float camWidth = camHeight * Camera.main.aspect;

        // 확대&축소 후에도 카메라가 맵 밖으로 나가지 않도록 자동 조정
        minBounds.x = -10f + camWidth / 2;
        maxBounds.x = 10f - camWidth / 2;
        minBounds.y = -10f + camHeight / 2;
        maxBounds.y = 10f - camHeight / 2;
    }


}
