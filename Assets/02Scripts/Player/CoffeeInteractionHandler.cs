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

    private void HandleCoffeeMachineInteraction(Vector3 worldPos) 
    {
        var coffeMachine = coffeMachineManager.GetCoffeeMachineAtWorldPosition(worldPos);

        if (coffeMachine == null) return;

        float distance = Vector3.Distance(transform.position, coffeMachine.transform.position);

        if (distance <= _interactionRange)
        {
            coffeMachineManager.SetLastTouchedMachine(coffeMachine);

            if (coffeMachine.IsRoasting)
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
