using UnityEngine;

public class PatternTester : MonoBehaviour
{
    public StainPatternGenerator generator;

    private Vector3[][] patternShapes;

    void Awake()
    {
        // 하드코딩된 도형 + 함수형 도형 결합
        patternShapes = new Vector3[][]
        {
            // 별
            new Vector3[]
            {
                new Vector3(-30, 0, 0),
                new Vector3(0, 45, 0),
                new Vector3(30, 0, 0),
                new Vector3(-30, 30, 0),
                new Vector3(30, 30, 0),
            },

            // 삼각형
            new Vector3[]
            {
                new Vector3(-40, -30, 0),
                new Vector3(40, -30, 0),
                new Vector3(0, 40, 0),
            },

            // 사각형
            new Vector3[]
            {
                new Vector3(-30, -30, 0),
                new Vector3(-30, 30, 0),
                new Vector3(30, 30, 0),
                new Vector3(30, -30, 0),
            },


            GenerateCirclePattern(),
            GenerateHeartPattern()
        };
    }

    void Start()
    {
        int randomIndex = Random.Range(0, patternShapes.Length);
        Vector3[] chosenPattern = patternShapes[randomIndex];
        generator.GeneratePattern(chosenPattern);
    }

    // 원형
    Vector3[] GenerateCirclePattern(int segments = 12, float radius = 30f)
    {
        Vector3[] circle = new Vector3[segments];
        for (int i = 0; i < segments; i++)
        {
            float angle = 2 * Mathf.PI * i / segments;
            float x = Mathf.Cos(angle) * radius;
            float y = Mathf.Sin(angle) * radius;
            circle[i] = new Vector3(x, y, 0);
        }
        return circle;
    }

    //  하트
    Vector3[] GenerateHeartPattern(int segments = 40, float scale = 2f)
    {
        Vector3[] heart = new Vector3[segments];
        for (int i = 0; i < segments; i++)
        {
            float t = Mathf.PI * 2 * i / segments;
            float x = 16 * Mathf.Pow(Mathf.Sin(t), 3);
            float y = 13 * Mathf.Cos(t) - 5 * Mathf.Cos(2 * t) - 2 * Mathf.Cos(3 * t) - Mathf.Cos(4 * t);
            heart[i] = new Vector3(x, y, 0) * scale * 0.5f;
        }
        return heart;
    }
}
