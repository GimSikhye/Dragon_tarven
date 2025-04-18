using DalbitCafe.Operations;
using DalbitCafe.UI;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using System.Collections;

public class ButtonManager : MonoBehaviour
{
    [SerializeField] private AudioClip click_clip;

    public void LoadButton(string sceneName)
    {
        GameManager.Instance.SoundManager.PlaySFX(click_clip);
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

        if (GameManager.Instance.PlayerStatsManager.statsSO.coffeeBean >= coffeeData.BeanUse)
        {
            GameManager.Instance.PlayerStatsManager.AddCoffeeBean(-coffeeData.BeanUse);
            CoffeeMachine.LastTouchedMachine.RoastCoffee(coffeeData);
        }

        StartCoroutine(EnableButtonAfterDelay(button, 3f));
    }

    public void QuitButton()
    {
        GameManager.Instance.SoundManager.PlaySFX(click_clip);
        Application.Quit();
    }

    private IEnumerator EnableButtonAfterDelay(GameObject button, float delay)
    {
        yield return new WaitForSeconds(delay);
        button.GetComponent<Button>().interactable = true;
    }


}
