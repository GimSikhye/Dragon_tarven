using UnityEngine;
using DalbitCafe.Operations;
using DalbitCafe.Inputs;

public class CoffeeInteractionHandler : MonoBehaviour
{
    [SerializeField] private float _interactionRange = 0.3f;

    private void Start()
    {
        InputManager.OnTouchEnded += HandleTouch;
    }

    private void OnDestroy()
    {
        InputManager.OnTouchEnded -= HandleTouch;
    }

    private void HandleTouch(Vector3 worldPos)
    {
        var machine = CoffeeMachineManager.Instance.GetMachineAtPosition(worldPos);

        if (machine == null) return;

        float distance = Vector3.Distance(transform.position, machine.transform.position);

        if (distance <= _interactionRange)
        {
            CoffeeMachine.SetLastTouchedMachine(machine);

            if (machine.IsRoasting)
            {
                Debug.Log("커피가 로스팅 중입니다");
                //UIManager.Instance.ShowCurrentMenuPopUp();
            }
            else
            {
                Debug.Log("커피 만들기 윈도우");
                //UIManager.Instance.ShowMakeCoffeePopUp();
            }
        }
        else
        {
            Debug.Log("너무 멀어요");
            //UIManager.Instance.ShowCaptionText();
        }
    }
}
