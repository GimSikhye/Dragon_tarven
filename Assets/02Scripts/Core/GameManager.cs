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
    public class GameManager : MonoSingleton<GameManager>
    {
        [SerializeField] private AudioClip[] bgm_clips;

        [Header("�÷��̾� ������")]
        public PlayerStats playerStats;

        void Awake()
        {
            SceneManager.sceneLoaded += ChangeScene;
        }

        private void Start()
        {
            playerStats.LoadFromPrefs();

            // UI ������Ʈ
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
                    Debug.Log("���θ޴�");
                    SoundManager.Instance.PlayBGM(bgm_clips[(int)Bgm.Main], 0.5f);
                    break;
                case "GameScene":
                    SoundManager.Instance.PlayBGM(bgm_clips[(int)Bgm.Game], 0.5f);
                    break;
            }
        }


    }

}
