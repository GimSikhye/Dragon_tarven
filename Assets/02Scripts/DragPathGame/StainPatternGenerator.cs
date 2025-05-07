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

        // ����Ʈ ���� ���� LineRenderer �ʱ�ȭ
        pathLineRenderer.positionCount = points.Length;

        for (int i = 0; i < points.Length; i++)
        {
            Vector3 pos = points[i];
            GameObject node = Instantiate(nodePrefab, nodeParent); // nodeParent �ڽ����� ����
            node.transform.localPosition = pos;
            currentPath.Add(node.transform);

            // ���ἱ�� ���� �׷���
            Vector3 worldPos = node.transform.position;
            pathLineRenderer.SetPosition(i, worldPos);
        }
    }
}
