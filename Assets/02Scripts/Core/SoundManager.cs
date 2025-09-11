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

    public class SoundManager : MonoSingleton<SoundManager>
    {
        [SerializeField] private AudioSource _bgmAudioSource;
        [SerializeField] private AudioSource _sfxAudioSource;
        [SerializeField] private AudioClip[] _bgmClips; // BGM 클립 배열

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
            // 씬별 BGM 재생
            PlaySceneBGM(scene);

           
            
        }

        // 씬별 브금 선택
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
                    //clipToPlay = _bgmClips[(int)Bgm.Game];
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
                bgm.onValueChanged.RemoveAllListeners(); // 중복 방지
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
