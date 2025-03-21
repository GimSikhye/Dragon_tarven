using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float dragSpeed = 0.1f; // 드래그 속도 조절
    public Vector2 minLimit; // 카메라 이동 최소 좌표(x,y)
    public Vector2 maxLimit; // 카메라 이동 최대 좌표(x,y)

    private Vector3 dragOrigin; // 드래그 시작점


    void Start()
    {
        
    }

    void Update()
    {
        HandleTouchInput();
    }

    private void HandleTouchInput()
    {
        if(Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);

        }
    }
}
