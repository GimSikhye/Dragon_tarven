using UnityEngine.SceneManagement;
using UnityEngine;

public class SettlementUIManager : MonoBehaviour
{
    public void OnNextDayButtonPressed()
    {
        // 타이틀씬 또는 게임씬 다시 로드
        SceneManager.LoadScene("GameScene");
    }
}
