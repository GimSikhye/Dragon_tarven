using System.Collections.Generic;
using UnityEngine;

public class DragJudge : MonoBehaviour
{
    public StainPatternGenerator generator;
    public float hitRange = 0.5f;
    public float loopCloseThreshold = 80f; // 루프가 닫혔다고 인정할 거리
    
    public void Evaluate(List<Vector3> drawn) // 그려진것들을 판정
    {
        List<Transform> targets = generator.currentPath; // 기군 점들
        int hits = 0;

        // 1. 각 점들이 다 맞았는지 체크
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

        float accuracy = (float)hits / targets.Count; // 정확성

        // 2. 루프가 닫혔는지 확인 ( 맨 처음점 - 마지막점)
        bool loopClosed = drawn.Count > 2 && Vector3.Distance(drawn[0], drawn[drawn.Count - 1]) < loopCloseThreshold;

        // 3. 판정 결정
        string result = "BAD";
        if (accuracy >= 0.9f && loopClosed) result = "PERFECT";
        else if (accuracy >= 0.7f) result = "GREAT";
        else if (accuracy >= 0.4f) result = "GOOD";

        Debug.Log($"Result: {result}, Accuracy: {accuracy}");
        // 피드백 이펙트 호출 etc.



    }

}
