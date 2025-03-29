using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.EventSystems;

namespace DalbitCafe.UI
{
    public class SlideMenu : MonoBehaviour
    {
        [SerializeField]
        private Button _menuButton;
        [SerializeField] private List<RectTransform> _hiddenButtons; // 숨겨진 버튼들
        [SerializeField] private float _spacing = 150; // 버튼 간격
        [SerializeField] private float _duration = 0.5f; // 애니메이션 지속시간

        private bool _isMenuOpen = false; // 메뉴 오픈 상태
        private bool _isAnimating = false; // 중복 터치 방지
        private List<Vector2> _originalPositions = new List<Vector2>(); // 버튼 원래 위치 저장

        private void Start()
        {
            // 버튼 클릭 이벤트 등록
            _menuButton.onClick.AddListener(ToggleMenu);

            // 초기 위치 설정 (왼쪽에 숨기기)
            foreach (var btn in _hiddenButtons)
            {
                _originalPositions.Add(btn.anchoredPosition); // 원래 위치 저장
                btn.anchoredPosition = _menuButton.GetComponent<RectTransform>().anchoredPosition; // 메뉴 버튼 위치로 숨김

                // 버튼 숨기기
                btn.gameObject.SetActive(false);
            }
        }

        private void ToggleMenu()
        {
            // UI 중복 입력 방지
            if (EventSystem.current.IsPointerOverGameObject() == false) return;

            // 애니메이션 중이면 실행 방지
            if (_isAnimating) return;
            _isAnimating = true;

            _isMenuOpen = !_isMenuOpen;

            for (int i = 0; i < _hiddenButtons.Count; i++)
            {
                RectTransform btn = _hiddenButtons[i];

                if (_isMenuOpen)
                {
                    // 버튼 활성화 후 애니메이션 시작
                    btn.gameObject.SetActive(true);

                    Vector2 targetPos = _originalPositions[i] - new Vector2(_spacing * (i + 1), 0);
                    btn.DOAnchorPos(targetPos, _duration).SetEase(Ease.OutQuad)
                        .OnComplete(() => _isAnimating = false);
                }
                else
                {
                    // 버튼이 다시 메뉴 버튼으로 이동
                    btn.DOAnchorPos(_menuButton.GetComponent<RectTransform>().anchoredPosition, _duration).SetEase(Ease.InQuad)
                        .OnComplete(() =>
                        {
                            btn.gameObject.SetActive(false); // 버튼 숨김
                            _isAnimating = false;
                        });
                }
            }

        }
    }
}

