using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterButton : MonoBehaviour
{
    private GameObject highlight;
    internal Button button;
    public int selectionID;
    internal EnumClass.PlayerTag playerTag;
    public Image image;

    private void Awake()
    {
        playerTag = EnumClass.PlayerTag.None;
        button = GetComponent<Button>();
        image = transform.GetChild(0).GetComponent<Image>();
        highlight = transform.GetChild(1).gameObject;
        button.onClick.AddListener(() => ClickDetected());
    }

    public void Setup(Sprite sprite, EnumClass.PlayerTag _playerTag)
    {
        image.sprite = sprite;
        playerTag = _playerTag;
    }

    private void ClickDetected()
    {
        EventManager.Instance.Click();
        EventManager.Instance.CharacterSelect(playerTag.ToString());
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
