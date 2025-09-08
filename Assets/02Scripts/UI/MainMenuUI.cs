using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private Button startButton;
    [SerializeField] private Button quitButton;

    private void Awake()
    {
        startButton.onClick.AddListener(() => SceneManager.LoadScene("GameScene"));
        quitButton.onClick.AddListener(() => Application.Quit());
    }

}
