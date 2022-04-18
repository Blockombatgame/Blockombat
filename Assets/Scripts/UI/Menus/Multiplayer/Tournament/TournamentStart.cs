using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TournamentStart : Menu
{
    public Button register, join, back, create;

    private void Start()
    {
        register.onClick.AddListener(() => RegisterCallback());
        join.onClick.AddListener(() => JoinCallback());
        back.onClick.AddListener(() => BackCallback());
        create.onClick.AddListener(() => CreateCallback());
    }

    private void RegisterCallback()
    {
        EventManager.Instance.Click();
        List<string> menuTagNames = new List<string>();
        menuTagNames.Add("header");
        menuTagNames.Add("tournamentRegister");
        LoadMenu(register, menuTagNames);
    }

    private void JoinCallback()
    {
        EventManager.Instance.Click();
        List<string> menuTagNames = new List<string>();
        menuTagNames.Add("header");
        menuTagNames.Add("tournamentJoin");
        LoadMenu(join, menuTagNames);
    }

    private void BackCallback()
    {
        EventManager.Instance.Click();
        List<string> menuTagNames = new List<string>();
        menuTagNames.Add("header");
        menuTagNames.Add("home");
        LoadMenu(back, menuTagNames);
    }

    private void CreateCallback()
    {
        EventManager.Instance.Click();
        List<string> menuTagNames = new List<string>();
        menuTagNames.Add("header");
        menuTagNames.Add("tournamentCreate");
        LoadMenu(join, menuTagNames);
    }
}
