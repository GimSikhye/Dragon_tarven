using DalbitCafe.Core;
using DalbitCafe.UI;
using UnityEngine.SceneManagement;
using UnityEngine;
using DalbitCafe.Deco;
using DalbitCafe.Inputs;
using DalbitCafe.Operations;
using DalbitCafe.Map;

public class GameManager : MonoSingleton<GameManager>
{
    public Transform CashDesk;
    public Transform OutSide;

    private void Start()
    {
        //_playerStatsManager.Load();
    }

    private void Update()
    {
#if UNITY_ANDROID
        if (Input.GetKey(KeyCode.Escape))
        {
            UIManager.Instance.ShowExitPopUp();
        }
#endif
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        //_soundManager.PlaySceneBGM(scene);
        if(scene.name == "GameScene")
        {
            CashDesk = GameObject.Find("Cashdesk").transform;
            OutSide = GameObject.Find("Outside").transform;
        }
    }

  
}
