using System.Collections.Generic;
using UnityEngine;

public enum CoffeeState { BaseSelect, Pouring, Syrup}
public class CoffeeMakingManager : MonoBehaviour
{
    [SerializeField] private GameObject basePanel;
    [SerializeField] private GameObject pouringPanel;
    [SerializeField] private GameObject syrupPanel;

    private CoffeeState currentState;
    private string selectedBase;

    [SerializeField] private Animator pouringAnimator; // 애니메이션 컨트롤용
    [SerializeField] private float pourSpeed = 10f;
    [SerializeField] private float pourDecreaseSpeed = 5f;
    private float pouredAmount = 0f; // 현재 따라진 양
    private float pourIntensity = 0f; // 기울기 기반으로 계산된 양

    private List<string> selectedSyrups = new();

    private void Start()
    {

    }

    private void Update()
    {
        if (currentState == CoffeeState.Pouring)
        {
            HandlePouring();
        }

    }

    private void HandlePouring()
    {
        Vector3 tilt = Input.acceleration; // tilt: 기울이다

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

    public void SetState(CoffeeState newState)
    {
        currentState = newState;

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

    void InitBaseSelect() { /* 버튼 이벤트 연결 등 */}
    void InitPouring() { /* 기울기 초기화*/}
    void InitSyrup() { /* 시럽 버튼 활성화*/}

    public void OnBaseSelected(string baseName)
    {
        selectedBase = baseName;
        SetState(CoffeeState.Pouring);
    }

    public void OnSyrupButtonClick(string syrupName, Animator anim)
    {
        if (selectedSyrups.Contains(syrupName)) return;

        anim.SetTrigger("Pump");
        selectedSyrups.Add(syrupName);
        UpdateSyrupUI(syrupName);
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
        // 4단계 애니메이션 트리거 (0~3)
        int level = 0;

        if (intensity > 0.8f)
            level = 3;
        else if (intensity > 0.5f)
            level = 2;
        else if (intensity > 0.2f)
            level = 1;
        else
            level = 0;

        pouringAnimator.SetInteger("PourLevel", level);
    }

}
