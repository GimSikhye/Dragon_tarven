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

            // GameScene에서 슬라이더 찾기
            if (scene.name == "GameScene")
            {
                // 이름으로 슬라이더 찾기 (씬 UI에 "BGMSlider", "SFXSlider"라는 이름의 오브젝트가 있어야 함)
                Slider bgmSlider = GameObject.Find("BGMSlider")?.GetComponent<Slider>();
                Slider sfxSlider = GameObject.Find("SFXSlider")?.GetComponent<Slider>();

                if (bgmSlider != null)
                {
                    Debug.Log("브금 슬라이더가 null아님");
                    bgmSlider.value = _bgmAudioSource.volume;
                    bgmSlider.onValueChanged.AddListener(SettingBGMVolume);
                }
                else
                {
                    Debug.Log("브금 슬라이더가 null");
                }


                if (sfxSlider != null)
                {
                    sfxSlider.value = _sfxAudioSource.volume;
                    sfxSlider.onValueChanged.AddListener(SettingSFXVolume);
                }
            }
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


    }
}
