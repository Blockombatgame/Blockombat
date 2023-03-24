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
    [SerializeField] private List<Button> paginations = new List<Button>();
    public int lastCount;

    private void Start()
    {
        back.onClick.AddListener(() => LoadCharacterMenu());
        proceed.onClick.AddListener(() => LoadLoadingMenu());
        proceed.interactable = false;
        EventManager.Instance.OnArenaSelect += ArenaSelect;

        LoadArenaData();
        InitializePaginations();
        TurnOnOnePaginations(0);

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

        PlayerPrefs.SetInt("ArenaSelected", id + lastCount);
        selectedArena = id;

        if (!arenaSelected)
        {
            proceed.interactable = true;
            arenaSelected = true;
        }
    }

    private void LoadArenaData()
    {
        for (int i = lastCount; i < lastCount + 8; i++)
        {
            if (i >= FactoryManager.Instance.itemsFactory.GetItems(EnumClass.ItemType.Level).Count)
            {
                buttonsParent.GetChild(i - lastCount).GetComponent<ArenaButton>().gameObject.SetActive(false);
                continue;
            }

            switch (FactoryManager.Instance.itemsFactory.GetItems(EnumClass.ItemType.Level)[i].itemPurchaseState)
            {
                case EnumClass.ItemPurchaseState.NotBought:
                    buttonsParent.GetChild(i - lastCount).GetComponent<ArenaButton>().button.interactable = false;
                    break;
                case EnumClass.ItemPurchaseState.Bought:
                    buttonsParent.GetChild(i - lastCount).GetComponent<ArenaButton>().Setup(FactoryManager.Instance.itemsFactory.GetItems(EnumClass.ItemType.Level)[i].iconImage);
                    buttonsParent.GetChild(i - lastCount).GetComponent<ArenaButton>().button.interactable = true;
                    break;
                case EnumClass.ItemPurchaseState.ComingSoon:
                    buttonsParent.GetChild(i - lastCount).GetComponent<ArenaButton>().button.interactable = false;
                    break;
                default:
                    break;
            }

            buttonsParent.GetChild(i - lastCount).GetComponent<ArenaButton>().gameObject.SetActive(true);
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

    private void InitializePaginations()
    {
        for (int i = 0; i < paginations.Count; i++)
        {
            var index = i;
            paginations[index].onClick.AddListener(() =>
            {
                EventManager.Instance.Click();
                TurnOnOnePaginations(index);
                lastCount = index * 8;
                LoadArenaData();
                DeactivateHighlight();
            });
        }
    }

    private void TurnOnOnePaginations(int index)
    {
        for (int i = 0; i < paginations.Count; i++)
        {
            paginations[i].transform.GetChild(0).gameObject.SetActive(i == index);
        }
    }

    private void DeactivateHighlight()
    {
        for (int i = 0; i < 8; i++)
        {
            buttonsParent.GetChild(i).GetComponent<ArenaButton>().DeactivateHighlight();
        }
    }
}
