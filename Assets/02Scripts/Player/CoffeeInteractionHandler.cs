using UnityEngine;
using DalbitCafe.Operations;
using DalbitCafe.Inputs;

public class CoffeeInteractionHandler : MonoBehaviour
{
    [SerializeField] private float _interactionRange = 0.3f;

    private void Start()
    {
        InputManager.OnTouchEnded += HandleCoffeeMachineInteraction;
    }

    private void OnDestroy()
    {
        InputManager.OnTouchEnded -= HandleCoffeeMachineInteraction;
    }

    private void HandleCoffeeMachineInteraction(Vector3 worldPos) //
    {
        var coffeMachine = CoffeeMachineManager.Instance.GetCoffeeMachineAtWorldPosition(worldPos);

        if (coffeMachine == null) return;

        float distance = Vector3.Distance(transform.position, coffeMachine.transform.position);

        if (distance <= _interactionRange)
        {
            CoffeeMachine.SetLastTouchedMachine(coffeMachine);

            if (coffeMachine.IsRoasting)
            {
                Debug.Log("커피가 로스팅 중입니다");
                UIManager.Instance.ShowCurrentMenuPopUp();
            }
            else
            {
                Debug.Log("커피 만들기 윈도우");
                UIManager.Instance.ShowMakeCoffeePopUp();
            }
        }
        else
        {
            Debug.Log("너무 멀어요");
            //UIManager.Instance.ShowCaptionText();
        }
    }
}
