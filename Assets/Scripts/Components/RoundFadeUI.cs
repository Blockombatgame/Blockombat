using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundFadeUI : MonoBehaviour
{
    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();

        EventManager.Instance.OnLoadFadePanel += LoadFadePanel;
    }

    public void LoadFadePanel()
    {
        animator.Play("RoundFadeIn");
    }
}
