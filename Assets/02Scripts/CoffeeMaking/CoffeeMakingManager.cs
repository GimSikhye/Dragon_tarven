using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor.SceneManagement;

public enum CoffeeState { BaseSelect, Pouring, Syrup}
public class CoffeeMakingManager : MonoBehaviour
{
    [SerializeField] private GameObject basePanel;
    [SerializeField] private GameObject pouringPanel;
    [SerializeField] private GameObject syrupPanel;

    private CoffeeState currentState;
    private string selectedBase;

    private Vector2 simulatedTilt = Vector2.zero;
    [SerializeField] private Animator pouringAnimator; // 애니메이션 컨트롤용
    [SerializeField] private float pourSpeed = 10f;
    [SerializeField] private float pourDecreaseSpeed = 5f;
    [SerializeField] private TextMeshProUGUI amountText;

    private float pouredAmount = 0f; // 현재 따라진 양
    private float pourIntensity = 0f; // 기울기 기반으로 계산된 양

    // 시럽 펌핑 횟수 저장용 딕셔너리
    private Dictionary<string, int> syrupCounts = new();
    [SerializeField] private HorizontalLayoutGroup syrupListPanel;
    [SerializeField] private GameObject syrupLabelPrefab; // 시럽이 추가되었음을 나타낼 UI 프리팹 (Text)

    private void Start()
    {
        SetState(CoffeeState.BaseSelect);

    }
    private void Update()
    {
        if (currentState == CoffeeState.Pouring)
        {
            Debug.Log("붓기");
            HandlePouring();
        }

    }

    private void HandlePouring()
    {
        Vector3 tilt = GetSimulatedAcceleration(); // tilt: 기울이다

        // 기울기 감지
        bool isTilting = tilt.x > 0.3f || tilt.z > 0.3f;

        if (pouredAmount >= 100f)
        {
            pourIntensity = 0f;
            UpdatePouringAnimation(0);
            return;
        }

        if(isTilting)
        {
            // 기울어진 정도에 따라 기울기 강도 조절, Mathf.Clmap01: 주어진 숫자를 0과 1 사이에 묶어주는 함수
            pourIntensity = Mathf.Clamp01(Mathf.Max(Mathf.Abs(tilt.x), Mathf.Abs(tilt.z))); // 여기 부분 잘 모르겠음
            pouredAmount += Time.deltaTime * pourIntensity * pourSpeed;
        }
        else
        {
            // 점점 줄어들다가 멈춤, Mathf.MovToWards: 지정된 값(current)을 목표 값(target)으로 일정한 속도로 이동시키는 함수.
            pourIntensity = Mathf.MoveTowards(pourIntensity, 0f, Time.deltaTime * pourDecreaseSpeed);
        }

        pouredAmount = Mathf.Min(pouredAmount, 100f); // 100으로 제한 (capped)
        UpdatePouringUI(pouredAmount);
        UpdatePouringAnimation(pourIntensity);
        
    }

    private Vector3 GetSimulatedAcceleration()
    {
#if UNITY_EDITOR
        float x = 0f;
        float z = 0f;

        if (Input.GetKey(KeyCode.LeftArrow)) x = -0.7f;
        if (Input.GetKey(KeyCode.RightArrow)) x = 0.7f;
        if (Input.GetKey(KeyCode.UpArrow)) z = 0.7f;
        if (Input.GetKey(KeyCode.DownArrow)) z = -0.7f;

        return new Vector3(x, 0f, z);
#else
    return Input.acceleration;
#endif
    }


    public void SetState(CoffeeState newState)
    {
        currentState = newState;
        SetAllPanelsInactive();
        basePanel.SetActive(newState == CoffeeState.BaseSelect);
        pouringPanel.SetActive(newState == CoffeeState.Pouring);
        syrupPanel.SetActive(newState == CoffeeState.Syrup);

        // 상태별 초기화도 여기에 넣어줄 수 있음
        switch(newState)
        {
            case CoffeeState.BaseSelect:
                InitBaseSelect();
                break;
            case CoffeeState.Pouring:
                InitPouring();
                break;
            case CoffeeState.Syrup:
                InitSyrup();
                break;
        }
    }

    void InitBaseSelect()
    {
        selectedBase = "";
    }


    void InitPouring() 
    {
        pouredAmount = 0f;
        pourIntensity = 0f;

        UpdatePouringUI(pouredAmount);
        UpdatePouringAnimation(0);

    }
    void InitSyrup()
    {
        syrupCounts.Clear();

        // 기존 시럽 UI 초기화
        foreach(Transform child in syrupListPanel.transform)
        {
            Destroy(child.gameObject);
        }

    }

    private void SetAllPanelsInactive()
    {
        basePanel?.SetActive(false);
        pouringPanel?.SetActive(false);
        syrupPanel?.SetActive(false);
    }

    public void OnBaseSelected(string baseName)
    {
        selectedBase = baseName;
        SetState(CoffeeState.Pouring);
    }

    public void OnSyrupButtonClick(string syrupName, Animator anim)
    {
        // 애니메이션 실행
        anim.SetTrigger("Pump");

        // 시럽 카운트 추가
        if (!syrupCounts.ContainsKey(syrupName))
            syrupCounts[syrupName] = 1;
        else
            syrupCounts[syrupName]++;

        // 해당 버튼의 위치를 기준으로 UI 띄우기
        GameObject syrupButton = anim.gameObject; // Animator가 붙은 시럽 버튼 오브젝트
        UpdateSyrupUI(syrupCounts[syrupName], anim.transform);
    }

    private void CheckRecipe() // 레시피 조건 ScriptableObject로 만들기
    {
        //if(selectedBase == "Milk" && selectedSyrups.Contains("카라멜"))
        //{
        //    ShowResult("카라멜라뗴 완성!");
        //}
        //else
        //{
        //    ShowResult("레시피가 달라요!");
        //}
    }

    private void UpdatePouringAnimation(float intensity)
    {
        // 5단계 (0~4)로 기울기 정도를 나눔
        int level = 0;

        if (intensity > 0.9f)
            level = 4;
        else if (intensity > 0.7f)
            level = 3;
        else if (intensity > 0.5f)
            level = 2;
        else if (intensity > 0.3f)
            level = 1;
        else
            level = 0;

        pouringAnimator.SetInteger("PourLevel", level);
    }

    private void UpdatePouringUI(float amount)
    {
        if(amountText != null)
            amountText.text = $"{amount:F2} ml";
    }

    private void UpdateSyrupUI(int count, Transform targetTransform)
    {
        if (syrupLabelPrefab == null) return;

        GameObject label = Instantiate(syrupLabelPrefab, syrupPanel.transform);
        label.transform.position = Camera.main.WorldToScreenPoint(targetTransform.position);

        TextMeshProUGUI text = label.GetComponent<TextMeshProUGUI>();
        if (text != null)
            text.text = $"{count}";

        // 효과 컴포넌트 자동 실행
        SyrupLabelEffect effect = label.GetComponent<SyrupLabelEffect>();
        if (effect != null)
            effect.Play();
    }

}
