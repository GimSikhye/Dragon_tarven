using UnityEngine;
using UnityEngine.AI; // Nav Mesh �� ����ϱ� ���� �ʿ��� using ��

public class Monster : MonoBehaviour
{

    private static Monster instance;

    public static Monster Instance
    {
        get
        {
            if (instance == null) instance = new Monster();
            return instance;
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

    }


}