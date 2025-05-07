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

            // �߿�: ���� ��ǥ�� �θ�(dragArea �Ǵ� canvas)�� ���ؿ��� ���� ��ǥ�� ��ȯ
            Vector3 worldPos = nodeParent.transform.TransformPoint(localPos);

            GameObject node = Instantiate(nodePrefab, nodeParent);
            node.transform.position = worldPos; // ���� ��Ȯ�� ���� ��ġ�� ��ġ

            currentPath.Add(node.transform);

            pathLineRenderer.SetPosition(i, worldPos);
        }
    }

}
