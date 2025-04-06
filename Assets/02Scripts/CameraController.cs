using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;


namespace DalbitCafe.Cameras
{
    public class CameraController : MonoBehaviour
    {
        [Header("ȭ�� �̵�")]
        public float dragSpeed = 0.1f; // �巡�� �ӵ� ����
        public Vector2 minBounds; // �ּ� ��ǥ(���� �Ʒ�)
        public Vector2 maxBounds; // �ִ� ��ǥ(������ ��)
        [Space(10)]
        [Header("�� �̵�")]
        public float zoomSpeed = 0.05f; // �� �ӵ� ����
        public float minZoom = 3f; // �ּ� ��(�ִ�� Ȯ��)
        public float maxZoom = 10f; // �ִ� ��(�ִ�� ���)


        private Vector3 lastTouchPosition;
        private float lastTouchDistance;
        private bool isTouchingUI = false; // UI ��ġ ���� ����

        void Update()
        {
            HandleCameraDrag();
            HandlePinchZoom();
        }

        
        private void HandleCameraDrag()
        {
            if (Input.touchCount == 1)
            {
                Touch touch = Input.GetTouch(0);

                if (touch.phase == TouchPhase.Began)
                {
                    // ��ġ ���� �� UI ���� �ִ��� Ȯ��
                    isTouchingUI = EventSystem.current.IsPointerOverGameObject(touch.fingerId);
                    lastTouchPosition = Camera.main.ScreenToWorldPoint(touch.position);
                }
                else if (touch.phase == TouchPhase.Moved)
                {
                    // �巡�� �߿��� UI ������ Ȯ���Ͽ� UI���� ���������� ȭ�� �̵� ����
                    if (isTouchingUI) return;

                    Vector3 currentPosition = Camera.main.ScreenToWorldPoint(touch.position);
                    Vector3 direction = currentPosition - lastTouchPosition; // �̵����� ���
                    MoveCamera(-direction);
                    lastTouchPosition = currentPosition;
                }
                else if (touch.phase == TouchPhase.Ended)
                {
                    // ��ġ�� ������ UI ��ġ ���� �ʱ�ȭ
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

                // �� �հ��� �� �ϳ��� UI�� ��ġ������ ��X
                if (EventSystem.current.IsPointerOverGameObject(touch1.fingerId) || EventSystem.current.IsPointerOverGameObject(touch2.fingerId))
                    return;

                // ���� �� �հ��� �Ÿ� ���(��� ����)
                float currentDistance = Vector2.Distance(touch1.position, touch2.position);

                if (touch1.phase == TouchPhase.Began || touch2.phase == TouchPhase.Began)
                {
                    lastTouchDistance = currentDistance; // �ʱ� �Ÿ� ����
                }
                else if (touch1.phase == TouchPhase.Moved || touch2.phase == TouchPhase.Moved)
                {
                    float deltaDistance = currentDistance - lastTouchDistance; // �Ÿ� ��ȭ ��� 
                    ZoomCamera(deltaDistance * zoomSpeed);
                    lastTouchDistance = currentDistance;
                    lastTouchDistance = currentDistance; // ���� �������� ���� ����
                }
            }
        }

        private void MoveCamera(Vector3 moveDirection)
        {
            Vector3 newPosition = transform.position + moveDirection * dragSpeed; // ���ο� ��ġ ���

            // ī�޶� ��ġ�� ���ѵ� ���� ������ ����
            newPosition.x = Mathf.Clamp(newPosition.x, minBounds.x, maxBounds.x);
            newPosition.y = Mathf.Clamp(newPosition.y, minBounds.y, maxBounds.y);

            transform.position = newPosition;
        }

        private void ZoomCamera(float zoomAmount)
        {
            // ī�޶� �� ����
            Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize - zoomAmount, minZoom, maxZoom);

            // ���� �ϸ� ī�޶��� ȭ�� ������ �ٲ�Ƿ� minBOunds�� maxBounds�� �ٽ� ����
            AdjustCameraBounds();
        }

        private void AdjustCameraBounds()
        {
            float camHeight = Camera.main.orthographicSize * 2f;
            float camWidth = camHeight * Camera.main.aspect;

            // Ȯ��&��� �Ŀ��� ī�޶� �� ������ ������ �ʵ��� �ڵ� ����
            minBounds.x = -10f + camWidth / 2;
            maxBounds.x = 10f - camWidth / 2;
            minBounds.y = -10f + camHeight / 2;
            maxBounds.y = 10f - camHeight / 2;
        }


    }

}




