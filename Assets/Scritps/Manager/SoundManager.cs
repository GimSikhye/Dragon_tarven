using Unity.VisualScripting;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;
    [SerializeField] private AudioSource bgm_audioSource;
    [SerializeField] private AudioSource sfx_audioSource;


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
        bgm_audioSource.clip = clip;
        bgm_audioSource.volume = volume;
        bgm_audioSource.Play();
    }

    public void PlaySFX(AudioClip clip, float volume)
    {
        sfx_audioSource.PlayOneShot(clip, volume);  
    }


}
