using UnityEngine;

public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour // where T: Monobehaviour를 상속받아야 한다는 제약 조건
{
    private static T instance;

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                Debug.LogError($"[MonoSingleton] {typeof(T)}의 인스턴스가 초기화되지 않았습니다. " +
                "사용하기 전에 씬에 존재하는지 또는 인스턴스화되었는지 확인하세요.");
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
