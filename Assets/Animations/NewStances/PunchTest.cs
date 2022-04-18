using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PunchTest : MonoBehaviour
{
    public Animator animator;

    public Coroutine attackRoutine, idleRoutine;

    public List<AnimData> unFlippedAnimDatas = new List<AnimData>();
    public List<AnimData> flippedAnimDatas = new List<AnimData>();

    public StanceState currentState;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            PlayAnimation();
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            PlayPunch();
        }
    }

    public void PlayPunch()
    {
        Debug.Log("punching");

        if (currentState == StanceState.Flip1)
        {
            StartCoroutine(AttackSequence(unFlippedAnimDatas));
        }
        else if (currentState == StanceState.Flip2)
        {
            StartCoroutine(AttackSequence(flippedAnimDatas));
        }
    }

    public void PlayAnimation()
    {
        Debug.Log("started");
        if(currentState == StanceState.Flip1)
        {
            StartCoroutine(ChangeStance(unFlippedAnimDatas));
            currentState = StanceState.Flip2;
        }else if (currentState == StanceState.Flip2)
        {
            StartCoroutine(ChangeStance(flippedAnimDatas));
            currentState = StanceState.Flip1;
        }
    }

    IEnumerator AttackSequence(List<AnimData> animData)
    {
        animator.CrossFadeInFixedTime(animData[2].animationClip.name, 0.25f);
        yield return new WaitForSeconds(animData[2].animationClip.length);
        PlayAnimation();
    }

    IEnumerator ChangeStance(List<AnimData> animData)
    {
        animator.CrossFadeInFixedTime(animData[1].animationClip.name, 0.25f);
        yield return new WaitForSeconds(animData[1].animationClip.length);
        animator.CrossFadeInFixedTime(animData[0].animationClip.name, 0.25f);
    }
}

public enum StanceState
{
    Flip1,
    Flip2,
}

public enum AnimDataMap
{
    Idle,
    ChangeStance,
    Punch,
}

[System.Serializable]
public struct AnimData
{
    public AnimDataMap animDataMap;
    public AnimationClip animationClip;
}