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
        foreach (Transform child in nodeParent)
            Destroy(child.gameObject);

        currentPath.Clear();
        pathLineRenderer.positionCount = points.Length;

        for (int i = 0; i < points.Length; i++)
        {
            Vector3 localPos = points[i];
            Vector3 worldPos = nodeParent.transform.TransformPoint(localPos);

            GameObject node = Instantiate(nodePrefab, nodeParent);
            node.transform.position = worldPos;

            currentPath.Add(node.transform);
            pathLineRenderer.SetPosition(i, worldPos);
        }
    }

    public List<Vector3> GetCurrentPatternPoints()
    {
        List<Vector3> result = new();
        foreach (var t in currentPath)
            result.Add(t.position);
        return result;
    }


}
