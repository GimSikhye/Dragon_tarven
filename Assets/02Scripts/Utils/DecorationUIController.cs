using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class DecorationUIController : MonoBehaviour
{
    [SerializeField] private GameObject decoPanel;
    [SerializeField] private GameObject storePanel;
    [SerializeField] private Button decorationButton;
    [SerializeField] private Button nextDayButton; // 다음 일차 넘어가기

    void Start()
    {
        nextDayButton.onClick.AddListener(ProceedToNextDay);
    }


    void ProceedToNextDay() // 진행하다
    {
        SaveData();
        SceneManager.LoadScene("GameScene");
    }

    void SaveData()
    {
        PlayerPrefs.SetInt("Day", PlayerPrefs.GetInt("Day", 1) + 1);
        PlayerPrefs.SetFloat("Coin", PlayerStatsManager.Instance.Coin);
        PlayerPrefs.SetInt("CoffeeBean", PlayerStatsManager.Instance.CoffeeBeans);
        PlayerPrefs.SetInt("Gem", PlayerStatsManager.Instance.Gem);
        PlayerPrefs.Save(); // PlayerPrefs Save 공부
    }
}
