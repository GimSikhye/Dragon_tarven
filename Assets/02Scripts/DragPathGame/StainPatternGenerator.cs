using System.Collections.Generic;
using UnityEngine;

public class StainPatternGenerator : MonoBehaviour
{
    public Transform nodeParent;
    public GameObject nodePrefab;
    public List<Transform> currentPath = new();

    public void GeneratePattern(Vector3[] points)
    {
        foreach (Transform child in nodeParent) Destroy(child.gameObject);
        currentPath.Clear();

        foreach(Vector3 pos in points)
        {
            GameObject node = Instantiate(nodePrefab, nodeParent);
            node.transform.localPosition = pos;
            currentPath.Add(node.transform);
        }
    }
 
}
