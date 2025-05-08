using UnityEngine;
using UnityEngine.UI;

public class TabsManager : MonoBehaviour
{
    public GameObject[] Tabs;
    public Image[] TabButtons;
    public Sprite InactiveTabBG, ActiveTabBG;
    public Vector2 InactiveTabButtionSize, ActiveTabButtonSize;

    public void SwitchToTab(int TabID)
    {
        foreach(GameObject go in Tabs)
        {
            go.SetActive(false);
        }
        Tabs[TabID].SetActive(true);

        foreach(Image im in TabButtons)
        {
            im.sprite = InactiveTabBG;
            im.rectTransform.sizeDelta = InactiveTabButtionSize;
        }
        TabButtons[TabID].sprite = ActiveTabBG;
        TabButtons[TabID].rectTransform.sizeDelta = ActiveTabButtonSize;
                
    }

}
