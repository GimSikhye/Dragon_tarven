using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CustomerOrder : MonoBehaviour
{
    public enum MenuType { Americano, CafeLatte, Conpanna, Espresso, Latte }

    [SerializeField] private GameObject speechBubblePrefab;
    [SerializeField] private Sprite[] menuIcons;

    private GameObject speechBubble;
    private MenuType selectedMenu;

    private bool hasOrdered = false;

    public void StartOrderingAfterDelay(float delay)
    {
        if (!hasOrdered)
            StartCoroutine(DelayedOrder(delay));
    }

    private IEnumerator DelayedOrder(float delay)
    {
        yield return new WaitForSeconds(delay);
        ShowOrder();
    }

    private void ShowOrder()
    {
        selectedMenu = (MenuType)UnityEngine.Random.Range(0, Enum.GetValues(typeof(MenuType)).Length);

        speechBubble = Instantiate(speechBubblePrefab, transform);
        speechBubble.transform.localPosition = new Vector3(0, 1.5f, 0);

        // 자식 중 이름이 "menuIcon"인 오브젝트 찾기
        Transform iconTransform = speechBubble.transform.Find("SpeechBalloon/menuIcon");
        if (iconTransform == null)
        {
            Debug.LogError("menuIcon 오브젝트를 찾을 수 없습니다!");
            return;
        }

        Image iconImage = iconTransform.GetComponent<Image>();
        if (iconImage == null)
        {
            Debug.LogError("menuIcon에 Image 컴포넌트가 없습니다!");
            return;
        }

        iconImage.sprite = menuIcons[(int)selectedMenu];
        hasOrdered = true;
    }

    private void OnMouseDown()
    {
        if (!hasOrdered) return;

        OrderData.CurrentMenu = selectedMenu;
        UnityEngine.SceneManagement.SceneManager.LoadScene("OrderScene");
    }
}
