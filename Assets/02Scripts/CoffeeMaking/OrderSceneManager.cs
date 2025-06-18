using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class OrderSceneManager : MonoBehaviour
{
    [SerializeField] private Image portraitImage;
    [SerializeField] private TextMeshProUGUI speechText;
    [SerializeField] private Button hintButton;
    [SerializeField] private Button okayButton;
    [SerializeField] private GameObject canvasOrder;
    [SerializeField] private GameObject canvasCoffee;

    [Header("Portrait Sprites")]
    public List<CustomerPortraitEntry> portraitEntries;

    [Header("Menu Recipes")]
    public List<MenuWithVariants> menuRecipeData;

    private MenuVariantRecipe selectedRecipe;
    private int currentHintIndex = 0;


    private void Start()
    {
        InitUI();
    }

    private void InitUI()
    {
        //  1. 캐릭터 이미지 설정
        var entry = portraitEntries.Find(p => OrderData.CustomerName.Contains(p.name));
        if (entry != null)
            portraitImage.sprite = entry.sprite;

        //  2. 메뉴 레시피에서 무작위로 선택
        var menu = menuRecipeData.Find(m => m.menuType == OrderData.CurrentMenu);
        if (menu == null || menu.variants.Count == 0)
        {
            Debug.LogError("레시피 없음!");
            return;
        }

        selectedRecipe = menu.variants[Random.Range(0, menu.variants.Count)];
        OrderData.CurrentRecipe = selectedRecipe;

        //  3. 힌트 인덱스 초기화 및 첫 힌트 출력
        currentHintIndex = 0;
        if (selectedRecipe.hintText != null && selectedRecipe.hintText.Length > 0)
        {
            speechText.text = selectedRecipe.hintText[0];
        }
        else
        {
            speechText.text = "힌트가 없습니다!";
        }

        //  4. 버튼 이벤트 등록
        hintButton.onClick.AddListener(OnHintClicked);
        okayButton.onClick.AddListener(OnOkayClicked);
    }

    private void OnHintClicked()
    {
        if (selectedRecipe.hintText == null || selectedRecipe.hintText.Length == 0)
            return;

        currentHintIndex = (currentHintIndex + 1) % selectedRecipe.hintText.Length;
        speechText.text = selectedRecipe.hintText[currentHintIndex];
    }


    private void OnOkayClicked()
    {
        canvasOrder.SetActive(false);
        canvasCoffee.SetActive(true);
    }

    [System.Serializable]
    public class CustomerPortraitEntry
    {
        public string name;
        public Sprite sprite;
    }

    [System.Serializable]
    public class MenuWithVariants
    {
        public CustomerOrder.MenuType menuType;
        public List<MenuVariantRecipe> variants;
    }
}
