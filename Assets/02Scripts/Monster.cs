using UnityEngine;
using UnityEngine.AI; // Nav Mesh 를 사용하기 위해 필요한 using 문

public class Monster : MonoBehaviour
{

    private static Monster instance;

    public static Monster Instance
    {
        get
        {
            if (instance == null) instance = new Monster();
            return instance;
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

    }


}