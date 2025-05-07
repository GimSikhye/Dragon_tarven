using DalbitCafe.Operations;
using DalbitCafe.UI;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using System.Collections;
using DalbitCafe.Core;


public class ButtonManager : MonoSingleton<ButtonManager>
{
    [SerializeField] private AudioClip click_clip;

    private void OnEnable()
    {
        SceneManager.sceneLoaded += ButtonInit;
    }
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= ButtonInit;
    }

    private void ButtonInit(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "MainMenu")
        {
            // MainMenu에 있는 버튼을 찾아서 람다로 이벤트 추가
            Button[] mainMenuButtons = GameObject.Find("Canvas_MainMenu").GetComponentsInChildren<Button>();
            foreach (Button button in mainMenuButtons)
            {
                if (button.name == "UI_StartBtn")
                {
                    button.onClick.AddListener(() => LoadButton("GameScene"));
                }
                else if (button.name == "UI_QutiBtn")
                {
                    button.onClick.AddListener(() => QuitButton());
                }
            }

        }
        else if (scene.name == "GameScene")
        {
            Button[] gameSceneButtons = GameObject.Find("Canvas_GameScene").GetComponentsInChildren<Button>(true); // 비활성화된 버튼들도 가져옴
            foreach (Button button in gameSceneButtons)
            {
                if (button.name == "UI_QuestBtn")
                {
                    button.onClick.AddListener(() => UIManager.Instance.ShowQuestPopUp());
                }
                if(button.name == "UI_StoargeBoxBtn")
                {
                    button.onClick.AddListener(() => UIManager.Instance.OpenInventory());
                }
                if (button.name == "UI_StoreBtn")
                {
                    button.onClick.AddListener(() => UIManager.Instance.OpenStore());
                }
                if (button.name == "UI_DecoRotateBtn")
                {
                    button.onClick.AddListener(() => { button.interactable = false; Debug.Log("회전");  StartCoroutine(EnableButtonAfterDelay(button.gameObject, 1f)); }); 
 
                }

            }
        }

    }

    public void LoadButton(string sceneName)
    {
        SoundManager.Instance.PlaySFX(click_clip);
        GameObject currentButton = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;

        if (currentButton != null)
        {
            if (!PlayerPrefs.HasKey("HasSeenPrologue"))
            {
                currentButton.transform.DOScale(1.2f, 0.2f).SetLoops(2, LoopType.Yoyo).OnComplete(() =>
                {
                    PlayerPrefs.SetString("NextDialogue", "Prologue");
                    PlayerPrefs.SetInt("HasSeenPrologue", 1);
                    SceneManager.LoadScene("DialogueScene");
                });
            }
            else
            {
                currentButton.transform.DOScale(1.2f, 0.2f).SetLoops(2, LoopType.Yoyo).OnComplete(() =>
                {
                    SceneManager.LoadScene(sceneName);
                });
            }
        }
    }

    public void CloseWindowButton(string windowName)
    {
        GameObject window = GameObject.Find(windowName);
        window.transform.DOScale(Vector3.zero, 0.3f).SetEase(Ease.InBack).OnComplete(() => window.SetActive(false));
    }
    //make_btn
    public void RoastingButton(GameObject button)
    {
        button.GetComponent<Button>().interactable = false;
        GameObject menuContainer = button.transform.parent?.gameObject;
        RoastingWindow roastingWindow = FindObjectOfType<RoastingWindow>();
        int index = roastingWindow.menuContainers.IndexOf(menuContainer);

        if (index < 0 || index >= roastingWindow.coffeDataList.Count)
        {
            button.GetComponent<Button>().interactable = true;
            return;
        }

        CoffeeData coffeeData = roastingWindow.coffeDataList[index];

        if (PlayerStatsManager.Instance.statsSO.coffeeBean >= coffeeData.BeanUse)
        {
            PlayerStatsManager.Instance.AddCoffeeBean(-coffeeData.BeanUse);
            CoffeeMachine.LastTouchedMachine.RoastCoffee(coffeeData);
        }

        StartCoroutine(EnableButtonAfterDelay(button, 3f));
    }

    public void QuitButton()
    {
        SoundManager.Instance.PlaySFX(click_clip);
        Application.Quit();
    }

    private IEnumerator EnableButtonAfterDelay(GameObject button, float delay)
    {
        yield return new WaitForSeconds(delay);
        button.GetComponent<Button>().interactable = true;
    }
}
