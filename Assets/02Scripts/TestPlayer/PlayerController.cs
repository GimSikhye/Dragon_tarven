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

    //public void HandleTouchEnded(Vector2 screenPos) // 터치를 똈을 때 움직임
    //{
    //    _touchOnUI = UIManager.Instance.IsTouchOverUIPosition(screenPos); // 터치 UI 표시?

    //    if (!_canMoveControl || _touchOnUI)
    //    {
    //        _touchOnUI = false;  // 리셋
    //        return;
    //    }

    //    Vector3 worldPos = Camera.main.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, Camera.main.nearClipPlane)); // camera.main.nearclipplane
    //    worldPos.z = 0;

    //    // 커피머신 좌표 기반으로 찾기
    //    var machine = CoffeeMachineManager.Instance.GetMachineAtPosition(worldPos); // GetMachingAtPosition 터치한 곳에 머신이 있으면 머신을 가져옴
    //    if (machine != null) // 터치한 곳이 머신이라면
    //    {
    //        TouchCoffeeMachine(machine);
    //    }
    //    else if (FloorManager.Instance.IsFloor(worldPos))// 터치한 곳이 바닥이라면
    //    {
    //        OnMove(worldPos);
    //    }
    //}

    //private void TouchCoffeeMachine(CoffeeMachine machine) // 여기 검사
    //{
    //    if (Vector3.Distance(transform.position, machine.transform.position) <= _interactionRange)
    //    {
    //        CoffeeMachine.SetLastTouchedMachine(machine); // 해당 머신을 마지막으로 터치한 머신으로 설정

    //        if (machine.IsRoasting)
    //        {
    //            UIManager.Instance.ShowCurrentMenuPopUp(); // 현재 만들고 있는 메뉴 팝업
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
    //        UIManager.Instance.ShowCapitonText(); // 너무 멀어요
    //    }
    //}
}
