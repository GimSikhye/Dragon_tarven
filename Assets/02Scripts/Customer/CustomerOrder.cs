using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class CustomerOrder : MonoBehaviour
{
    public enum MenuType { Americano, CafeLatte, Conpanna, Espresso, Latte }
    private MenuType selectedMenu;
    private bool hasOrdered = false;

    [Header("메뉴 UI 컴포넌트 할당")]
    [SerializeField] private GameObject speechBubblePrefab; // 메뉴 말풍선
    [SerializeField] private Sprite[] menuIcons; // 메뉴 아이콘

    private GameObject speechBubble;
    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
    }

    public void StartOrderingAfterDelay(float delayTime) // 지연 후 주문 시작
    {
        if (!hasOrdered)
            StartCoroutine(DelayedOrder(delayTime));
    }

    private IEnumerator DelayedOrder(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        CreateOrderBubble();
    }

    private void CreateOrderBubble()
    {
        selectedMenu = (MenuType)Random.Range(0, Enum.GetValues(typeof(MenuType)).Length); // 선택된 메뉴

        // 말풍선 생성+ 손님 위에 띄우기
        speechBubble = Instantiate(speechBubblePrefab, transform);
        speechBubble.transform.localPosition = new Vector3(0, 1f, 0);

        Transform menuIconTransform = speechBubble.transform.Find("SpeechBalloon/menuIcon"); // SpeechBalloon 자식

        Image menuIcon = menuIconTransform.GetComponent<Image>();

        menuIcon.sprite = menuIcons[(int)selectedMenu];
        hasOrdered = true;
    }

    private void Update()
    {
        if (!hasOrdered || Input.touchCount == 0)
            return;

        Touch touch = Input.GetTouch(0); // 터치 감지
        if (touch.phase != TouchPhase.Began)
            return;

        Vector2 touchPosition = mainCamera.ScreenToWorldPoint(touch.position);
        RaycastHit2D hit = Physics2D.Raycast(touchPosition, Vector2.zero); // 방향 zero

        if (hit.collider != null && hit.collider.transform == transform) // 손님 위치라면
        {
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
