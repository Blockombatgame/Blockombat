using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance;
    public List<Menu> menus = new List<Menu>();

    private List<string> currentMenuName = new List<string>();
    private List<string> previousMenuName = new List<string>();

    public static string startMenuName = "start";
    public bool byPassStartUI = false;
    public GameObject vidPlayer;

    private void Awake()
    {
        if(Instance == null)
            Instance = this;
        else
        {
            Destroy(this);
        }
    }

    private void Start()
    {
        EventManager.Instance.OnMenuChange += OpenMenu;
        EventManager.Instance.OnOpenPopMenuAction += OpenPopMenu;
        EventManager.Instance.OnClosePopMenu += ClosePopMenu;
        if (!byPassStartUI)
            LoadUI();
    }

    public void OpenMenu(List<string> menuTagNames)
    {
        CloseMenus();

        for (int i = 0; i < menuTagNames.Count; i++)
        {
            if (menuTagNames[i] != "")
            {
                menus.Where(x => x.menuName == menuTagNames[i]).FirstOrDefault().OpenMenu();
            }
        }

        currentMenuName = menuTagNames;
    }

    public void OpenMenu(string closeMenuTag, string openMenuTag)
    {
        menus.Where(x => x.menuName == closeMenuTag).FirstOrDefault().CloseMenu();

        menus.Where(x => x.menuName == openMenuTag).FirstOrDefault().OpenMenu();

        List<string> currentMenusTags = new List<string>();
        currentMenusTags.Add(openMenuTag);
        currentMenuName = currentMenusTags;
    }

    public void OpenPreviousMenu()
    {
        List<string> _prev = previousMenuName;
        OpenMenu(_prev);
    }

    private void CloseMenus()
    {
        foreach (var menu in menus)
        {
            menu.CloseMenu();
            menu.previousMenuName = currentMenuName;
            previousMenuName = currentMenuName;
        }
    }

    private void OpenPopMenu(Action acceptCallback, Action declineCallback, string hostName, string stakeAmount)
    {
        Menu popMenu = menus.Where(x => x.menuName == "popMenu").FirstOrDefault();
        PopMenu pop = (PopMenu)popMenu;
        pop.SetPopMenu(acceptCallback, declineCallback, hostName, stakeAmount);
        pop.OpenMenu();
    }

    private void ClosePopMenu()
    {
        menus.Where(x => x.menuName == "popMenu").FirstOrDefault().CloseMenu();
    }

    private void LoadUI()
    {
        List<string> menuTagNames = new List<string>();
        if(startMenuName != "home")
        {
            menuTagNames.Add(startMenuName);
        }
        else
        {
            if(vidPlayer != null)
                vidPlayer.SetActive(false);

            menuTagNames.Add("header");
            menuTagNames.Add(startMenuName);
        }

        if(startMenuName != "")
        {
            EventManager.Instance.MenuChange(menuTagNames);
            startMenuName = "home";
        }
    }
}
