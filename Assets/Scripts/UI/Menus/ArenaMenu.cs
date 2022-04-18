using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArenaMenu : Menu
{
    public Button back, proceed;
    private bool arenaSelected = false, loaded = false;
    public Transform buttonsParent;
    public int selectedArena;

    private void Start()
    {
        back.onClick.AddListener(() => LoadCharacterMenu());
        proceed.onClick.AddListener(() => LoadLoadingMenu());
        proceed.interactable = false;
        EventManager.Instance.OnArenaSelect += ArenaSelect;

        LoadArenaData();
        loaded = true;
    }

    private void LoadCharacterMenu()
    {
        EventManager.Instance.Click();
        List<string> menuTagNames = new List<string>();
        menuTagNames.Add("header");
        menuTagNames.Add("character");
        LoadMenu(back, menuTagNames);
    }

    public void LoadLoadingMenu()
    {
        EventManager.Instance.Click();
        List<string> menuTagNames = new List<string>();
        menuTagNames.Add("loading");
        LoadMenu(proceed, menuTagNames);
    }

    private void ArenaSelect(int id)
    {
        for (int i = 0; i < buttonsParent.childCount; i++)
        {
            if (buttonsParent.GetChild(i).GetComponent<ArenaButton>().selectionID == id)
                buttonsParent.GetChild(i).GetComponent<ArenaButton>().ActivateHighlight();
            else
                buttonsParent.GetChild(i).GetComponent<ArenaButton>().DeactivateHighlight();
        }

        PlayerPrefs.SetInt("ArenaSelected", id);
        selectedArena = id;

        if (!arenaSelected)
        {
            proceed.interactable = true;
            arenaSelected = true;
        }
    }

    private void LoadArenaData()
    {
        for (int i = 0; i < FactoryManager.Instance.itemsFactory.GetItems(EnumClass.ItemType.Level).Count; i++)
        {
            switch (FactoryManager.Instance.itemsFactory.GetItems(EnumClass.ItemType.Level)[i].itemPurchaseState)
            {
                case EnumClass.ItemPurchaseState.NotBought:
                    buttonsParent.GetChild(i).GetComponent<ArenaButton>().button.interactable = false;
                    break;
                case EnumClass.ItemPurchaseState.Bought:
                    buttonsParent.GetChild(i).GetComponent<ArenaButton>().Setup(FactoryManager.Instance.itemsFactory.GetItems(EnumClass.ItemType.Level)[i].iconImage);
                    buttonsParent.GetChild(i).GetComponent<ArenaButton>().button.interactable = true;
                    break;
                case EnumClass.ItemPurchaseState.ComingSoon:
                    buttonsParent.GetChild(i).GetComponent<ArenaButton>().button.interactable = false;
                    break;
                default:
                    break;
            }
        }
    }

    private void OnEnable()
    {
        if (arenaSelected)
        {
            proceed.interactable = false;
            arenaSelected = false;
        }

        if (loaded)
            LoadArenaData();
    }
}
