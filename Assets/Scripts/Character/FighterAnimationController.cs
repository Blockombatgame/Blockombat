using Photon.Pun;
using System;
using UnityEngine;

//Handles fighter animation play, stop and accessing animation storage
public delegate void OnAnimationChange(int viewID, int animationID);
public class FighterAnimationController : MonoBehaviour
{
    public OnAnimationChange AnimationChange;

    public float animationSpeed = 1;
    public FighterAnimationFactory fighterAnimationsSet;

    private Animator animator;
    private EnumClass.FighterAnimations currentAnimation;
    private bool lockIdleAnimation;

    private void Start()
    {
        animator = GetComponentInChildren<Animator>();
    }

    private void MotionPlay(Models.AnimationSet? animationDetail)
    {
        animator.speed = animationSpeed;
        animator.CrossFadeInFixedTime(animationDetail.Value.animationClip.name, 0.25f);
    }

    public void AnimationUpdate()
    {
        if(!lockIdleAnimation)
        {
            PhotonView photonView = GetComponent<PhotonView>();
            if (photonView != null)
            {
                if (photonView.IsMine)
                {
                    PlayIdleMotion(EnumClass.FighterAnimations.Idle);
                }
            }
            else
            {
                PlayIdleMotion(EnumClass.FighterAnimations.Idle);
            }
        }
    }

    public void PlayIdleMotion(EnumClass.FighterAnimations animationID)
    {
        if (currentAnimation == animationID)
            return;

        Models.AnimationSet? animationSet = fighterAnimationsSet.GetAnimation(animationID);

        currentAnimation = animationID;
        PhotonView photonView = GetComponent<PhotonView>();

        if (photonView != null && photonView.IsMine)
            AnimationChange?.Invoke(GetComponent<PhotonView>().ViewID, (int)animationID);

        if (animationSet != null)
        {
            MotionPlay(animationSet);
        }
    }

    public void PlayAnimation(EnumClass.FighterAnimations animationID, Action<float> callback)
    {
        PhotonView photonView = GetComponent<PhotonView>();

        if (photonView != null && photonView.IsMine)
        {
            AnimationChange?.Invoke(GetComponent<PhotonView>().ViewID, (int)animationID);
        }

        LockIdleAnimation();

        Models.AnimationSet? animationSet = fighterAnimationsSet.GetAnimation(animationID);

        currentAnimation = animationID;

        if (animationSet != null)
        {
            MotionPlay(animationSet);
            callback?.Invoke(animationSet.Value.animationClip.length);
        }
    }

    public void ProcessedAnimation(EnumClass.FighterAnimations animationID)
    {
        Models.AnimationSet? animationSet = fighterAnimationsSet.GetAnimation(animationID);

        currentAnimation = animationID;

        if (animationSet != null)
        {
            MotionPlay(animationSet);
        }
    }

    public void LockIdleAnimation()
    {
        lockIdleAnimation = true;
    }

    public void UnlockIdleAnimation()
    {
        lockIdleAnimation = false;
    }
}
