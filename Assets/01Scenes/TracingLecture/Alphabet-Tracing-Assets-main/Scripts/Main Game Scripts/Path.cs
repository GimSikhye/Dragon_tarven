using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class Path
{
    public List<Vector2> points;

    public Path(Vector2 centre)
    {
        points = new List<Vector2>
        {
            centre + Vector2.left,
            centre + Vector2.left * 0.35f,
            centre + Vector2.right * 0.35f,
            centre + Vector2.right
        };
    }

    public Vector2 this[int i] => points[i]; // Path 클래스에 정수 인덱스를 넘기면, 내부의 points 리스트의 i 번째 요소를 반환함 (this[int i]: 인덱서 정의)
        
}
