using UnityEngine;

public class ButtonHandler : MonoBehaviour
{
    public void ActivePanel(string _panelName)
    {
        MainMenuHandler.Instance.panelName = _panelName;
    }
}
