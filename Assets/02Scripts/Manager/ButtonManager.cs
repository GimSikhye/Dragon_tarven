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
            SceneManager.sceneLoaded += OnSceneLoaded; // 씬 로드 시 호출

        }
        else
        {
            Destroy(Instance.gameObject);
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"씬 로드됨: {scene.name}");
        // 씬이 바뀔 때 GameManager가 다시 찾아지도록 설정
        if (GameManager.Instance == null)
        {
            Debug.LogError("GameManager 인스턴스를 찾을 수 없음!");
        }
    }

    public void LoadButton(string sceneName)
    {
        SoundManager.Instance.PlaySFX(click_clip, 0.6f);
        Debug.Log("씬 이동");
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
        window.SetActive(false);
    }


    public void RoastingButton(GameObject button)
    {
        // 현재 버튼이 속한 Menu Container 찾기
        GameObject menuContainer = button.transform.parent?.gameObject;

        RoastingWindow roastingWindow = FindObjectOfType<RoastingWindow>();
        int index = roastingWindow.menuContainers.IndexOf(menuContainer);

        if (index < 0 || index >= roastingWindow.coffeDataList.Count)
        { 
            Debug.LogError("유효하지 않은 커피 메뉴 선택!");
            return;
        }

        // 해당 인덱스를 이용하여 coffeDataList에서 CoffeeData를 가져오기
        CoffeeData coffeeData = roastingWindow.coffeDataList[index];

        // 원두 소모 체크
        if (GameManager.Instance.CoffeeBean >= coffeeData.BeanUse)
        {
            GameManager.Instance.CoffeeBean -= coffeeData.BeanUse;
            CoffeeMachine.LastTouchedMachine.RoastCoffee(coffeeData);
            Debug.Log($"{coffeeData.CoffeName} 로스팅 시작!");
        }
        else
        {
            Debug.LogError("커피콩이 부족합니다!");
        }
    }


    public void QuitButton()
    {
        SoundManager.Instance.PlaySFX(click_clip, 0.6f);
        Application.Quit();
    }






}
