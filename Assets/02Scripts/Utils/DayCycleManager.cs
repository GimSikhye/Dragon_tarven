using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
public class DayCycleManager : MonoBehaviour
{
    [Header("UI 연결")]
    [SerializeField] private TMP_Text timeText;
    [SerializeField] private TMP_Text ampmText; // 오전오후
    [SerializeField] private TMP_Text dayText;
    
    private int day = 1;
    private int gameHour = 20; 
    private int gameMinute = 0; // 시간 바꿔주는 용도(60분-> 1시간)
    private float tickInterval = 5f; // 5초마다 시간 흐름
    private float elapsed = 0f;

    private bool showColon = true; // :
    private const int minutesPerTick = 10; // 분당 틱
    //private const int minutesPerDay = 300; // 10시간 = 300분
    private const int minutesPerDay = 50; // 임시


    private int totalGameMinutesPassed = 0;

    // 시간 흐름 제어용 변수
    private bool _isTimePaused = false;
    private Coroutine _gameTimeCoroutine;
    private Coroutine _blinkCoroutine;
    private void Awake()
    {
        if (FindObjectsOfType<DayCycleManager>().Length > 1)
        {
            Destroy(gameObject); // 중복 방지
            return;
        }
        DontDestroyOnLoad(gameObject);
    }
    private void Start()
    {
        LoadDay();
        UpdateTimeUI();
        _gameTimeCoroutine = StartCoroutine(GameTimeLoop());
        _blinkCoroutine = StartCoroutine(BlinkColon());
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "GameScene")
        {
            timeText = GameObject.Find("TimeText").GetComponent<TextMeshProUGUI>();
            ampmText = GameObject.Find("AmPmText").GetComponent<TextMeshProUGUI>();
            dayText = GameObject.Find("DayText").GetComponent<TextMeshProUGUI>();
            UpdateTimeUI(); // UI 즉시 갱신

            ResumeTime();
        }
    }
    private IEnumerator GameTimeLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(tickInterval);
            
            // 시간이 일시정지된 상태라면 시간을 진행하지 않음
            if(!_isTimePaused)
            {
                AdvanceTime(); // 시간 전진
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
            yield return new WaitForSeconds(0.5f); // 0.5초 간격으로 깜빡임
        }
    }

    private void EndDay()
    {
        Debug.Log($"[DayCycle] Day {day} 종료, 정산 씬 이동!");
        SaveDay();
        PauseTime();
        SceneManager.LoadScene("SettlementScene"); // 정산 씬으로 전환
    }

    private void SaveDay()
    {
        PlayerPrefs.SetInt("Day", day);
        PlayerPrefs.SetInt("Coin", PlayerStatsManager.Instance.Coin);
        PlayerPrefs.SetInt("CoffeeBean", PlayerStatsManager.Instance.CoffeeBeans);
        PlayerPrefs.Save();
    }
    public void LoadDay()
    {
        day = PlayerPrefs.GetInt("Day", 1); // 기본 1일차부터
    }

    
    public void AdvanceToNextDay() // 다음날로
    {
        ++day;
        PlayerPrefs.SetInt("Day", day);
        PlayerPrefs.Save();

        // 다음날 초기화
        gameHour = 20;
        gameMinute = 0;
        totalGameMinutesPassed = 0;
        UpdateTimeUI();

        // 기존 코루틴을 정지하고 새로 시작
        if (_gameTimeCoroutine != null)
        {
            StopCoroutine(_gameTimeCoroutine);
        }
        _gameTimeCoroutine = StartCoroutine(GameTimeLoop());
    }

    // 시간 흐름 일시정지
    public void PauseTime()
    {
        _isTimePaused = true;
        Debug.Log("[DayCycleManager] 시간 흐름이 일시정지되었습니다.");
    }

    // 시간 흐름 재개
    public void ResumeTime()
    {
        _isTimePaused = false;
        Debug.Log("[DayCycleManager] 시간 흐름이 재개되었습니다.");
    }

    // 현재 시간 일시정지 상태 확인
    public bool IsTimePaused => _isTimePaused;
}
