using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class InputManager : MonoBehaviour
{
    public static event Action<Vector3> OnTouchEnded; // 터치 끝 이벤트 (월드 좌표 기준)

    // 이동 시 UI 위인지 체크
    private GraphicRaycaster uiRaycaster;
    private EventSystem eventSystem;
    private PointerEventData pointerEventData;

    void Awake()
    {

        uiRaycaster = FindObjectOfType<GraphicRaycaster>();
        eventSystem = EventSystem.current;
    }

    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Ended)
            {
                if (IsTouchOverUI(touch.position))
                {
                    Debug.Log("UI 위에서 손 뗌 -> 이동 안함");
                    return;
                }

                Vector3 worldPosition = Camera.main.ScreenToWorldPoint(touch.position);
                worldPosition.z = 0f;

                OnTouchEnded?.Invoke(worldPosition);
            }
        }
    }

    private bool IsTouchOverUI(Vector2 screenPosition)
    {
        pointerEventData = new PointerEventData(eventSystem);
        pointerEventData.position = screenPosition;

        List<RaycastResult> results = new List<RaycastResult>();
        uiRaycaster.Raycast(pointerEventData, results);

        return results.Count > 0;
    }
}
