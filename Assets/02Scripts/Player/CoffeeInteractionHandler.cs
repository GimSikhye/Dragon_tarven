using UnityEngine;
using DalbitCafe.Operations;
using DalbitCafe.Inputs;

public class CoffeeInteractionHandler : MonoBehaviour
{
    [SerializeField] private float _interactionRange = 0.3f;
    [SerializeField] private UIManager uiManager;
    [SerializeField] CoffeeMachineManager coffeMachineManager;

    private void Start()
    {
        InputManager.OnTouchEnded += HandleCoffeeMachineInteraction;
    }

    private void OnDestroy()
    {
        InputManager.OnTouchEnded -= HandleCoffeeMachineInteraction;
    }

    private void HandleCoffeeMachineInteraction(Vector2 worldPos) 
    {
        CoffeeMachine coffeeMachine = coffeMachineManager.GetCoffeeMachineAtWorldPosition(worldPos, 0.5f);

        if (coffeeMachine == null)
        {
            Debug.Log("커피머신 null");
            return;
        }


        float distance = Vector2.Distance(transform.position, coffeeMachine.transform.position);

        if (distance <= _interactionRange)
        {
            coffeMachineManager.SetLastTouchedMachine(coffeeMachine);
            if (coffeeMachine.IsRoasting)
            {
                uiManager.ShowCurrentMenuPopUp();
            }
            else
            {
                uiManager.ShowMakeCoffeePopUp();
            }
        }
        else
        {
            Debug.Log("너무 멀어요");
        }
    }
}
