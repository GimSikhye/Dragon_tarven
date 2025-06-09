using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using UnityEditor.SceneManagement;

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
    [SerializeField] private TextMeshProUGUI amountText;

    private float pouredAmount = 0f; // ���� ������ ��
    private float pourIntensity = 0f; // ���� ������� ���� ��
    private Vector2 simulatedTilt = Vector2.zero;
    [SerializeField] private float simulatedTiltSpeed = 1.5f;
    [SerializeField] private float simulatedTiltMax = 1.2f;


    // �÷� ���� Ƚ�� ����� ��ųʸ�
    private Dictionary<string, int> syrupCounts = new();
    [SerializeField] private HorizontalLayoutGroup syrupListPanel;
    [SerializeField] private GameObject syrupLabelPrefab; // �÷��� �߰��Ǿ����� ��Ÿ�� UI ������ (Text)

    private void Start()
    {
        SetState(CoffeeState.BaseSelect);

    }
    private void Update()
    {
        if (currentState == CoffeeState.Pouring)
        {
            Debug.Log("�ױ�");
            HandlePouring();
        }

    }

    private void HandlePouring()
    {
        Vector3 tilt = GetSimulatedAcceleration(); // tilt: ����̴�

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

    private Vector3 GetSimulatedAcceleration()
    {
#if UNITY_EDITOR
        float xInput = 0f;
        float zInput = 0f;

        if (Input.GetKey(KeyCode.LeftArrow)) xInput = -1f;
        if (Input.GetKey(KeyCode.RightArrow)) xInput = 1f;
        if (Input.GetKey(KeyCode.UpArrow)) zInput = 1f;
        if (Input.GetKey(KeyCode.DownArrow)) zInput = -1f;

        // ������ �ִ� �ð��� ���� ���� ���� (Time.deltaTime ����)
        simulatedTilt.x = Mathf.Clamp(simulatedTilt.x + (xInput * simulatedTiltSpeed * Time.deltaTime), -simulatedTiltMax, simulatedTiltMax);
        simulatedTilt.y = Mathf.Clamp(simulatedTilt.y + zInput * simulatedTiltSpeed * Time.deltaTime, -simulatedTiltMax, simulatedTiltMax);

        // Ű�� ���� ���Ⱑ õõ�� ������
        if (xInput == 0) simulatedTilt.x = Mathf.MoveTowards(simulatedTilt.x, 0f, simulatedTiltSpeed * Time.deltaTime);
        if (zInput == 0) simulatedTilt.y = Mathf.MoveTowards(simulatedTilt.y, 0f, simulatedTiltSpeed * Time.deltaTime);

        return new Vector3(simulatedTilt.x, 0f, simulatedTilt.y);
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

        // ���� �÷� UI �ʱ�ȭ
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
    public void OnNextToBaseSelect() => SetState(CoffeeState.BaseSelect);
    public void OnNextToPouring() => SetState(CoffeeState.Pouring);
    public void OnNextToSyrup() => SetState(CoffeeState.Syrup);

    public void OnSyrupButtonClick(string syrupName)
    {
        Animator anim = EventSystem.current.currentSelectedGameObject.GetComponent<Animator>(); // ? 
        if (anim == null) return;

        // �ִϸ��̼� ����
        anim.SetTrigger("Pump");

        // �÷� ī��Ʈ �߰�
        if (!syrupCounts.ContainsKey(syrupName))
            syrupCounts[syrupName] = 1;
        else
            syrupCounts[syrupName]++;

        UpdateSyrupUI(syrupCounts[syrupName], anim.transform);
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
        // 5�ܰ� (0~4)�� ���� ������ ����
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

        // ȿ�� ������Ʈ �ڵ� ����
        SyrupLabelEffect effect = label.GetComponent<SyrupLabelEffect>();
        if (effect != null)
            effect.Play();
    }

}
