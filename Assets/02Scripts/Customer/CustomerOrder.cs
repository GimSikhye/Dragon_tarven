using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CustomerOrder : MonoBehaviour
{
    public enum MenuType { Americano, CafeLatte, Conpanna, Espresso, Latte }

    [SerializeField] private GameObject speechBubblePrefab;
    [SerializeField] private Sprite[] menuIcons;

    private GameObject speechBubble;
    private MenuType selectedMenu;

    private bool hasOrdered = false;
    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
    }

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

    private void Update()
    {
        if (!hasOrdered || Input.touchCount == 0)
            return;

        Touch touch = Input.GetTouch(0);
        if (touch.phase != TouchPhase.Began)
            return;

        Vector2 touchPos = mainCamera.ScreenToWorldPoint(touch.position);
        RaycastHit2D hit = Physics2D.Raycast(touchPos, Vector2.zero);

        if (hit.collider != null && hit.collider.transform == transform)
        {
            Debug.Log("손님 터치됨 (2D Raycast)!");
            LoadOrderScene();
        }
    }

    private void LoadOrderScene()
    {
        OrderData.CurrentMenu = selectedMenu;
        OrderData.CustomerName = gameObject.name; // ex: "Duck", "Fox", etc.
        SceneManager.LoadScene("OrderScene");
    }
}
