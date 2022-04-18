using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    public string menuName;
    internal List<string> previousMenuName;

    public void OpenMenu()
    {
        gameObject.SetActive(true);
    }

    public void CloseMenu()
    {
        gameObject.SetActive(false);
    }

    protected void LoadMenu(Button button, List<string> menuTagNames)
    {
        if(button != null)
            button.interactable = false;
        EventManager.Instance.MenuChange(menuTagNames);
        if (button != null)
            button.interactable = true;
    }
}
