using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsButton : MonoBehaviour
{
    public EnumClass.SettingsType settingsType;
    private GameObject highlight;
    private Button button;
    public int selectionID;

    private void Awake()
    {
        button = GetComponent<Button>();
        highlight = transform.GetChild(0).gameObject;
        button.onClick.AddListener(() => ClickDetected());
    }

    private void ClickDetected()
    {
        EventManager.Instance.Click();
        EventManager.Instance.SettingTypeChange(settingsType, selectionID);
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
