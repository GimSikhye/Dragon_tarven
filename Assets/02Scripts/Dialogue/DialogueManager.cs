using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using DalbitCafe.Core;

namespace DalbitCafe.Dialogue
{
    public class DialogueManager : MonoBehaviour
    {
        private AudioSource _audioSource;

        [Header("UI 출력")]
        [SerializeField] private TextMeshProUGUI _nameText;
        [SerializeField] private TextMeshProUGUI _characterText;
        [SerializeField] private Image _characterPortrait;
        [SerializeField] private Image FadePanel;
        [SerializeField] private SpriteRenderer BackgroundSprite;

        [Header("Event")]
        [SerializeField] private EventManager eventManager;

        [Header("타이핑 속도")]
        [SerializeField] private float TypingSpeed = 0f;

        [Header("대사 모음집")]
        [SerializeField] private DialogueData[] Groups;

        [Header("대사 효과음")]
        [SerializeField] private AudioClip typingSound;


        private int len = 0; // 전체 so 랭스
        private int textlen = 0; // 개별 so의 랭스

        bool isTyping = false; // 현재 타이핑 중인지 
        bool isEnd = false; // 모든 대사가 출력되었는지

        [SerializeField] private string SceneName;

        private Coroutine typingRoutine;

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
        }
        private void Start()
        {
            typingRoutine = StartCoroutine(Typing(Groups[len].Lines[textlen]));
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space) && !isEnd)
            {
                if (isTyping)
                {
                    // 현재 타이핑 중이면 즉시 모든 텍스트를 출력
                    StopCoroutine(typingRoutine);
                    _characterText.text = Groups[len].Lines[textlen];
                    isTyping = false;
                }
                else
                {
                    // 타이핑이 끝났으면 다음 대사로 넘어감
                    NextTalk();
                }
            }
        }

        private void NextTalk() // 다음 대사 출력
        {
            if (isTyping) return; // 타이핑 중에는 스킵되지 않도록 방지

            textlen++;

            if (textlen == Groups[len].Lines.Length)
            {
                len++; // 다음 SO로 넘어감

                if (len != Groups.Length) // 전체 대사가 끝나지 않았다면 다음 SO로
                {
                    textlen = 0; // 다음 SO로 넘어가니 초기화
                    eventManager.EventChange(Groups[len]);
                    typingRoutine = StartCoroutine(Typing(Groups[len].Lines[textlen]));
                }
                else
                {
                    // 완전히 끝났다면
                    isEnd = true;
                    FadePanel.gameObject.SetActive(true);
                    StartCoroutine(GameManager.Instance.FadeIn(FadePanel, 1, EndTyping));
                }
            }
            else
            {
                typingRoutine = StartCoroutine(Typing(Groups[len].Lines[textlen]));
            }
        }

        IEnumerator Typing(string text)
        {
            _characterPortrait.sprite = Groups[len].CharacterSprite;
            _nameText.text = Groups[len].CharacterName;

            _characterText.text = string.Empty; // 초기화
            isTyping = true;

            for (int i = 0; i < text.Length; i++)
            {
                _characterText.text += text[i];

                if (typingSound != null && !_audioSource.isPlaying)
                {
                    Debug.Log("소리");
                    _audioSource.PlayOneShot(typingSound, 0.5f);
                }

                yield return new WaitForSeconds(TypingSpeed); // 텍스트 출력 속도 조정 (0.05초로 느리게)
            }

            isTyping = false; // 타이핑 완료
        }

        private void EndTyping()
        {
            if (isEnd) // 모든 대사가 끝난 경우에만 씬 전환
            {
                SceneManager.LoadScene(SceneName);
            }
        }
    }

}

