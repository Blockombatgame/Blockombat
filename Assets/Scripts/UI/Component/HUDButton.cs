using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HUDButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private GameObject highlight;

    private void Start()
    {
        highlight = transform.GetChild(0).gameObject;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        //highlight.SetActive(true);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        //highlight.SetActive(false);
    }
}
