using System.Collections.Generic;
using UnityEngine;

public class DragJudge : MonoBehaviour
{
    public StainPatternGenerator generator;
    public float hitRange = 50f;
    
    public void Evaluate(List<Vector2> drawn) // �׷����͵��� ����
    {
        List<Transform> targets = generator.currentPath; // ���� currentPath
        int hits = 0;

        foreach(Transform node in targets)
        {
            foreach(Vector2 point in drawn)
            {
                if(Vector2.Distance(point, node.localPosition) < hitRange)
                {
                    hits++;
                    break;
                }
            }
        }

        float accuracy = (float)hits / targets.Count; // ��Ȯ��
        string result = "BAD";
        if (accuracy >= 0.9f) result = "PERFECT";
        else if (accuracy >= 0.7f) result = "GREAT";
        else if (accuracy >= 0.4f) result = "GOOD";

        Debug.Log($"Result: {result}, Accuracy: {accuracy}");
        // �ǵ�� ����Ʈ ȣ�� etc.



    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
