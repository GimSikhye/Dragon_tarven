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

        // 포인트 수에 따라 LineRenderer 초기화
        pathLineRenderer.positionCount = points.Length;

        for (int i = 0; i < points.Length; i++)
        {
            Vector3 pos = points[i];
            GameObject node = Instantiate(nodePrefab, nodeParent); // nodeParent 자식으로 생성
            node.transform.localPosition = pos;
            currentPath.Add(node.transform);

            // 연결선도 같이 그려줌
            Vector3 worldPos = node.transform.position;
            pathLineRenderer.SetPosition(i, worldPos);
        }
    }
}
