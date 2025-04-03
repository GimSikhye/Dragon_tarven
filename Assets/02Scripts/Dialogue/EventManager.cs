using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DalbitCafe.Dialogue
{
    public class EventManager : MonoBehaviour
    {

        [SerializeField] private SpriteRenderer ChangeSprite;
        private AudioSource audioSource;
        [SerializeField] private AudioClip explosion_sound;
        [SerializeField] private AudioClip shine_sound;


        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
        }
        public void EventChange(DialogueData textso)
        {
            int EventNum = textso.EventNum;
            if (EventNum == 0)
            {
                Debug.Log("이벤트 없음");
            }
            if (EventNum == 1)
            {
                SpriteChange(textso.ChangeBG);
                audioSource.PlayOneShot(explosion_sound, 0.3f);
                audioSource.Play();
            }
            if (EventNum == 2)
            {
                audioSource.PlayOneShot(shine_sound, 0.4f);

            }


        }

        private void SpriteChange(Sprite change)
        {
            ChangeSprite.sprite = change;
        }
    }

}
