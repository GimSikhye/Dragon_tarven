using UnityEngine;
using UnityEngine.SceneManagement;

namespace DalbitCafe.Core
{
    public class SoundManager : MonoSingleton<SoundManager> 
    {

        [SerializeField] private AudioSource _bgmAudioSource;
        [SerializeField] private AudioSource _sfxAudioSource;
        [SerializeField] private AudioClip[] _bgmClips; // BGM 클립 배열

        // 브금 볼륨'
        public void PlaySceneBGM(Scene scene)
        {
            AudioClip clipToPlay = null; // 플레이 할 브금
            float volume = _bgmAudioSource.volume;

            switch (scene.name)
            {
                case "MainMenu":
                    clipToPlay = _bgmClips[(int)Bgm.Menu];
                    break;
                case "GameScene":
                    clipToPlay = _bgmClips[(int)Bgm.Game];
                    break;
            }

            if (clipToPlay != null)
            {
                PlayBGM(clipToPlay, volume);
            }
        }

        public void PlayBGM(AudioClip clip, float volume)
        {
            _bgmAudioSource.Stop();
            _bgmAudioSource.clip = clip;
            _bgmAudioSource.volume = volume;
            _bgmAudioSource.Play();
        }

        public void PlaySFX(AudioClip clip)
        {
            _sfxAudioSource.PlayOneShot(clip, _sfxAudioSource.volume);
        }

        public void SettingBGMVolume(float value)
        {
            _bgmAudioSource.volume = value;
        }

        public void SettingSFXVolume(float value)
        {
            _sfxAudioSource.volume = value;
        }
    }

    public enum Bgm
    {
        Menu,
        Game
    }
}
