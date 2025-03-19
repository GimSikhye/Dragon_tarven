using Unity.VisualScripting;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;
    private AudioSource bgm_audioSource;
    private AudioSource sfx_audioSource;


    private void Start()
    {
        bgm_audioSource = transform.GetChild(0).GetComponent<AudioSource>();
        sfx_audioSource = transform.GetChild(1).GetComponent<AudioSource>();
    }

    void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    public void PlayBGM(AudioClip clip, float volume)
    {
        bgm_audioSource.PlayOneShot(clip, volume);
    }

    public void PlaySFX(AudioClip clip, float volume)
    {
        sfx_audioSource.PlayOneShot(clip, volume);  
    }


}
