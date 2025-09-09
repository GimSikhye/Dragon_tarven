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
        // DayCycleManager 찾아서 AdvanceToNextDay 실행
        var dayCycleManager = FindAnyObjectByType<DayCycleManager>();
        if (dayCycleManager != null)
        {
            dayCycleManager.AdvanceToNextDay();
        }

        SceneManager.LoadScene("GameScene");
        PlayerPrefs.Save();
    }


}
