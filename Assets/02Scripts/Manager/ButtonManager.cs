using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
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


    public void LoadButton(string sceneName)
    {
        SoundManager.Instance.PlaySFX(click_clip, 0.6f);
        SceneManager.LoadScene(sceneName);
    }

    public void CloseWindowButton(string windowName)
    {
        GameObject window = GameObject.Find(windowName);
        if (window == null)
        {
            Debug.LogError("비활성화 활 윈도우를 찾지 못했습니다!");
            return;
        }

        Debug.Log($"비활성화할 패널: {window.name}");
        window.SetActive(false);
    }


    public void IngredientButton()
    {
        GameObject ingredientButton = GameObject.Find("ingredient_btn");
        GameObject makeButton = GameObject.Find("make_btn");
        
        ingredientButton.SetActive(false);
    }

    public void MakeDrinkButton()
    {
        Debug.Log("만들기 시작");

        // 만들기
    }



    public void QuitButton()
    {
        SoundManager.Instance.PlaySFX(click_clip, 0.6f);
        Application.Quit(); 
    }
    



    
    
}
