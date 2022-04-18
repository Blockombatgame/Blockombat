using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ActionController : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public string buttonKey;
    public bool multipleClick = true;
    private bool click;

    private void ButtonFire()
    {
        InputManager.Instance.AttackKeyPressed?.Invoke(buttonKey);
    }

    private void Update()
    {
        if (!multipleClick)
            return;

        if (click)
            ButtonFire();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Taptic.Light();
        click = true;
        ButtonFire();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        click = false;
    }
}
