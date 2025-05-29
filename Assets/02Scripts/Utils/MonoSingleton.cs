using UnityEngine;

public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                Debug.LogError($"[MonoSingleton] Instance of {typeof(T)} is not initialized. " +
                               "Ensure it exists in the scene or is instantiated before use.");
            }
            return instance;
        }
    }

    public static bool HasInstance => instance != null;

    protected virtual void Awake()
    {
        if (instance == null)
        {
            instance = this as T;
            Debug.Log($"[MonoSingleton] {typeof(T).Name} 인스턴스 등록 완료");
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Debug.LogWarning($"[MonoSingleton] 중복 인스턴스 감지: {typeof(T).Name} 제거됨");
            Destroy(gameObject);
        }
    }

}
