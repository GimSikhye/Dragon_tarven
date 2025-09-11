using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;


namespace DalbitCafe.Cameras
{
    public class CameraController : MonoBehaviour
    {
        [Header("화면 이동")]
        public float dragSpeed = 0.001f; // 드래그 민감도 (낮출수록 천천히)
        public float smoothTime = 0.15f;  // 부드럽게 이동할 시간
        public Vector2 minBounds = new Vector2(-10f, -10f);
        public Vector2 maxBounds = new Vector2(10f, 10f);

        [Header("줌 이동")]
        public float zoomSpeed = 0.001f; // 줌 민감도 (낮출수록 천천히)
        public float zoomSmoothTime = 0.15f;
        public float minZoom = 3f;
        public float maxZoom = 10f;

        private Camera cam;
        private bool isTouchingUI = false;

        private Vector3 velocity = Vector3.zero; // SmoothDamp용
        private float zoomVelocity = 0f;         // SmoothDamp용
        private Vector3 targetPos;               // 목표 카메라 위치
        private float targetZoom;                // 목표 줌 값

        private void Awake()
        {
            cam = Camera.main;
            targetPos = transform.position;
            targetZoom = cam.orthographicSize;
        }

        void Update()
        {
            if (Input.touchCount == 1)
            {
                HandleDrag();
            }
            else if (Input.touchCount == 2)
            {
                HandlePinchZoom();
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
            }

            if (isTouchingUI) return;

            if (touch.phase == TouchPhase.Moved)
            {
                Vector2 delta = touch.deltaPosition * dragSpeed;

                Vector3 move = new Vector3(-delta.x, -delta.y, 0);
                SetTargetPosition(move);
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

            float delta = currentDist - prevDist;

            targetZoom = Mathf.Clamp(targetZoom - delta * zoomSpeed, minZoom, maxZoom);
        }

        private void SetTargetPosition(Vector3 move)
        {
            Vector3 newPos = targetPos + move;

            newPos.x = Mathf.Clamp(newPos.x, minBounds.x, maxBounds.x);
            newPos.y = Mathf.Clamp(newPos.y, minBounds.y, maxBounds.y);

            targetPos = newPos;
        }
    }
}