using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    //enum으로 윈도우 창 이름 배열 관리하기

    public static UIManager Instance;
    [SerializeField] private GameObject[] panels;
    [SerializeField] private TextMeshProUGUI captionTmp;

    [Header("재화량 텍스트")]
    [SerializeField] private TextMeshProUGUI coffeeBeanText;
    [SerializeField] private TextMeshProUGUI coinText;
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
        foreach(var item  in panels)
        {
            item.SetActive(false);  
        }
        captionTmp.enabled = false;
    }

    public void UpdateCoffeeBeanUI(int value)
    {
        coffeeBeanText.text = value.ToString();
    }

    public void UpdateCoinUI(int value)
    {
        coinText.text = value.ToString();
    }

    public void UpdateGemUI(int value)
    {
        gemText.text = value.ToString();
    }

 




    public void ShowPopup()
    {
        panels[0].SetActive(true);
    }

    public void ShowExitWindow()
    {
        panels[1].SetActive(true);
    }

    public void ShowCapitonText()
    {
        Vector3 playerScreenPos = Camera.main.WorldToScreenPoint(PlayerCtrl.Instance.transform.position);

        captionTmp.rectTransform.position = playerScreenPos;
        captionTmp.enabled = true;
        captionTmp.text = "거리가 너무 멀어요!";
    }



}
