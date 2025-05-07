using System.Collections.Generic;
using UnityEngine;

public class DragTracker : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public List<Vector2> drawnPoints = new(); // �׷��� �͵�
    public RectTransform dragArea;
    private bool isDragging = false;

    private Vector3 lastPoint;
    public float minDistance = 5f; // �ּ� �Ÿ���
    

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
            Vector2 localPos; // ScreenPointToLocalPointInRectangle �Լ�
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                dragArea, Input.mousePosition, null, out localPos); // pos ��ȯ�� ���� ��ǥ ���


            Vector3 worldPos = dragArea.TransformPoint(localPos);


            // ���� ���� ���� �Ÿ� �̻��� ���� �߰�
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
            FindObjectOfType<DragJudge>().Evaluate(drawnPoints); // ����
        }
    }

}
