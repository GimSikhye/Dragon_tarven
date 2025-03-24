using UnityEngine;

public class CoffeeMachine : MonoBehaviour
{
    public static CoffeeMachine LastTouchedMachine { get; private set; } // 마지막 터치한 커피머신 저장

    [Header("커피머신 상태 변수")]
    [SerializeField] private CoffeeData currentCoffee; // 현재 로스팅 중인 커피
    [SerializeField] private int remainingMugs; // 남은 커피 잔 수
    [SerializeField] private bool isRoasting = false;

    public void RoastCoffee(CoffeeData coffee)
    {
        isRoasting = true;
        currentCoffee = coffee;
        remainingMugs = coffee.MugQty;
        Debug.Log($"{coffee.CoffeName} 커피를 로스팅 시작! 잔 수: {remainingMugs}");

    }

    // 현재 로스팅 중일 때, isRosating = true;
    // 팝업창 띄우지 말고, 남은 잔수 띄우는 팝업 띄우기.
    public void SellCoffee() // 손님한테 팔면.
    {
        // 현재 남은 잔수 표기
        if (remainingMugs > 0)
        {//currentCoffee.sprite (파티클로 날아가게?) (instantiate)
            remainingMugs--;
            GameManager.Instance.Coin += currentCoffee.Price;
            Debug.Log($"{currentCoffee.CoffeName} 판매! 남은 잔 수: {remainingMugs}");
        }
        else
        {
            isRoasting = false;
            Debug.Log("더 이상 판매할 커피가 없습니다!");
        }
    }

    public static void SetLastTouchedMachine(CoffeeMachine machine)
    {
        LastTouchedMachine = machine;
        Debug.Log(LastTouchedMachine.gameObject.name);
    }


}
