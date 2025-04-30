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
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject); // 중복 방지
        }
    }
}
