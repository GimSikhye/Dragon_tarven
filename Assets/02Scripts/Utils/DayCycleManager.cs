using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class DayCycleManager : MonoBehaviour
{
    [Header("UI 연결")]
    [SerializeField] private TMP_Text timeText;
    [SerializeField] private TMP_Text ampmText;
    [SerializeField] private TMP_Text dayText;

    private int day = 1;
    private int gameHour = 20; // PM 8시
    private int gameMinute = 0;

    private float tickInterval = 5f; // 5초마다 시간 흐름
    private float elapsed = 0f;

    private const int minutesPerTick = 10;
    private const int minutesPerDay = 600; // 10시간 = 600분
    private int totalGameMinutesPassed = 0;

    private void Start()
    {
        LoadDay();
        UpdateTimeUI();
        StartCoroutine(GameTimeLoop());
    }

    private IEnumerator GameTimeLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(tickInterval);
            AdvanceTime();
        }
    }

    private void AdvanceTime()
    {
        gameMinute += minutesPerTick;
        totalGameMinutesPassed += minutesPerTick;

        if (gameMinute >= 60)
        {
            gameMinute -= 60;
            gameHour = (gameHour + 1) % 24;
        }

        UpdateTimeUI();

        // 하루 종료
        if (totalGameMinutesPassed >= minutesPerDay)
        {
            EndDay();
        }
    }

    private void UpdateTimeUI()
    {
        string period = gameHour >= 12 && gameHour < 24 ? "pm" : "am";

        int displayHour = gameHour % 12;
        if (displayHour == 0) displayHour = 12;

        string formattedTime = $"{displayHour}:{gameMinute:00}";
        timeText.text = formattedTime;
        ampmText.text = period;

        dayText.text = $"Day {day}";
    }

    private void EndDay()
    {
        Debug.Log($"[DayCycle] Day {day} 종료, 정산 씬 이동!");
        SaveDay();
        SceneManager.LoadScene("SettlementScene"); // 정산 씬으로 전환
    }

    private void SaveDay()
    {
        PlayerPrefs.SetInt("Day", day);
        PlayerPrefs.SetFloat("Coin", PlayerStatsManager.Instance.Coin);
        PlayerPrefs.SetInt("CoffeeBean", PlayerStatsManager.Instance.CoffeeBeans);
        PlayerPrefs.Save();
    }

    public void LoadDay()
    {
        day = PlayerPrefs.GetInt("Day", 1); // 기본 1일차부터
    }

    public void AdvanceToNextDay()
    {
        day++;
        PlayerPrefs.SetInt("Day", day);
        PlayerPrefs.Save();

        // 다음날 초기화
        gameHour = 20;
        gameMinute = 0;
        totalGameMinutesPassed = 0;

        UpdateTimeUI();
        StartCoroutine(GameTimeLoop());
    }
}
