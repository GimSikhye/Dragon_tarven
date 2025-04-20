using UnityEngine;

public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    protected static T instance;

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject obj = GameObject.Find(typeof(T).Name);
                if (obj == null)
                {
                    obj = new GameObject(typeof(T).Name);
                    instance = obj.AddComponent<T>();
                }
                else
                {
                    instance = obj.GetComponent<T>();
                }
            }
            return instance;
        }
    }

    public static bool HasInstance => instance != null; // ������ �ִٸ� true 

    // �ܺο��� �̸� ���� instance�� �Ҵ��ϱ� ����
    public static void AssignInstance(T inst)
    {
        if (instance == null)
        {
            instance = inst;
        }
    }

    protected virtual void Awake()
    {
        if (instance == null)
        {
            instance = this as T;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject); // �ߺ� ���� ����
        }
    }
}
