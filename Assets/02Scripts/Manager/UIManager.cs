using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

enum Windows
{
    MakeCoffee = 0,
    Exit,
    CurrentMenu
}
public class UIManager : MonoBehaviour
{
    //enum으로 윈도우 창 이름 배열 관리하기

    public static UIManager Instance;
    [SerializeField] private GameObject[] _panels;
    [SerializeField] private TextMeshProUGUI _captionText;

    [Header("재화량 텍스트")]
    [SerializeField] private TextMeshProUGUI _coffeeBeanText;
    [SerializeField] private TextMeshProUGUI _coinText;
    [SerializeField] private TextMeshProUGUI gemText;

    // 원두 개수 값이 바뀔때 갱신해주기.
    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        foreach (var panel in _panels)
            panel.SetActive(false);
    }
    // 모든 UI 비활성화
    public void UpdateCoffeeBeanUI(int value)
    {
        _coffeeBeanText.text = value.ToString();
    }

    public void UpdateCoinUI(int value)
    {
        _coinText.text = value.ToString();
    }

    public void UpdateGemUI(int value)
    {
        gemText.text = value.ToString();
    }

    public void ShowRoastingWindow()
    {
        _panels[(int)Windows.MakeCoffee].SetActive(true);
    }

    public void ShowExitWindow()
    {
        _panels[(int)Windows.Exit].SetActive(true);
    }

    public void ShowCapitonText()
    {
        Vector3 playerScreenPos = Camera.main.WorldToScreenPoint(PlayerCtrl.Instance.transform.position);

        _captionText.rectTransform.position = playerScreenPos;
        _captionText.enabled = true;
        _captionText.text = "거리가 너무 멀어요!";
    }

    public void ShowCurrentMenuWindow()
    {
        Debug.Log("현재 메뉴창 띄움");
        _panels[(int)Windows.CurrentMenu].SetActive(true); 

    }

    public void CloseWindow(string window)
    {
        GameObject windowPanel = GameObject.Find(window);
        windowPanel.SetActive(false);

    }



    // 터치 위치가 UI 위인지 판단함
    public bool IsTouchOverUI(Touch touch)
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = touch.position;

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);

        return results.Count > 0;
    }

}
