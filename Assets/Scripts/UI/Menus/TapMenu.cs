using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TapMenu : Menu
{
    public Button loadButton;
    public GameObject bgMusic;

    private void Start()
    {
        loadButton.onClick.AddListener(() => LoadHomeMenu());
    }

    public void LoadHomeMenu()
    {
        EventManager.Instance.Click();
        bgMusic.SetActive(true);
        loadButton.interactable = false;
        List<string> menuTagNames = new List<string>();
        if (PlayerPrefs.GetString("GameStart") == "loggedIn")
        {
            menuTagNames.Add("header");
            menuTagNames.Add("home");
        }
        else
        {
            menuTagNames.Add("login");
            PlayerPrefs.SetString("GameStart", "loggedIn");
        }
        EventManager.Instance.MenuChange(menuTagNames);
        loadButton.interactable = true;
    }
}
