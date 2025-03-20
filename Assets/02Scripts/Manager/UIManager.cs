using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    //enum으로 윈도우 창 이름 배열 관리하기

    public static UIManager Instance;
    [SerializeField] private GameObject[] panels;
    //[SerializeField] private GameObject captionText;
    [SerializeField] private TextMeshProUGUI captionTmp;


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

    public void ShowPopup()
    {
        panels[0].SetActive(true);
    }

    public void ShowExitWindow()
    {

    }

    public void ShowCapitonText()
    {
        Vector3 playerScreenPos = Camera.main.WorldToScreenPoint(PlayerCtrl.Instance.transform.position);

        captionTmp.rectTransform.position = playerScreenPos;
        captionTmp.enabled = true;
        captionTmp.text = "거리가 너무 멀어요!";
    }



}
