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

        Debug.Log($"비활성화할 패널: {window.name}");
        window.SetActive(false);
    }


    public void IngredientButton()
    {
        // 만들기 버튼 띄우고 Ingrdient Button 비활성화하기.

        GameObject ingredientButton = GameObject.Find("ingredient_btn");
        GameObject makeButton = GameObject.Find("make_btn");
        GameObject beanUseTmp = GameObject.Find("beanUse_tmp");

        ingredientButton.SetActive(false);
        beanUseTmp.SetActive(false);
        makeButton.SetActive(true);
    }


    public void MakeDrinkButton(GameObject button)
    {

        // 1. 현재 버튼이 속한 Menu Container 찾기
        GameObject menuContainer = button.transform.parent?.gameObject;
        if (menuContainer == null)
        {
            Debug.LogError("Menu Container를 찾을 수 없습니다! (버튼의 부모 오브젝트가 없음)");
            return;
        }
        Debug.Log($"찾은 Menu Container: {menuContainer.name}");

        // 2. DrinkWindow 오브젝트 찾기
        DrinkWindow drinkWindow = FindObjectOfType<DrinkWindow>();
        int index = drinkWindow.menuContainers.IndexOf(menuContainer);
        if (index < 0)
        {
            Debug.LogError("해당 Menu Container가 DrinkWindow.menuContainers 리스트에 없습니다!");
            return;
        }

        // 해당 인덱스를 이용하여 coffeDataList에서 CoffeeData를 가져오기
        CoffeeData coffeeData = drinkWindow.coffeDataList[index];
        if (coffeeData == null)
        {
            Debug.LogError("coffeDataList[" + index + "]가 null입니다!");
            return;
        }

        int beanUseAmount = coffeeData.BeanUse; // 커피를 만들 때 사용할 원두 소모량

        // 5. GameManager.Instance의 커피콩 수량에서 beanUseAmount만큼 차감
        if (GameManager.Instance.CoffeeBean >= beanUseAmount)
        {
            GameManager.Instance.CoffeeBean -= beanUseAmount;
            Debug.Log($"{coffeeData.CoffeName} 커피를 만듭니다. 남은 커피콩: {GameManager.Instance.CoffeeBean}");
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
