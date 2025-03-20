using UnityEngine;

public class Player : MonoBehaviour
{
    void Update()
    {
        OnMove();
    }

    private void OnMove()
    {
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended)
        {
            Touch touch = Input.GetTouch(0);
            Debug.Log("터치 감지됨!");

            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, Camera.main.nearClipPlane));

            worldPosition.z = 0;

            transform.position = worldPosition;
        }
    }
}
