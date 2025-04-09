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
        });

    }
}
