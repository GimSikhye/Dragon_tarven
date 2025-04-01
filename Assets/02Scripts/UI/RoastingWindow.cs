using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using DG.Tweening;

namespace DalbitCafe.UI
{
    public class RoastingWindow : MonoBehaviour
    {
        [Header("메뉴 데이터 리스트")]
        [SerializeField] public List<CoffeeData> coffeDataList; // SO 데이터 리스트

        [Header("메뉴 UI 패널")]
        [SerializeField] public List<GameObject> menuContainers; //menu Container 패널 리스트

        void Start()
        {
            UpdateMenuUI();
        }

        void UpdateMenuUI()
        {
            for (int i = 0; i < coffeDataList.Count && i < menuContainers.Count; i++)
            {
                CoffeeData coffee = coffeDataList[i];
                GameObject container = menuContainers[i];

                // 각 UI 요소 가져오기
                TextMeshProUGUI nameTmp = container.transform.Find("name_tmp").GetComponent<TextMeshProUGUI>();
                TextMeshProUGUI coinTmp = container.transform.Find("coin_tmp").GetComponent<TextMeshProUGUI>();
                TextMeshProUGUI mugQtyTmp = container.transform.Find("mugQty_tmp").GetComponent<TextMeshProUGUI>();
                TextMeshProUGUI beanUseTmp = container.transform.Find("beanUse_tmp").GetComponent<TextMeshProUGUI>();

                Image iconImg = container.transform.Find("icon_Img").GetComponent<Image>();

                // UI 업데이트
                nameTmp.text = coffee.CoffeeName;
                coinTmp.text = coffee.Price.ToString();
                mugQtyTmp.text = "X " + coffee.MugQty.ToString();
                beanUseTmp.text = "- " + coffee.BeanUse.ToString();
                iconImg.sprite = coffee.MenuIcon;

            }
        }
    }


}

