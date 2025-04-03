using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using DalbitCafe.Core;

namespace DalbitCafe.Dialogue
{
    public class DialogueManager : MonoBehaviour, IPointerClickHandler
    {
        private AudioSource _audioSource;

        [Header("UI 출력")]
        [SerializeField] private TextMeshProUGUI _nameText;
        [SerializeField] private TextMeshProUGUI _dialogueText;
        [SerializeField] private Image _characterPortrait;

        [Header("Event")]
        [SerializeField] private EventManager _eventManager;

        [Header("타이핑 속도")]
        [SerializeField] private float _typingSpeed = 0f;

        [Header("대사 모음집")]
        [SerializeField] private DialogueData[] _groups;

        [Header("대사 효과음")]
        [SerializeField] private AudioClip _typingSound;


        [SerializeField] private int _len = 0; // 전체 so 랭스 (그룹의 수)
        private int _textIndex = 0; // 개별 so의 랭스 (현재 출력되는 대사의 순서)

        private bool _isTyping = false; // 현재 타이핑 중인지 
        private bool _isEnd = false; // 모든 대사가 출력되었는지

        [SerializeField] private string _sceneName;

        private Coroutine _typingRoutine;

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
        }
        private void Start()
        {
            _typingRoutine = StartCoroutine(Typing(_groups[_len].Lines[_textIndex]));
        }

        private void NextTalk() // 다음 대사 출력
        {
            if (_isTyping) return; // 타이핑 중에는 스킵되지 않도록 방지

            _textIndex++; //0 1 2 3 4 5

            if (_textIndex == _groups[_len].Lines.Length) // 현재 그룹의 텍스트가 다 출력되었다면
            {
                _len++; // 다음 SO로 넘어감 //0 1 2 3 4 5

                if (_len != _groups.Length) // 전체 대사가 끝나지 않았다면 다음 SO로 //Length가 2라면 0 1
                {
                    _textIndex = 0; // 다음 SO로 넘어가니 초기화
                    _eventManager.EventChange(_groups[_len]);
                    _typingRoutine = StartCoroutine(Typing(_groups[_len].Lines[_textIndex]));
                }
                else
                {
                    // 마지막 그룹까지 완전히 끝났다면
                    _isEnd = true;
                    // 엔딩 씬으로 이동
                    EndTyping();
                }
            }
            else
            {
                _typingRoutine = StartCoroutine(Typing(_groups[_len].Lines[_textIndex]));
            }
        }

        IEnumerator Typing(string lineText)
        {
            _characterPortrait.sprite = _groups[_len].CharacterSprite;
            _nameText.text = _groups[_len].CharacterName;

            _dialogueText.text = string.Empty; // 초기화
            _isTyping = true;

            for (int i = 0; i < lineText.Length; i++)
            {
                _dialogueText.text += lineText[i]; // 한 글자씩 출력

                yield return new WaitForSeconds(_typingSpeed); // 텍스트 출력 속도 조정 (0.05초로 느리게)
            }

            _isTyping = false; // 타이핑 완료
        }

        private void EndTyping()
        {
            if (_isEnd) // 모든 대사가 끝난 경우에만 씬 전환
            {
                SceneManager.LoadScene(_sceneName);
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!_isEnd) // 현재 출력중인 텍스트 전체 출력
            {
                if (_isTyping)
                {
                    // 현재 타이핑 중이면 즉시 모든 텍스트를 출력
                    StopCoroutine(_typingRoutine);
                    _dialogueText.text = _groups[_len].Lines[_textIndex];
                    _isTyping = false;
                }
                else
                {
                    // 타이핑이 끝났으면 다음 대사로 넘어감
                    NextTalk();
                }
            }
        }
    }

}

