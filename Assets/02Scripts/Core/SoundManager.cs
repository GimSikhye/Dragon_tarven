using UnityEngine;
using UnityEngine.SceneManagement;

namespace DalbitCafe.Core
{
    public class SoundManager : MonoBehaviour
    {
        public static SoundManager Instance;

        [SerializeField] private AudioSource _bgmAudioSource;
        [SerializeField] private AudioSource _sfxAudioSource;
        [SerializeField] private AudioClip[] _bgmClips; // BGM 클립 배열

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(Instance.gameObject);
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
        }

        public void PlaySceneBGM(Scene scene)
        {
            AudioClip clipToPlay = null;
            float volume = 0.5f;

            switch (scene.name)
            {
                case "MainMenu":
                    clipToPlay = _bgmClips[(int)Bgm.Main];
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

        public void PlaySFX(AudioClip clip, float volume)
        {
            _sfxAudioSource.PlayOneShot(clip, volume);
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
        Main,
        Game
    }
}
