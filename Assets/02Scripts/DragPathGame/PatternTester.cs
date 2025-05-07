using UnityEngine;

public class PatternTester : MonoBehaviour
{
    public StainPatternGenerator generator;
    void Start()
    {
        Vector3[] starShape = new Vector3[] // 현재 위치가 어떻게 그려지는지 확인해야함. (원리)
        {
            new Vector3(-30, 0, 0), // 
            new Vector3(0, 45, 0), // x : 100 y : 150
            new Vector3(30, 0, 0), // // x : 200
            new Vector3(-30, 30, 0), // y : 75
            new Vector3(30, 30, 0),

        };

        generator.GeneratePattern(starShape);

    }


}
