using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartMenu : Menu
{
    public Button shop;
    public Button duel;
    public Button solo;
    public Button tournament;

    private void Start()
    {
        shop.onClick.AddListener(() => LoadShopMenu());
        duel.onClick.AddListener(() => LoadDuelMenu());
        solo.onClick.AddListener(() => LoadSoloMenu());
        tournament.onClick.AddListener(() => LoadTournamentMenu());
    }

    private void LoadTournamentMenu()
    {
        EventManager.Instance.Click();
        List<string> menuTagNames = new List<string>();
        menuTagNames.Add("header");
        menuTagNames.Add("tournamentStart");
        LoadMenu(tournament, menuTagNames);
    }

    private void LoadDuelMenu()
    {
        EventManager.Instance.Click();
        List<string> menuTagNames = new List<string>();
        menuTagNames.Add("header");
        menuTagNames.Add("duelHome");
        LoadMenu(solo, menuTagNames);
    }

    private void LoadShopMenu()
    {
        EventManager.Instance.Click();
        List<string> menuTagNames = new List<string>();
        menuTagNames.Add("shop");
        LoadMenu(shop, menuTagNames);
    }

    private void LoadSoloMenu()
    {
        EventManager.Instance.Click();
        List<string> menuTagNames = new List<string>();
        menuTagNames.Add("header");
        menuTagNames.Add("solo");
        LoadMenu(solo, menuTagNames);
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            if (Application.platform == RuntimePlatform.Android)
            {
                AndroidJavaObject activity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
                activity.Call<bool>("moveTaskToBack", true);
            }
            else
            {
                Application.Quit();
            }
        }
    }
}
