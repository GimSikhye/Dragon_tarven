using UnityEngine;
using TMPro;
using System;

public class WiperJudge : MonoBehaviour
{
    public RectTransform wiper;
    public RectTransform stainRect;
    public GameObject stainPrefab;
    private GameObject stainInstance;
    private RectTransform currentStainRect; // 새로 생성된 얼룩용



    public float perfectRange = 0.2f;
    public float greatRange = 0.4f;
    public float goodRange = 0.6f;

    private readonly VertexGradient badColor = new VertexGradient(new Color32(0,255,82,255), new Color32(129, 255,85, 255), new Color32(54,255,18,255), new Color32(67,255,28,255));
    private readonly VertexGradient greatColor = new VertexGradient(new Color32(255, 0, 220, 255), new Color32(255, 0, 220, 255), new Color32(0, 18, 163, 255), new Color32(255, 150, 217, 255));
    private readonly VertexGradient goodColor = new VertexGradient(new Color32(0, 215, 255, 255), new Color32(85, 189, 255, 255), new Color32(18, 35, 255, 255), new Color32(0, 51, 255, 255));
    private readonly VertexGradient perfectColor = new VertexGradient(new Color32(255, 0, 220, 255), new Color32(255, 0, 220, 255), new Color32(0, 105, 255, 255), new Color32(0, 255, 222, 255));
    private VertexGradient currentColor;

    public ParticleSystem cleanEffect;
    public TextMeshProUGUI resultText;
    public TextMeshProUGUI stageProgressText;

    public int currentStage = 1;
    public int maxStage = 5;

    private WiperController wiperController;



    private void Start()
    {
        wiperController = wiper.GetComponent<WiperController>();
        SpawnNewStain();
        UpdateStageText();

        resultText.enableVertexGradient = true;
        //resultText.fontMaterial.SetFloat(ShaderUtilities.ID_OutlineWidth, 0.2f);
        //resultText.fontMaterial.SetColor(ShaderUtilities.ID_OutlineColor, Color.black);

    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            wiperController.StopMovement();
            EvaluateHit();
        }
    }

    void EvaluateHit()
    {
        float distance = Mathf.Abs(wiper.anchoredPosition.x - stainRect.anchoredPosition.x);
        Debug.Log(distance);
        string result = "BAD"; currentColor = badColor;
        int score = 0;

        if(distance <= perfectRange) { result = "PERECT"; score = 10; currentColor = perfectColor; }
        else if(distance <= greatRange) { result = "GREAT"; score = 5; currentColor = greatColor; }
        else if (distance <= goodRange) { result = "GOOD"; score = 3; currentColor = goodColor; }

        // Show result
        resultText.text = result;
        resultText.colorGradient = currentColor;
        resultText.gameObject.SetActive(true);

        if(score > 0)
        {
            Instantiate(cleanEffect, stainRect.anchoredPosition, Quaternion.identity);
            Destroy(stainInstance);
        }
        Invoke(nameof(NextStage), 1.5f);

    }

    void NextStage()
    {
        resultText.gameObject.SetActive(false);
        currentStage++;

        if(currentStage > maxStage)
        {
            // 게임 종료. 게임 씬으로 옮기기
            stageProgressText.text = "CLEAR!";
            return;
        }


        wiperController.currentStage = currentStage;
        wiperController.ResumeMovement();

        SpawnNewStain();
        UpdateStageText();


    }

    void UpdateStageText()
    {
        stageProgressText.text = $"{currentStage}/{maxStage}";
    }

    void SpawnNewStain()
    {
        float x = UnityEngine.Random.Range(-150f, 210f);
        Vector2 newPos = new Vector3(x, stainRect.anchoredPosition.y);
        stainInstance = Instantiate(stainPrefab, stainRect); // new stain Position

        currentStainRect = stainInstance.GetComponent<RectTransform>();
        currentStainRect.anchoredPosition = newPos;

    }

}
