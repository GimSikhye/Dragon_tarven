using UnityEngine.SceneManagement;
using UnityEngine;

public class SettlementUIManager : MonoBehaviour
{
    public void OnNextDayButtonPressed()
    {
        // Ÿ��Ʋ�� �Ǵ� ���Ӿ� �ٽ� �ε�
        SceneManager.LoadScene("GameScene");
    }
}
