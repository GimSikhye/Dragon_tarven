using UnityEngine;
using TMPro;
using DG;
using DG.Tweening;

public class TimingBarJudge : MonoBehaviour
{
    // 판정 코드 보기
    public RectTransform pointerRect;
    public RectTransform areaRect; // 얘가 갱신이 안됨
    public GameObject areaPrefab;
    public ParticleSystem cleanEffect;

    public TextMeshProUGUI resultText;
    public TextMeshProUGUI stageProgressText;
    [SerializeField] private GameObject startText;

    public int currentStage = 1;
    public int maxStage = 5;

    private GameObject areaInstance;
    private RectTransform currentAreaRect; // 새로 생성된 얼룩용

    private PointerController wiperController;
    [SerializeField] private UIShake uiShake;

    // accuracy
    public float perfectRange = 0.2f;
    public float greatRange = 0.4f;
    public float goodRange = 0.6f;

    // accuracy text gradient color
    private readonly VertexGradient badColor = new VertexGradient(new Color32(0,255,82,255), new Color32(129, 255,85, 255), new Color32(54,255,18,255), new Color32(67,255,28,255));
    private readonly VertexGradient greatColor = new VertexGradient(new Color32(255, 0, 220, 255), new Color32(255, 0, 220, 255), new Color32(0, 18, 163, 255), new Color32(255, 150, 217, 255));
    private readonly VertexGradient goodColor = new VertexGradient(new Color32(0, 215, 255, 255), new Color32(85, 189, 255, 255), new Color32(18, 35, 255, 255), new Color32(0, 51, 255, 255));
    private readonly VertexGradient perfectColor = new VertexGradient(new Color32(255, 0, 220, 255), new Color32(255, 0, 220, 255), new Color32(0, 105, 255, 255), new Color32(0, 255, 222, 255));
    private VertexGradient currentColor;




    private void Start()
    {
        startText.SetActive(true);
        startText.transform.DOMoveX(5,0.5f).OnComplete(()=>startText.SetActive(false));
  

        wiperController = pointerRect.GetComponent<PointerController>();
        SpawnNewArea();
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
        float distance = Mathf.Abs(pointerRect.anchoredPosition.x - currentAreaRect.anchoredPosition.x);
        Debug.Log(distance);
        string result = "BAD"; currentColor = badColor;
        int score = 0;

        if(distance <= perfectRange) { result = "PERFECT"; score = 10; currentColor = perfectColor; }
        else if(distance <= greatRange) { result = "GREAT"; score = 5; currentColor = greatColor; }
        else if (distance <= goodRange) { result = "GOOD"; score = 3; currentColor = goodColor; }

        // Show result
        resultText.text = result;
        resultText.colorGradient = currentColor;
        resultText.gameObject.SetActive(true);

        if(score > 0)
        {
            //Instantiate(cleanEffect, stainRect.position, Quaternion.identity);
            Instantiate(cleanEffect, Vector2.zero, Quaternion.identity);
        }
        if(score == 0)
        {
            Debug.Log("0점");
            uiShake.ShakeUI();
        }

        Destroy(areaInstance);
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

        SpawnNewArea();
        UpdateStageText();


    }

    void UpdateStageText()
    {
        stageProgressText.text = $"{currentStage}/{maxStage}";
    }

    void SpawnNewArea()
    {
        float x = UnityEngine.Random.Range(-350f, 350f);
        Vector2 newPos = new Vector3(x, areaRect.anchoredPosition.y);
        areaInstance = Instantiate(areaPrefab, areaRect); // area Rect Child

        currentAreaRect = areaInstance.GetComponent<RectTransform>();
        currentAreaRect.anchoredPosition = newPos; // new area Position 

    }

}
