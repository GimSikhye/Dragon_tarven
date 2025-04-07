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
        //enum���� ������ â �̸� �迭 �����ϱ�

        public static UIManager Instance;
        [SerializeField] private GameObject[] _panels;
        [SerializeField] private TextMeshProUGUI _captionText;

        [Header("��ȭ�� �ؽ�Ʈ")]
        [SerializeField] private TextMeshProUGUI _coffeeBeanText;
        [SerializeField] private TextMeshProUGUI _coinText;
        [SerializeField] private TextMeshProUGUI gemText;

        private int _currentCoffeeBean;
        private int _currentCoin;
        private int _currentGem;

        // ���� ���� ���� �ٲ� �������ֱ�.
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

            // ���� ���� �� UI ���� (GameManager�� ���� ����ǹǷ� ����)
            var stats = GameManager.Instance.playerStats;
            UpdateCoffeeBeanUI(stats.coffeeBean);
            UpdateCoinUI(stats.coin);
            UpdateGemUI(stats.gem);

        }
        // ��� UI ��Ȱ��ȭ
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
            _captionText.text = "�Ÿ��� �ʹ� �־��!";
        }

        public void ShowCurrentMenuPopUp()
        {
            Debug.Log("���� �޴�â ���");
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

        // ��ġ ��ġ�� UI ������ �Ǵ���
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

