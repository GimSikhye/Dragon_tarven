using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [SerializeField] private AudioClip[] bgm_clips;

    [Header("재화")]
    [SerializeField] private int coffeeBean;
    [SerializeField] private int gem;
    [SerializeField] private int coin;

    void Awake()
    {
        if(Instance == null) // 첫 번째 GameManager라면 유지
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

        }
        else // 기존 GameManager가 있다면, 새 씬의 GameManager를 유지하고 기존 것을 삭제
        {
            Destroy(Instance.gameObject);
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        SceneManager.sceneLoaded += ChangeScene;
    }

    private void Start()
    {
        // 게임 시작 시 저장된 데이터 불러오기
        coffeeBean = PlayerPrefs.GetInt("CoffeeBean", 100);
        coin = PlayerPrefs.GetInt("Coin", 1000);
        gem = PlayerPrefs.GetInt("Gem", 0);

        // UI 업데이트
        UIManager.Instance.UpdateCoffeeBeanUI(coffeeBean);
        UIManager.Instance.UpdateCoinUI(coin);
        UIManager.Instance.UpdateGemUI(gem);

    }


    public int CoffeeBean
    {
        get { return coffeeBean; }
        set
        {
            coffeeBean = value;
            PlayerPrefs.SetInt("CoffeeBean", coffeeBean); // 자동 저장
            UIManager.Instance.UpdateCoffeeBeanUI(coffeeBean);
        }
    }

    public int Coin
    {
        get { return coin; }
        set
        {
            gem = value;
            PlayerPrefs.SetInt("Coin", coin); // 자동 저장
            UIManager.Instance.UpdateCoinUI(coin);
        }
    }

    public int Gem
    {
        get { return gem; }
        set
        {
            gem = value;
            PlayerPrefs.SetInt("Gem", gem); // 자동 저장
            UIManager.Instance.UpdateGemUI(gem);

        }
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
                //Debug.Log("게임씬");
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
