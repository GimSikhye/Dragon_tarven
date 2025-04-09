using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace DalbitCafe.Dialogue
{
    enum EventNum
    {
        Prologue = 0,
        FirstChapter,
        SecondChapter,
        ThirdChapter,
        FourthChapter,
        Ending
    }

    public class EventManager : MonoBehaviour
    {

        [SerializeField] private SpriteRenderer ChangeSprite;
        private AudioSource _audioSource;
        [SerializeField] private AudioClip explosion_sound;
        [SerializeField] private AudioClip shine_sound;

        // 이미지가 안바뀌는디
        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
        }
        public void EventChange(DialogueData dialogueData)
        {
            int eventNum = (int)dialogueData.eventNum;

            if (eventNum == (int)EventNum.Prologue)
            {
                SpriteChange(dialogueData.ChangeBG);

            }



        }

        private void SpriteChange(Sprite change)
        {
            ChangeSprite.sprite = change;
        }
    }

}
