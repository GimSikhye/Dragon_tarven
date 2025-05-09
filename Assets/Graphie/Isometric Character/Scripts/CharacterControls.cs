using System.Collections;
using System.Collections.Generic;
using Graphie.Assets.Characters;

using UnityEngine;
using UnityEngine.UI;

public class CharacterControls : MonoBehaviour
{
    [SerializeField] 
    Text animationState;
    [SerializeField]
    private List<GameObject> characterSkins = new List<GameObject>();
    private int currentSkin = 0, currentAnimation = 0; // 현재 애니메이션
    [SerializeField]
    private int totalAnimationClips = 0;
    [SerializeField]
    private List<CharacterAnimation> characterAnimations = new List<CharacterAnimation>(); // 스크립트
    private void Reset()
    {
        characterSkins = new List<GameObject>();
        characterAnimations = new List<CharacterAnimation>();
        foreach (Transform child in transform) // 오브젝트의 자식들
        {
            characterSkins.Add(child.gameObject);
            characterAnimations.Add(child.GetComponent<CharacterAnimation>());
            child.gameObject.SetActive(false);
        }

        if (characterSkins.Count > 0)
        {
            characterSkins[0].SetActive(true);
        }

        totalAnimationClips = 8;
        //List<Renderer> allRenderers = new List<Renderer>();
        //GetComponentsInChildren<Renderer>(true, allRenderers);
    }

    private void Update()
    {
        #region Skin
        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) // 다음 스킨
        {
            currentSkin++;
            SetSkin();
        }
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) // 이전 스킨
        {
            currentSkin--;
            SetSkin();
        }
        #endregion

        #region Animation
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) // 다음 애니메이션
        {
            currentAnimation++;
            SetAnimation();
        }
        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) // 이전 애니메이션
        {
            currentAnimation--;
            SetAnimation();
        }
        #endregion
    }

    private void Awake()
    {
        currentSkin = 0;
        SetSkin();

        currentAnimation = 0;
        SetAnimation();
    }

    private void SetSkin()
    {
        if (characterSkins.Count <= 0) return;
        currentSkin = (int)Mathf.Repeat(currentSkin, characterSkins.Count); // 인덱스 유효하게 만들기
        foreach (GameObject skin in characterSkins)
        {
            if (skin.activeInHierarchy) // 현재 켜져있는 스킨 꺼줌
                skin.SetActive(false);
        }
        characterSkins[currentSkin].SetActive(true); // 선택한 스킨 껴줌
    }
    private void SetAnimation()
    {
        currentAnimation = (int)Mathf.Repeat(currentAnimation, totalAnimationClips); // 8
        foreach (CharacterAnimation characterAnimation in characterAnimations)
        {
            characterAnimation.SetAnimation(currentAnimation);
            animationState.text = $"Animation State: {characterAnimation.CurrentState}";
        }
    }
}
