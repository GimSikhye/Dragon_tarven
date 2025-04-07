using UnityEngine;
using UnityEngine.UI;

public class ResetPrefsBtn : MonoBehaviour
{
    public Button resetButton;

    void Start()
    {
        resetButton.onClick.AddListener(() =>
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();
            Debug.Log("PlayerPrefs 초기화 완료");
        });
    }
}
