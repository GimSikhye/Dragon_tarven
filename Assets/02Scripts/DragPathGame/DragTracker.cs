using System.Collections.Generic;
using UnityEngine;

public class DragTracker : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public List<Vector2> drawnPoints = new(); // 그려진 것들
    public RectTransform dragArea;
    private bool isDragging = false;

    private Vector3 lastPoint;
    public float minDistance = 5f; // 최소 거리차
    public Camera lineCam;


    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            isDragging = true;
            drawnPoints.Clear();
            lineRenderer.positionCount = 0;
        }

        if (isDragging && Input.GetMouseButton(0))
        {
            Vector3 screenPos = Input.mousePosition;
            screenPos.z = Mathf.Abs(lineCam.transform.position.z);

            Vector3 worldPos = lineCam.ScreenToWorldPoint(screenPos);

            if (lineRenderer.positionCount == 0 || Vector3.Distance(lastPoint, worldPos) >= minDistance)
            {
                drawnPoints.Add(worldPos); // localPos → worldPos
                lastPoint = worldPos;
                lineRenderer.positionCount++;
                lineRenderer.SetPosition(lineRenderer.positionCount - 1, worldPos);
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
            FindObjectOfType<DragJudge>().Evaluate(drawnPoints);
        }
    }

}
