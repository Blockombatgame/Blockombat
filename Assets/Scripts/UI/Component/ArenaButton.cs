using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArenaButton : MonoBehaviour
{
    public GameObject highlight;
    internal Button button;
    public int selectionID;
    public Image image;

    private void Awake()
    {
        button = GetComponent<Button>();
        image = transform.GetChild(0).GetComponent<Image>();
        highlight = transform.GetChild(1).gameObject;
        button.onClick.AddListener(() => ClickDetected());
    }

    public void Setup(Sprite sprite)
    {
        image.sprite = sprite;
    }

    private void ClickDetected()
    {
        EventManager.Instance.Click();
        EventManager.Instance.ArenaSelect(selectionID);
    }

    public void ActivateHighlight()
    {
        highlight.SetActive(true);
    }

    public void DeactivateHighlight()
    {
        highlight.SetActive(false);
    }
}
