using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;


namespace DalbitCafe.Cameras
{
    public class CameraController : MonoBehaviour
    {
        [Header("화면 이동")]
        public float dragSpeed = 0.3f;        // 드래그 민감도 (월드 단위 기준)
        public float smoothTime = 0.3f;       // 부드럽게 이동 시간
        public float dragThreshold = 5f;      // 드래그 시작 최소 거리(px)
        public Vector2 minBounds = new Vector2(-10f, -10f);
        public Vector2 maxBounds = new Vector2(10f, 10f);

        [Header("줌 이동")]
        public float zoomSpeed = 0.5f;        // 줌 민감도
        public float zoomSmoothTime = 0.15f;
        public float minZoom = 3f;
        public float maxZoom = 10f;
        public float maxZoomDelta = 0.5f;     // 프레임당 최대 줌 변화

        [Header("관성")]
        public float inertiaDamping = 5f;     // 관성 감속 속도

        private Camera cam;
        private bool isTouchingUI = false;
        private Vector2 dragStartPos;

        private Vector3 velocity = Vector3.zero;      // SmoothDamp용
        private Vector3 targetPos;
        private Vector3 dragVelocity = Vector3.zero;  // 관성용

        private float targetZoom;
        private float zoomVelocity = 0f;

        private void Awake()
        {
            cam = Camera.main;
            targetPos = transform.position;
            targetZoom = cam.orthographicSize;
        }

        private void Update()
        {
            if (Input.touchCount == 1)
            {
                HandleDrag();
            }
            else if (Input.touchCount == 2)
            {
                HandlePinchZoom();
            }

            // 관성 적용 (터치 끝난 후에도 서서히 이동)
            if (Input.touchCount == 0 && dragVelocity.magnitude > 0.01f)
            {
                SetTargetPosition(dragVelocity * Time.deltaTime);
                dragVelocity = Vector3.Lerp(dragVelocity, Vector3.zero, inertiaDamping * Time.deltaTime);
            }

            // 카메라 위치 보간
            transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref velocity, smoothTime);

            // 카메라 줌 보간
            cam.orthographicSize = Mathf.SmoothDamp(cam.orthographicSize, targetZoom, ref zoomVelocity, zoomSmoothTime);
        }

        private void HandleDrag()
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                isTouchingUI = EventSystem.current.IsPointerOverGameObject(touch.fingerId);
                dragStartPos = touch.position;
                dragVelocity = Vector3.zero;
            }

            if (isTouchingUI) return;

            if (touch.phase == TouchPhase.Moved)
            {
                if (Vector2.Distance(touch.position, dragStartPos) < dragThreshold) return;

                // 픽셀 단위를 월드 단위로 변환
                Vector3 worldDelta = cam.ScreenToWorldPoint(touch.position) - cam.ScreenToWorldPoint(touch.position - touch.deltaPosition);
                Vector3 move = -worldDelta * dragSpeed;

                SetTargetPosition(move);

                // 관성 저장
                dragVelocity = move / Time.deltaTime;
            }

            if (touch.phase == TouchPhase.Ended)
            {
                isTouchingUI = false;
            }
        }

        private void HandlePinchZoom()
        {
            Touch touch0 = Input.GetTouch(0);
            Touch touch1 = Input.GetTouch(1);

            if (EventSystem.current.IsPointerOverGameObject(touch0.fingerId) ||
                EventSystem.current.IsPointerOverGameObject(touch1.fingerId))
                return;

            float prevDist = (touch0.position - touch0.deltaPosition - (touch1.position - touch1.deltaPosition)).magnitude;
            float currentDist = (touch0.position - touch1.position).magnitude;

            float delta = (currentDist - prevDist) * zoomSpeed * Time.deltaTime * 60f;
            delta = Mathf.Clamp(delta, -maxZoomDelta, maxZoomDelta);

            targetZoom = Mathf.Clamp(targetZoom - delta, minZoom, maxZoom);
        }

        private void SetTargetPosition(Vector3 move)
        {
            Vector3 newPos = targetPos + move;

            // 맵 범위 제한
            newPos.x = Mathf.Clamp(newPos.x, minBounds.x, maxBounds.x);
            newPos.y = Mathf.Clamp(newPos.y, minBounds.y, maxBounds.y);

            targetPos = newPos;
        }
    }
}