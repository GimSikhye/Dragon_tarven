using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private Button startButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private PlayerStats playerStats;

    private void Awake()
    {
        startButton.onClick.AddListener(() =>
        {
            // 재화 초기화
            playerStats.coffeeBean = 1000;
            playerStats.coin = 100;
            playerStats.gem = 10;

            SceneManager.LoadScene("DialogueScene");
        });
        //startButton.onClick.AddListener(() => SceneManager.LoadScene("DialogueScene"));
        quitButton.onClick.AddListener(() => Application.Quit());
    }

}
