using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class DayCycleManager : MonoBehaviour
{
    [Header("UI ����")]
    [SerializeField] private TMP_Text timeText;
    [SerializeField] private TMP_Text ampmText;
    [SerializeField] private TMP_Text dayText;

    private int day = 1;
    private int gameHour = 20; 
    private int gameMinute = 0;
    private float tickInterval = 5f; // 5�ʸ��� �ð� �帧
    private float elapsed = 0f;
    private bool showColon = true;
    private const int minutesPerTick = 10;
    private const int minutesPerDay = 600; // 10�ð� = 600��
    private int totalGameMinutesPassed = 0;

    // �ð� �帧 ����� ����
    private bool _isTimePaused = false;
    private Coroutine _gameTimeCoroutine;
    private Coroutine _blinkCoroutine;

    private void Start()
    {
        LoadDay();
        UpdateTimeUI();
        _gameTimeCoroutine = StartCoroutine(GameTimeLoop());
        _blinkCoroutine = StartCoroutine(BlinkColon());
    }

    private IEnumerator GameTimeLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(tickInterval);
            
            // �ð��� �Ͻ������� ���¶�� �ð��� �������� ����
            if(!_isTimePaused)
            {
                AdvanceTime();
            }
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

        // �Ϸ� ����
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

        string colon = showColon ? ":" : " ";
        string formattedTime = $"{displayHour}{colon}{gameMinute:00}";
        timeText.text = formattedTime;
        ampmText.text = period;

        dayText.text = $"Day {day}";
    }

    private IEnumerator BlinkColon()
    {
        while (true)
        {
            showColon = !showColon;
            UpdateTimeUI();
            yield return new WaitForSeconds(0.5f); // 0.5�� �������� ������
        }
    }

    private void EndDay()
    {
        Debug.Log($"[DayCycle] Day {day} ����, ���� �� �̵�!");
        SaveDay();
        SceneManager.LoadScene("SettlementScene"); // ���� ������ ��ȯ
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
        day = PlayerPrefs.GetInt("Day", 1); // �⺻ 1��������
    }

    public void AdvanceToNextDay()
    {
        day++;
        PlayerPrefs.SetInt("Day", day);
        PlayerPrefs.Save();

        // ������ �ʱ�ȭ
        gameHour = 20;
        gameMinute = 0;
        totalGameMinutesPassed = 0;
        UpdateTimeUI();

        // ���� �ڷ�ƾ�� �����ϰ� ���� ����
        if (_gameTimeCoroutine != null)
        {
            StopCoroutine(_gameTimeCoroutine);
        }
        _gameTimeCoroutine = StartCoroutine(GameTimeLoop());
    }

        // �ð� �帧 �Ͻ�����
    public void PauseTime()
    {
        _isTimePaused = true;
        Debug.Log("[DayCycleManager] �ð� �帧�� �Ͻ������Ǿ����ϴ�.");
    }

    // �ð� �帧 �簳
    public void ResumeTime()
    {
        _isTimePaused = false;
        Debug.Log("[DayCycleManager] �ð� �帧�� �簳�Ǿ����ϴ�.");
    }

    // ���� �ð� �Ͻ����� ���� Ȯ��
    public bool IsTimePaused => _isTimePaused;
}
