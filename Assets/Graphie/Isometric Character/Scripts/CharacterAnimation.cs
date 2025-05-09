using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace Graphie.Assets.Characters
{
    public class CharacterAnimation : MonoBehaviour
    {
        [SerializeField] private Animator animator = null;
        [SerializeField] private List<string> stateNames = new List<string>();
        private int targetStateId = 0;
        private static float currentStateTime = 0; // 어느 지점에서부터 재생할지(루프 애니메이션의 경우 다음 루프 사이클에 진입)

        public string CurrentState { get; private set; }

        private void Reset()
        {
            animator = GetComponent<Animator>();
            stateNames = new List<string>
            {
                "Front_Idle_Stand",
                "Front_Walk",
                "Front_Sit",
                "Front_Idle_Sit",
                "Back_Idle_Stand",
                "Back_Walk",
                "Back_Sit",
                "Back_Idle_Sit"
            };
        }

        private void OnEnable()
        {
            if (animator == null)
            {
                animator = GetComponent<Animator>();
            }
            animator.Play(stateNames[targetStateId], -1, currentStateTime); // -1: 기본 레이어 사용
        }

        private void Update()
        {
            currentStateTime = animator.GetCurrentAnimatorStateInfo(0).normalizedTime; // 현재 상태 정보를 가져옴(.normalizedTime : 현재 애니메이션의 진행률)
            // 이걸 나중에 Animator.Play(...)할 때 인자로 주면, 중간부터 재생 같은 효과를 줄 수 있다.
        }

        public void SetAnimation(int stateId)
        {
            stateId = Mathf.Clamp(stateId, 0, stateNames.Count - 1);
            targetStateId = stateId;

            if (animator.gameObject.activeInHierarchy)
            {
                animator.CrossFadeInFixedTime(stateNames[stateId], .2f); // 현재 재생 중인 애니메이션에서 목표 애니메이션으로 부드럽게 전환 (.2f: 새 상태로 전환할 데 걸리는 시간)
            }

            CurrentState = stateNames[targetStateId];
        }
    }
}