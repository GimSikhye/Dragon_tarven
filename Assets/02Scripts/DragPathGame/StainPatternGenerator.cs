using System.Collections.Generic;
using UnityEngine;

public class StainPatternGenerator : MonoBehaviour
{
    public Transform nodeParent;
    public GameObject nodePrefab;
    public List<Transform> currentPath = new();
    public LineRenderer pathLineRenderer;

    public void GeneratePattern(Vector3[] points)
    {
        foreach (Transform child in nodeParent) Destroy(child.gameObject);
        currentPath.Clear();

        pathLineRenderer.positionCount = points.Length;

        for (int i = 0; i < points.Length; i++)
        {
            Vector3 localPos = points[i];

            // 중요: 로컬 좌표를 부모(dragArea 또는 canvas)의 기준에서 월드 좌표로 변환
            Vector3 worldPos = nodeParent.transform.TransformPoint(localPos);

            GameObject node = Instantiate(nodePrefab, nodeParent);
            node.transform.position = worldPos; // 이제 정확한 월드 위치로 배치

            currentPath.Add(node.transform);

            pathLineRenderer.SetPosition(i, worldPos);
        }
    }

}
