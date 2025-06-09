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

    [SerializeField] private Animator pouringAnimator; // �ִϸ��̼� ��Ʈ�ѿ�
    [SerializeField] private float pourSpeed = 10f;
    [SerializeField] private float pourDecreaseSpeed = 5f;
    private float pouredAmount = 0f; // ���� ������ ��
    private float pourIntensity = 0f; // ���� ������� ���� ��

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
        Vector3 tilt = Input.acceleration; // tilt: ����̴�

        // ���� ����
        bool isTilting = tilt.x > 0.3f || tilt.z > 0.3f;

        if (pouredAmount >= 100f)
        {
            pourIntensity = 0f;
            UpdatePouringAnimation(0);
            return;
        }

        if(isTilting)
        {
            // ������ ������ ���� ���� ���� ����, Mathf.Clmap01: �־��� ���ڸ� 0�� 1 ���̿� �����ִ� �Լ�
            pourIntensity = Mathf.Clamp01(Mathf.Max(Mathf.Abs(tilt.x), Mathf.Abs(tilt.z))); // ���� �κ� �� �𸣰���
            pouredAmount += Time.deltaTime * pourIntensity * pourSpeed;
        }
        else
        {
            // ���� �پ��ٰ� ����, Mathf.MovToWards: ������ ��(current)�� ��ǥ ��(target)���� ������ �ӵ��� �̵���Ű�� �Լ�.
            pourIntensity = Mathf.MoveTowards(pourIntensity, 0f, Time.deltaTime * pourDecreaseSpeed);
        }

        pouredAmount = Mathf.Min(pouredAmount, 100f); // 100���� ���� (capped)
        UpdatePouringUI(pouredAmount);
        UpdatePouringAnimation(pourIntensity);
        
    }

    public void SetState(CoffeeState newState)
    {
        currentState = newState;

        basePanel.SetActive(newState == CoffeeState.BaseSelect);
        pouringPanel.SetActive(newState == CoffeeState.Pouring);
        syrupPanel.SetActive(newState == CoffeeState.Syrup);

        // ���º� �ʱ�ȭ�� ���⿡ �־��� �� ����
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

    void InitBaseSelect() { /* ��ư �̺�Ʈ ���� �� */}
    void InitPouring() { /* ���� �ʱ�ȭ*/}
    void InitSyrup() { /* �÷� ��ư Ȱ��ȭ*/}

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

    private void CheckRecipe() // ������ ���� ScriptableObject�� �����
    {
        //if(selectedBase == "Milk" && selectedSyrups.Contains("ī���"))
        //{
        //    ShowResult("ī����� �ϼ�!");
        //}
        //else
        //{
        //    ShowResult("�����ǰ� �޶��!");
        //}
    }

    private void UpdatePouringAnimation(float intensity)
    {
        // 4�ܰ� �ִϸ��̼� Ʈ���� (0~3)
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
