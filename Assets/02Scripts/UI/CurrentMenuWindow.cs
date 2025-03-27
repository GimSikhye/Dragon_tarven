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


    private void Update()
    {
        HideMenuOnOustideTouch();
    }

    void HideMenuOnOustideTouch()
    {
        // 터치가 1개 이상 있을 때만 처리
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0); // 첫 번째 터치만 처리

            // 터치 상태가 Began일 때만 반응
            if (touch.phase == TouchPhase.Began)
            {
                // UI 외부를 터치했을 때
                if (!UIManager.Instance.IsTouchOverUI(touch))
                {
                    if (currentMenuPanel != null)
                    {
                        Debug.Log("창 끔");
                        currentMenuPanel.SetActive(false);
                    }
                    else
                    {
                        Debug.LogError("currentMenuPanel이 null입니다!");
                    }
                }
            }
        }
    }


    public void UpdateMenuPanel(CoffeeMachine coffeeMachine) // 나중에 제너릭<T>로 토스터, 믹서기 등등 만들거임 //sell Coffee함수에서 호출, 처음 패널을 띄울때도 호출.
    {
        
        if (coffeeMachine != null && coffeeMachine.IsRoasting)
        {
            currentMenuPanel.SetActive(true);
            menuNameText.text = coffeeMachine.CurrentCoffee.CoffeName;
            menuIcon.sprite = coffeeMachine.CurrentCoffee.MenuIcon;
            remainingMugsText.text = $"{coffeeMachine.RemainingMugs}잔 남음";
            mugProgressBar.value = (float)coffeeMachine.RemainingMugs / coffeeMachine.CurrentCoffee.MugQty; // 스크롤바 value값 업데이트
        }
        else
        {
            currentMenuPanel.SetActive(false); 
        }
    }


}
