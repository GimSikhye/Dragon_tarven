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

    public Vector2 this[int i] => points[i]; // Path Ŭ������ ���� �ε����� �ѱ��, ������ points ����Ʈ�� i ��° ��Ҹ� ��ȯ�� (this[int i]: �ε��� ����)
        
}
