using DalbitCafe.Map;
using DalbitCafe.Operations;
using DalbitCafe.UI;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //public void HandleTouchBegan(Vector2 screenPos)
    //{
    //    if (!_canMoveControl) return;
    //}

    //public void HandleTouchMoved(Vector2 screenPos)
    //{
    //    if (!_canMoveControl) return;
    //}

    //public void HandleTouchEnded(Vector2 screenPos) // ��ġ�� �I�� �� ������
    //{
    //    _touchOnUI = UIManager.Instance.IsTouchOverUIPosition(screenPos); // ��ġ UI ǥ��?

    //    if (!_canMoveControl || _touchOnUI)
    //    {
    //        _touchOnUI = false;  // ����
    //        return;
    //    }

    //    Vector3 worldPos = Camera.main.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, Camera.main.nearClipPlane)); // camera.main.nearclipplane
    //    worldPos.z = 0;

    //    // Ŀ�Ǹӽ� ��ǥ ������� ã��
    //    var machine = CoffeeMachineManager.Instance.GetMachineAtPosition(worldPos); // GetMachingAtPosition ��ġ�� ���� �ӽ��� ������ �ӽ��� ������
    //    if (machine != null) // ��ġ�� ���� �ӽ��̶��
    //    {
    //        TouchCoffeeMachine(machine);
    //    }
    //    else if (FloorManager.Instance.IsFloor(worldPos))// ��ġ�� ���� �ٴ��̶��
    //    {
    //        OnMove(worldPos);
    //    }
    //}

    //private void TouchCoffeeMachine(CoffeeMachine machine) // ���� �˻�
    //{
    //    if (Vector3.Distance(transform.position, machine.transform.position) <= _interactionRange)
    //    {
    //        CoffeeMachine.SetLastTouchedMachine(machine); // �ش� �ӽ��� ���������� ��ġ�� �ӽ����� ����

    //        if (machine.IsRoasting)
    //        {
    //            UIManager.Instance.ShowCurrentMenuPopUp(); // ���� ����� �ִ� �޴� �˾�
    //            GameObject currentMenuWindow = GameObject.Find("Panel_CurrentMenu");
    //            currentMenuWindow.GetComponent<CurrentMenuWindow>().UpdateMenuPanel(machine);
    //        }
    //        else
    //        {
    //            UIManager.Instance.ShowMakeCoffeePopUp();
    //        }
    //    }
    //    else
    //    {
    //        UIManager.Instance.ShowCapitonText(); // �ʹ� �־��
    //    }
    //}
}
