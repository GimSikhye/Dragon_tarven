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
    

    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            isDragging = true;
            drawnPoints.Clear();
            lineRenderer.positionCount = 0; // positionCount?
        }

        if (isDragging && Input.GetMouseButton(0))
        {
            Vector2 localPos; // ScreenPointToLocalPointInRectangle 함수
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                dragArea, Input.mousePosition, null, out localPos); // pos 변환된 로컬 좌표 결과


            Vector3 worldPos = dragArea.TransformPoint(localPos);


            // 이전 점과 일정 거리 이상일 때만 추가
            if (lineRenderer.positionCount == 0 || Vector3.Distance(lastPoint, worldPos) >= minDistance)
            {
                drawnPoints.Add(localPos);
                lastPoint = worldPos;
                lineRenderer.positionCount++;
                lineRenderer.SetPosition(lineRenderer.positionCount - 1, worldPos);

            }

        }

        if(Input.GetMouseButtonUp(0))
        {
            isDragging = false;
            FindObjectOfType<DragJudge>().Evaluate(drawnPoints); // 판정
        }
    }

}
