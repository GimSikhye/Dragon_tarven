using DalbitCafe.Operations;
using DalbitCafe.UI;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using System.Collections;
using DalbitCafe.Core;
using DalbitCafe.Deco;

public class GameSceneButton : MonoBehaviour
{
    [SerializeField] CoffeeMachineManager coffeeMachineManager;
    //    else if (scene.name == "GameScene")
    //    {
    //        Button[] gameSceneButtons = GameObject.Find("Canvas_GameScene").GetComponentsInChildren<Button>(true); // 비활성화된 버튼들도 가져옴 <>true
    //        foreach (Button button in gameSceneButtons)
    //        {
    //            if (button.name == "UI_QuestButton")
    //            {
    //                //button.onClick.AddListener(() => UIManager.Instance.ShowQuestPopUp()); // 퀘스트 팝업
    //            }
    //            if (button.name == "UI_StoargeBoxButton")
    //            {
    //                //button.onClick.AddListener(() => UIManager.Instance.OpenInventory());
    //            }
    //            //if (button.name == "UI_StoreButton")
    //            //{
    //            //    button.onClick.AddListener(() => UIManager.Instance.OpenStore());
    //            //}
    //            if (button.name == "UI_DecoRotateButton")
    //            {
    //                button.onClick.AddListener(() =>
    //                {
    //                    button.interactable = false;
    //                    DecorateManager decorateManager = FindAnyObjectByType<DecorateManager>();
    //                    decorateManager.OnRotateButtonPressed(); // targetItem 기반 회전
    //                    StartCoroutine(EnableButtonAfterDelay(button.gameObject, 0.5f));
    //                });
    //            }


    //        }
    //    }

    //}

    public void LoadButton(string sceneName)
    {
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
        int index = roastingWindow.coffeeMachineMenuContainers.IndexOf(menuContainer);

        if (index < 0 || index >= roastingWindow.coffeDataList.Count)
        {
            button.GetComponent<Button>().interactable = true;
            return;
        }

        CoffeeData coffeeData = roastingWindow.coffeDataList[index];

        if (PlayerStatsManager.Instance.statsSO.coffeeBean >= coffeeData.BeanUse)
        {
            PlayerStatsManager.Instance.AddCoffeeBean(-coffeeData.BeanUse);
            coffeeMachineManager.lastTouchedMachine.RoastCoffee(coffeeData);
        }

        StartCoroutine(EnableButtonAfterDelay(button, 3f));
    }


    private IEnumerator EnableButtonAfterDelay(GameObject button, float delay)
    {
        yield return new WaitForSeconds(delay);
        button.GetComponent<Button>().interactable = true;
    }
}
