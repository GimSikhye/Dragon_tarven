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
        [SerializeField] public List<GameObject> coffeMachineMenuContainers; //menu Container 패널 리스트

        void Start()
        {
            UpdateMenuUI();
        }

        void UpdateMenuUI()
        {
            Debug.Log("menu UI 업데이트-전");

            for (int i = 0; i < coffeDataList.Count && i < coffeMachineMenuContainers.Count; i++)
            {
                Debug.Log("menu UI 업데이트-후");
                CoffeeData coffee = coffeDataList[i];
                GameObject container = coffeMachineMenuContainers[i];

                // 각 UI 요소 가져오기
                TextMeshProUGUI menuNameTmp = container.transform.Find("menuNameText").GetComponent<TextMeshProUGUI>();
                TextMeshProUGUI requireCoinTmp = container.transform.Find("coinAmountText").GetComponent<TextMeshProUGUI>();
                TextMeshProUGUI menuQuantityTmp = container.transform.Find("menuQuantityText").GetComponent<TextMeshProUGUI>();
                TextMeshProUGUI requireCoffeeBeanTmp = container.transform.Find("coffeeBeanAmountText").GetComponent<TextMeshProUGUI>();

                Image menuIcon = container.transform.Find("coffeeMenuIcon").GetComponent<Image>();

                // UI 업데이트
                menuNameTmp.text = coffee.CoffeeName;
                requireCoinTmp.text = coffee.Price.ToString();
                menuQuantityTmp.text = "X " + coffee.MugQty.ToString();
                requireCoffeeBeanTmp.text = "- " + coffee.BeanUse.ToString();
                if(menuIcon !=null)
                {
                    Debug.Log("이미지 등록됨");
                    menuIcon.sprite = coffee.MenuIcon;

                }

            }
        }
    }


}

