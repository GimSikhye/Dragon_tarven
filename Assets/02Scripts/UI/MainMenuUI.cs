using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private Button startButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private PlayerStatsManager playerStatsManager;
    private void Awake()
    {
        startButton.onClick.AddListener(() =>
        {

            PlayerPrefs.DeleteAll();
            //재화 초기화

            playerStatsManager.InitializeStat();
            PlayerPrefs.SetInt("CoffeeBean", playerStatsManager.CoffeeBeans);
            PlayerPrefs.SetInt("Coin", playerStatsManager.Coin);
            PlayerPrefs.SetInt("Gem", playerStatsManager.Gem);

            PlayerPrefs.Save();

            SceneManager.LoadScene("DialogueScene");
        });
        //startButton.onClick.AddListener(() => SceneManager.LoadScene("DialogueScene"));
        quitButton.onClick.AddListener(() => Application.Quit());
    }

}
