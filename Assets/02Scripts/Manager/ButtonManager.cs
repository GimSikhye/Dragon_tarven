using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class ButtonManager : MonoBehaviour
{
    public static ButtonManager Instance;
    [SerializeField] private AudioClip click_clip;
   
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);

    }


    public void ClickLoadButton(string sceneName)
    {
        SoundManager.Instance.PlaySFX(click_clip, 0.6f);
        SceneManager.LoadScene(sceneName);
    }

    public void ClickQuitButton()
    {
        SoundManager.Instance.PlaySFX(click_clip, 0.6f);
        Application.Quit(); 
    }
    



    
    
}
