using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CurrentMenuWindow : MonoBehaviour
{
    public GameObject currentMenuPanel; 
    public TextMeshProUGUI menuNameText;
    public Image menuIcon;     
    public Slider mugProgressBar; 
    public TextMeshProUGUI remainingMugsText;  

    public void UpdateMenuPanel(CoffeeMachine coffeeMachine) // 나중에 제너릭<T>로 토스터, 믹서기 등등 만들거임 //sell Coffee함수에서 호출, 처음 패널을 띄울때도 호출.
    {
        if (coffeeMachine != null && coffeeMachine.IsRoasting)
        {
            currentMenuPanel.SetActive(true);
            menuNameText.text = coffeeMachine.CurrentCoffee.CoffeName;
            menuIcon.sprite = coffeeMachine.CurrentCoffee.MenuIcon;
            remainingMugsText.text = $"{coffeeMachine.RemainingMugs}잔 남음";
            mugProgressBar.value = (float)coffeeMachine.RemainingMugs / coffeeMachine.CurrentCoffee.MugQty;
        }
        else
        {
            currentMenuPanel.SetActive(false);
        }
    }
}
