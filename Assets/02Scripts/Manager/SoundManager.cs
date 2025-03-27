using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

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
        else
        {
            Destroy(Instance);
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
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

    public void SettingBGMVolume(float value) //슬라이더로 조절
    {
        bgm_audioSource.volume = value;
    }

    public void SettingSFXVolume(float value)
    {
        sfx_audioSource.volume = value;
    }


}
