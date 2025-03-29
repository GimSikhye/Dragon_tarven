using UnityEngine;
using UnityEngine.SceneManagement;
using DalbitCafe.UI;

namespace DalbitCafe.Core
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;
        [SerializeField] private AudioClip[] bgm_clips;

        [Header("재화")]
        [SerializeField] private int _coffeeBean;
        [SerializeField] private int _gem;
        [SerializeField] private int _coin;

        void Awake()
        {
            if (Instance == null) // 첫 번째 GameManager라면 유지
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
            _coffeeBean = PlayerPrefs.GetInt("CoffeeBean", 100);
            _coin = PlayerPrefs.GetInt("Coin", 1000);
            _gem = PlayerPrefs.GetInt("Gem", 0);

            // UI 업데이트
            UIManager.Instance.UpdateCoffeeBeanUI(_coffeeBean);
            UIManager.Instance.UpdateCoinUI(_coin);
            UIManager.Instance.UpdateGemUI(_gem);

        }


        public int CoffeeBean
        {
            get { return _coffeeBean; }
            set
            {
                _coffeeBean = value;
                PlayerPrefs.SetInt("CoffeeBean", _coffeeBean); // 자동 저장
                UIManager.Instance.UpdateCoffeeBeanUI(_coffeeBean);
            }
        }

        public int Coin
        {
            get { return _coin; }
            set
            {
                _coin = value;
                PlayerPrefs.SetInt("Coin", _coin); // 자동 저장
                UIManager.Instance.UpdateCoinUI(_coin);
            }
        }

        public int Gem
        {
            get { return _gem; }
            set
            {
                _gem = value;
                PlayerPrefs.SetInt("Gem", _gem); // 자동 저장
                UIManager.Instance.UpdateGemUI(_gem);

            }
        }


        private void ChangeScene(Scene scene, LoadSceneMode mode)
        {
            switch (scene.name)
            {
                case "MaiMenu":
                    Debug.Log("메인메뉴");
                    SoundManager.Instance.PlayBGM(bgm_clips[0], 0.5f);
                    break;
                case "GameScene":
                    //Debug.Log("게임씬");
                    SoundManager.Instance.PlayBGM(bgm_clips[1], 0.5f);
                    break;
            }
        }

        void Update()
        {
            if (Application.platform == RuntimePlatform.Android)
            {
                if (Input.GetKey(KeyCode.Escape))
                {
                    UIManager.Instance.ShowExitPopUp();
                }
            }
        }
    }

}
