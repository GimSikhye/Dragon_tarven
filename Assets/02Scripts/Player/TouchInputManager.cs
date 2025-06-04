using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace DalbitCafe.Inputs
{
    public class TouchInputManager : MonoBehaviour
    {
        public event Action<Vector2> OnTouchBegan;
        public event Action<Vector2> OnTouchMoved;
        public event Action<Vector2> OnTouchEnded; //매개변수 vector2

        private void Update()
        {
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);

                // UI 위 터치는 무시
                if (EventSystem.current.IsPointerOverGameObject(touch.fingerId))
                    return;

                switch (touch.phase)
                {
                    case TouchPhase.Began:
                        OnTouchBegan?.Invoke(touch.position);
                        break;
                    case TouchPhase.Moved:
                    case TouchPhase.Stationary: // 터치 유지 상태
                        OnTouchMoved?.Invoke(touch.position);
                        break;
                    case TouchPhase.Ended:
                        OnTouchEnded?.Invoke(touch.position);
                        break;
                }
            }
        }
    }
}
