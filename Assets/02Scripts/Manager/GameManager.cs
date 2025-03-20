using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [SerializeField] private AudioClip[] bgm_clips;
    [Header("재화")]
    [SerializeField] private ushort beans_count;
    [SerializeField] private ushort coin_count;
    [SerializeField] private ushort gem_count;

    void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

        }else 
            Destroy(gameObject);

        SceneManager.sceneLoaded += ChangeScene;
    }

    private void ChangeScene(Scene scene, LoadSceneMode mode)
    {
        switch (scene.name)
        {
            case "Main menu":
                Debug.Log("메인메뉴");
                SoundManager.Instance.PlayBGM(bgm_clips[0], 0.5f);
                break;
            case "Game scene":
                Debug.Log("게임씬");
                SoundManager.Instance.PlayBGM(bgm_clips[1], 0.5f);
                break;
        }
    }

    void Update()
    {
        if(Application.platform == RuntimePlatform.Android)
        {
            if(Input.GetKey(KeyCode.Escape))
            {
                UIManager.Instance.ShowExitWindow();
            }
        }
    }
}
