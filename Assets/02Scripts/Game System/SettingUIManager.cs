using DalbitCafe.Core;
using UnityEngine;
using UnityEngine.UI;

public class SettingUIManager : MonoBehaviour
{
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider sfxSlider;

    private void OnEnable()
    {
        // 팝업이 열릴 때 슬라이더 값을 SoundManager랑 동기화
        SoundManager.Instance.RegisterSliders(bgmSlider, sfxSlider);
    }
}
