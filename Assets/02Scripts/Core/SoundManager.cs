using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;
    [SerializeField] private AudioSource _bgmAudioSource;
    [SerializeField] private AudioSource _sfxAudioSource;

    void Awake()
    {
        if (Instance == null)
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
        _bgmAudioSource.Stop();
        _bgmAudioSource.clip = clip;
        _bgmAudioSource.volume = volume;
        _bgmAudioSource.Play();
    }

    public void PlaySFX(AudioClip clip, float volume)
    {
        _sfxAudioSource.PlayOneShot(clip, volume);
    }

    public void SettingBGMVolume(float value) //슬라이더로 조절
    {
        _bgmAudioSource.volume = value;
    }

    public void SettingSFXVolume(float value)
    {
        _sfxAudioSource.volume = value;
    }


}
