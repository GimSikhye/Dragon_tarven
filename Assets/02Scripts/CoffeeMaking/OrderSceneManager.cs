using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class OrderSceneManager : MonoBehaviour
{
    [Header("카운터 손님")]
    [SerializeField] private Image portraitImage;
    [SerializeField] private TextMeshProUGUI speechText;
    [SerializeField] private Button hintButton;
    [SerializeField] private Button okayButton;

    [Header("Canvas 컴포넌트")]
    [SerializeField] private GameObject orderCanvas;
    [SerializeField] private GameObject coffeeMakingCanvas;

    [Header("미니게임 결과")]
    [SerializeField] private GameObject resultPanel;
    [SerializeField] private Button settleButton; // 정산 버튼

    [Header("손님 초상화 스프라이트 목록")]
    public List<CustomerPortraitEntry> portraitEntries;

    [Header("변종 메뉴 레시피 목록")]
    public List<MenuWithVariants> menuRecipeData;

    private MenuVariantRecipe selectedRecipe;
    private int currentHintIndex = 0;

    private void Start()
    {
        InitUI();
    }

    private void InitUI()
    {
        // 손님 초상화 가져오기
        CustomerPortraitEntry entry = portraitEntries.Find(p => OrderData.CustomerName.Contains(p.name));
        if (entry != null)
            portraitImage.sprite = entry.sprite;

        // 메뉴 레시피에서 무작위로 선택
        MenuWithVariants menu = menuRecipeData.Find(m => m.menuType == OrderData.CurrentMenu);
        if (menu == null || menu.variants.Count == 0)
        {
            //Debug.LogError(" 메뉴를 못 찾았거나, 해당 메뉴의 변종 레시피 없음!");
            return;
        }

        selectedRecipe = menu.variants[Random.Range(0, menu.variants.Count)];
        OrderData.CurrentRecipe = selectedRecipe;

        // 힌트 인덱스 초기화 및 첫 힌트 출력
        currentHintIndex = 0;
        if (selectedRecipe.hintText != null && selectedRecipe.hintText.Length > 0)
        {
            speechText.text = selectedRecipe.hintText[0];
        }
        else
        {
            speechText.text = "힌트가 없습니다!";
        }

        // 캔버스 전환 처리
        coffeeMakingCanvas.SetActive(false); // 제작 미니게임 Canvas 끄기
        orderCanvas.SetActive(true);   // 손님 주문 Canvas 켜기

        settleButton.onClick.AddListener(OnSettleClicked); // 정산 버튼
        resultPanel.SetActive(false);

        // 버튼 이벤트 등록
        hintButton.onClick.AddListener(OnHintClicked);
        okayButton.onClick.AddListener(OnOkayClicked);
    }

    private void OnHintClicked()
    {
        // 해당 레시피의 힌트가 없다면
        if (selectedRecipe.hintText == null || selectedRecipe.hintText.Length == 0) return;

        // 돌아가면서 힌트 출력
        currentHintIndex = (currentHintIndex + 1) % selectedRecipe.hintText.Length;
        speechText.text = selectedRecipe.hintText[currentHintIndex];
    }

    private void OnSettleClicked() // 정산 버튼 클릭
    {
        if (OrderData.Result != null)
        {
            RewardCalculator.GrantCoinByResult(OrderData.Result);
            Debug.Log("정산 완료: 보상 지급됨.");

            // 중복 지급 방지를 위해 버튼 비활성화
            settleButton.interactable = false;
            SceneManager.LoadScene("GameScene");

            // 보상 완료 메시지 UI 출력 등도 여기에 추가 가능
        }
        else
        {
            Debug.LogWarning("정산 실패: 결과 데이터 없음!");
        }
    }

    private void OnOkayClicked()
    {
        Debug.Log("오케이");
        orderCanvas.SetActive(false);
        coffeeMakingCanvas.SetActive(true);
    }

    [System.Serializable]
    public class CustomerPortraitEntry
    {
        public string name;
        public Sprite sprite;
    }

    [System.Serializable]
    public class MenuWithVariants // 변종메뉴 (ex. 따뜻한 아메리카노)
    {
        public CustomerOrder.MenuType menuType; // 일반메뉴 타입
        public List<MenuVariantRecipe> variants; // 변종 메뉴 레시피
    }
}
