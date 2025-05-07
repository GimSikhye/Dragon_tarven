using UnityEngine;

public class PatternTester : MonoBehaviour
{
    public StainPatternGenerator generator;
    void Start()
    {
        Vector3[] starShape = new Vector3[]
        {
            new Vector3(-100, 0, 0),
            new Vector3(0, 150, 0),
            new Vector3(100, 0, 0),
            new Vector3(-100, 75, 0),
            new Vector3(100, 75, 0),

        };

        generator.GeneratePattern(starShape);

    }


}
