using DalbitCafe.Core;
using DalbitCafe.UI;
using UnityEngine.SceneManagement;
using UnityEngine;

public class GameManager : MonoSingleton<GameManager>
{
    [SerializeField] private UIManager _uiManager;
    [SerializeField] private SoundManager _soundManager;
    [SerializeField] private PlayerStatsManager _playerStatsManager;
    [SerializeField] private RewardManager _rewardManager;
    [SerializeField] private ButtonManager _buttonManager;

    [SerializeField] private AudioClip[] _bgmClips;

    private void Start()
    {
        _playerStatsManager.Load();
        UpdateAllUI();
    }

    private void Update()
    {
#if UNITY_ANDROID
        if (Input.GetKey(KeyCode.Escape))
        {
            _uiManager.ShowExitPopUp();
        }
#endif
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
        _soundManager.PlaySceneBGM(scene);
    }


    private void UpdateAllUI()
    {
        var stats = _playerStatsManager;

        _uiManager.UpdateCoffeeBeanUI(stats.CoffeeBeans);
        _uiManager.UpdateCoinUI(stats.Coin);
        _uiManager.UpdateGemUI(stats.Gem);
        _uiManager.UpdateExpUI(stats.Exp, stats.MaxExp, stats.Level);
    }

    // 외부에서 접근 가능하도록 프로퍼티 제공
    public UIManager UIManager => _uiManager;
    public SoundManager SoundManager => _soundManager;
    public PlayerStatsManager PlayerStatsManager => _playerStatsManager;
    public RewardManager RewardManager => _rewardManager;
    public ButtonManager ButtonManager => _buttonManager;
}
