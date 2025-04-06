using UnityEngine;
using UnityEngine.SceneManagement;
using DalbitCafe.UI;
enum Bgm
{
    Main = 0,
    Game,
    End
}
namespace DalbitCafe.Core
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;
        [SerializeField] private AudioClip[] bgm_clips;

        [Header("플레이어 데이터")]
        public PlayerStats playerStats;

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
            playerStats.LoadFromPrefs();

            // UI 업데이트
            UIManager.Instance.UpdateCoffeeBeanUI(playerStats.coffeeBean);
            UIManager.Instance.UpdateCoinUI(playerStats.coin);
            UIManager.Instance.UpdateGemUI(playerStats.gem);

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

        private void ChangeScene(Scene scene, LoadSceneMode mode)
        {
            switch (scene.name)
            {
                case "MainMenu":
                    Debug.Log("메인메뉴");
                    SoundManager.Instance.PlayBGM(bgm_clips[(int)Bgm.Main], 0.5f);
                    break;
                case "GameScene":
                    SoundManager.Instance.PlayBGM(bgm_clips[(int)Bgm.Game], 0.5f);
                    break;
            }
        }


    }

}
