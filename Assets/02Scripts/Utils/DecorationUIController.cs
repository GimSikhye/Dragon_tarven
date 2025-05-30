using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class DecorationUIController : MonoBehaviour
{
    [SerializeField] private GameObject decoPanel;
    [SerializeField] private GameObject storePanel;
    [SerializeField] private Button decorationButton;
    [SerializeField] private Button nextDayButton;

    void Start()
    {
        decorationButton.onClick.AddListener(OpenDecorationPanel);
        nextDayButton.onClick.AddListener(ProceedToNextDay);
    }

    void OpenDecorationPanel()
    {
        decoPanel.SetActive(true);
        storePanel.SetActive(false);
    }

    void ProceedToNextDay()
    {
        SaveData();
        SceneManager.LoadScene("GameScene");
    }

    void SaveData()
    {
        PlayerPrefs.SetInt("Day", PlayerPrefs.GetInt("Day", 1) + 1);
        PlayerPrefs.SetInt("Coin", PlayerStatsManager.Instance.Coin);
        PlayerPrefs.SetInt("CoffeeBean", PlayerStatsManager.Instance.CoffeeBeans);
        PlayerPrefs.SetInt("Gem", PlayerStatsManager.Instance.Gem);
        PlayerPrefs.Save();
    }
}
