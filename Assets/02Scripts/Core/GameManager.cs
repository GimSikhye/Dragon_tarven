using DalbitCafe.Core;
using DalbitCafe.UI;
using UnityEngine.SceneManagement;
using UnityEngine;
using DalbitCafe.Deco;
using DalbitCafe.Inputs;
using DalbitCafe.Operations;
using DalbitCafe.Map;

public class GameManager : MonoSingleton<GameManager>
{
    [SerializeField] private UIManager _uiManager;
    [SerializeField] private ButtonManager _buttonManager;
    [SerializeField] private SoundManager _soundManager;
    [SerializeField] private PlayerStatsManager _playerStatsManager;
    [SerializeField] private TouchInputManager _touchInputManager;
    [SerializeField] private DialogueManager _dialogueManager;
    [SerializeField] private QuestManager _questManager;
    [SerializeField] private RewardManager _rewardManager;
    [SerializeField] private GridManager _gridManager;
    [SerializeField] private DecorateManager _decorateManager;
    [SerializeField] private CoffeeMachineManager _coffeeMachineManager;
    [SerializeField] private FloorManager _floorManager;

    private void Start()
    {
        _playerStatsManager.Load();
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

    // 외부에서 접근 가능하도록 프로퍼티 제공
    public UIManager UIManager => _uiManager;
    public SoundManager SoundManager => _soundManager;
    public ButtonManager ButtonManager => _buttonManager;
    public PlayerStatsManager PlayerStatsManager => _playerStatsManager;
    public TouchInputManager TouchInputManager => _touchInputManager;
    public DialogueManager DialogueManager => _dialogueManager;
    public QuestManager QuestManager => _questManager;
    public RewardManager RewardManager => _rewardManager;
    public GridManager GridManager => _gridManager;
    public DecorateManager DecorateManager => _decorateManager;
    public CoffeeMachineManager CoffeeMachineManager => _coffeeMachineManager;
    public FloorManager FloorManager => _floorManager;
}
