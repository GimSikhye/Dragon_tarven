using UnityEngine;

public class GameManagerLoader : MonoBehaviour
{
    [SerializeField] private GameObject gameManagerPrefab;

    private void Awake()
    {
        if (GameManager.HasInstance) return;

        var gmObj = Instantiate(gameManagerPrefab);
        gmObj.name = "GameManager";
        DontDestroyOnLoad(gmObj);

        //GameManager.AssignInstance(gmObj.GetComponent<GameManager>());
    }
}
