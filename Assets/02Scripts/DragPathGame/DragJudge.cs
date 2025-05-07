using System.Collections.Generic;
using UnityEngine;

public class DragJudge : MonoBehaviour
{
    public StainPatternGenerator generator;
    public float hitRange = 0.5f;
    public float loopCloseThreshold = 80f; // ������ �����ٰ� ������ �Ÿ�
    
    public void Evaluate(List<Vector3> drawn) // �׷����͵��� ����
    {
        List<Transform> targets = generator.currentPath; // �ⱺ ����
        int hits = 0;

        // 1. �� ������ �� �¾Ҵ��� üũ
        foreach(Transform node in targets)
        {
            foreach(Vector3 point in drawn)
            {
                if(Vector3.Distance(point, node.position) < hitRange)
                {
                    hits++;
                    break;
                }
            }
        }

        float accuracy = (float)hits / targets.Count; // ��Ȯ��

        // 2. ������ �������� Ȯ�� ( �� ó���� - ��������)
        bool loopClosed = drawn.Count > 2 && Vector3.Distance(drawn[0], drawn[drawn.Count - 1]) < loopCloseThreshold;

        // 3. ���� ����
        string result = "BAD";
        if (accuracy >= 0.9f && loopClosed) result = "PERFECT";
        else if (accuracy >= 0.7f) result = "GREAT";
        else if (accuracy >= 0.4f) result = "GOOD";

        Debug.Log($"Result: {result}, Accuracy: {accuracy}");
        // �ǵ�� ����Ʈ ȣ�� etc.



    }

}
