using DalbitCafe.Core;
using DalbitCafe.Player;
using DG.Tweening;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DalbitCafe.UI
{
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

        private int _currentCoffeeBean;
        private int _currentCoin;
        private int _currentGem;

        // 원두 개수 값이 바뀔때 갱신해주기.
        private void Awake()
        {
            if (Instance == null)
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

            // 게임 시작 시 UI 갱신 (GameManager가 먼저 실행되므로 안전)
            var stats = GameManager.Instance.playerStats;
            UpdateCoffeeBeanUI(stats.coffeeBean);
            UpdateCoinUI(stats.coin);
            UpdateGemUI(stats.gem);

        }
        // 모든 UI 비활성화
        public void UpdateCoffeeBeanUI(int value)
        {
            TextAnimationHelper.AnimateNumber(_coffeeBeanText, _currentCoffeeBean, value);
            _currentCoffeeBean = value;
        }

        public void UpdateCoinUI(int value)
        {
            TextAnimationHelper.AnimateNumber(_coinText, _currentCoin, value, 1.5f);
            _currentCoin = value;
        }

        public void UpdateGemUI(int value)
        {
            TextAnimationHelper.AnimateNumber(gemText, _currentGem, value);
            _currentGem = value;
        }

        public void ShowMakeCoffeePopUp()
        {

            _panels[(int)Windows.MakeCoffee].SetActive(true);
            _panels[(int)Windows.MakeCoffee].transform.localScale = Vector3.zero;
            _panels[(int)Windows.MakeCoffee].transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);

        }

        public void ShowExitPopUp()
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

        public void ShowCurrentMenuPopUp()
        {
            Debug.Log("현재 메뉴창 띄움");
            GameObject window = _panels[(int)Windows.CurrentMenu];
            window.SetActive(true);
            window.GetComponent<Image>().color = new Color32(255, 255, 255, 0);
            int count = window.transform.childCount;

            for(int i = 0; i< count; i++)
            {
                //window.transform.GetChild(i).GetComponent<Image>().color = new Color32(255, 255, 255, 0);
            }
            _panels[(int)Windows.CurrentMenu].GetComponent<Image>().DOFade(1, 0.5f);

        }

        public void ShowExitPopUp(string window)
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

}

