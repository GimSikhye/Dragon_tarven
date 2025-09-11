using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace DalbitCafe.Core
{
    public enum Bgm
    {
        Menu = 0,
        Game
    }

    public enum Sfx
    {
        Pouring,
        Syrup,
        Whipcream,
        Next,
        Correct
    }

    public class SoundManager : MonoSingleton<SoundManager>
    {
        [Header("Audio Sources")]
        [SerializeField] private AudioSource _bgmAudioSource;
        [SerializeField] private AudioSource _sfxAudioSource;

        [Header("Clips")]
        [SerializeField] private AudioClip[] _bgmClips;   // BGM 클립 배열
        [SerializeField] private AudioClip[] _sfxClips;   // SFX 클립 배열 (Sfx enum 순서대로 넣기)

        private Dictionary<Sfx, AudioClip> _sfxDictionary;

        protected override void Awake()
        {
            base.Awake(); 

            // 여기서 SoundManager 고유 초기화 실행
            _sfxDictionary = new Dictionary<Sfx, AudioClip>();
            for (int i = 0; i < _sfxClips.Length; i++)
            {
                if (i < System.Enum.GetValues(typeof(Sfx)).Length)
                {
                    _sfxDictionary[(Sfx)i] = _sfxClips[i];
                }
            }
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
            PlaySceneBGM(scene);
        }

        // 씬별 브금 선택
        public void PlaySceneBGM(Scene scene)
        {
            AudioClip clipToPlay = null;
            float volume = _bgmAudioSource.volume;

            switch (scene.name)
            {
                case "MainMenu":
                    clipToPlay = _bgmClips[(int)Bgm.Menu];
                    break;
                case "GameScene":
                    // clipToPlay = _bgmClips[(int)Bgm.Game];
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

        // --- 효과음 재생 ---
        public void PlaySFX(Sfx sfx)
        {
            if (_sfxDictionary.TryGetValue(sfx, out var clip))
            {
                _sfxAudioSource.PlayOneShot(clip, _sfxAudioSource.volume);
            }
            else
            {
                Debug.LogWarning($"SFX {sfx} 클립이 등록되지 않았습니다!");
            }
        }
        public void PlayLoopSFX(Sfx sfx)
        {
            if (_sfxDictionary.TryGetValue(sfx, out AudioClip clip))
            {
                _sfxAudioSource.clip = clip;
                _sfxAudioSource.loop = true;
                _sfxAudioSource.Play();
            }
        }

        public void StopSFX(Sfx sfx)
        {
            if (_sfxAudioSource.isPlaying &&
                _sfxAudioSource.clip == _sfxDictionary[sfx])
            {
                _sfxAudioSource.Stop();
                _sfxAudioSource.clip = null;
                _sfxAudioSource.loop = false;
            }
        }

        public bool IsSfxPlaying(Sfx sfx)
        {
            return _sfxAudioSource.isPlaying &&
                   _sfxAudioSource.clip == _sfxDictionary[sfx];
        }

        // --- 슬라이더 연결 ---
        public void SettingBGMVolume(float value)
        {
            Debug.Log("브금 음량 조절");
            _bgmAudioSource.volume = value;
        }

        public void SettingSFXVolume(float value)
        {
            _sfxAudioSource.volume = value;
        }

        public void RegisterSliders(Slider bgm, Slider sfx)
        {
            if (bgm != null)
            {
                bgm.value = _bgmAudioSource.volume;
                bgm.onValueChanged.RemoveAllListeners();
                bgm.onValueChanged.AddListener(SettingBGMVolume);
            }

            if (sfx != null)
            {
                sfx.value = _sfxAudioSource.volume;
                sfx.onValueChanged.RemoveAllListeners();
                sfx.onValueChanged.AddListener(SettingSFXVolume);
            }
        }
    }
}
