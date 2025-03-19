using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

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
                break;
            case "Game scene":
                Debug.Log("게임씬");
                break;
        }
    }

    void Update()
    {
        
    }
}
