using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ShopMenu : Menu
{
    public Button back, setting;
    public GameObject characterTab, skillTab, arenaTab;
    public GameObject characterContentParent, skillContentParent, arenaContentParent;
    public Color highlight, notHighlight;
    public Text coinDisplay;
    public EnumClass.ItemType currentItemDisplay;
    public List<Button> buttons = new List<Button>();
    public List<Models.ShopDisplayData> shopDisplayDatas = new List<Models.ShopDisplayData>();
    public Text activeItemDisplay;

    private List<Models.ShopDisplayData> availableDisplayTabs = new List<Models.ShopDisplayData>();

    private void Start()
    {
        back.onClick.AddListener(() => LoadHomeMenu());
        setting.onClick.AddListener(() => LoadSettingMenu());
        SetDisplayCallback();

        LoadCharacterData();
        LoadSkillData();
        LoadArenaData();

        for (int i = 0; i < buttons.Count; i++)
        {
            int index = i;
            buttons[i].onClick.AddListener(() => SwitchTabs(index));
        }
    }

    private void OnEnable()
    {
        //UpdateCoinText();
    }

    private void SetDisplayCallback()
    {
        shopDisplayDatas.Where(x => x.itemType == EnumClass.ItemType.Character).FirstOrDefault().callback = LoadCharacterTab;
        shopDisplayDatas.Where(x => x.itemType == EnumClass.ItemType.Level).FirstOrDefault().callback = LoadArenaTab;
        shopDisplayDatas.Where(x => x.itemType == EnumClass.ItemType.Skill).FirstOrDefault().callback = LoadSkillTab;

        SwapTabs(EnumClass.ItemType.Character);
    }

    private void SwapTabs(EnumClass.ItemType itemType)
    {
        activeItemDisplay.text = itemType.ToString() + " Store"; 
        shopDisplayDatas.Where(x => x.itemType == itemType).FirstOrDefault().callback?.Invoke();

        availableDisplayTabs.Clear();

        for (int i = 0; i < shopDisplayDatas.Count; i++)
        {
            if(shopDisplayDatas[i].itemType != itemType)
            {
                availableDisplayTabs.Add(shopDisplayDatas[i]);
            }
        }

        for (int i = 0; i < buttons.Count; i++)
        {
            buttons[i].image.sprite = availableDisplayTabs[i].image;
        }

        currentItemDisplay = itemType;
    }

    private void SwitchTabs(int index)
    {
        EventManager.Instance.Click();

        if (index == 0)
        {
            SwapTabs(availableDisplayTabs[0].itemType);
        }
        else if (index == 1)
        {
            SwapTabs(availableDisplayTabs[1].itemType);
        }
    }

    public void LoadSettingMenu()
    {
        EventManager.Instance.Click();
        List<string> menuTagNames = new List<string>();
        menuTagNames.Add("header");
        menuTagNames.Add("settings");
        LoadMenu(setting, menuTagNames);
    }

    private void LoadHomeMenu()
    {
        EventManager.Instance.Click();
        List<string> menuTagNames = new List<string>();
        menuTagNames.Add("header");
        menuTagNames.Add("home");
        LoadMenu(back, menuTagNames);
    }

    private void LoadCharacterTab()
    {
        TurnOffTabs();
        characterTab.SetActive(true);
    }

    private void LoadSkillTab()
    {
        TurnOffTabs();
        skillTab.SetActive(true);
    }

    private void LoadArenaTab()
    {
        TurnOffTabs();
        arenaTab.SetActive(true);
    }

    private void TurnOffTabs()
    {
        characterTab.SetActive(false);
        skillTab.SetActive(false);
        arenaTab.SetActive(false);
    }

    private void LoadCharacterData()
    {
        for (int i = 0; i < FactoryManager.Instance.itemsFactory.GetItems(EnumClass.ItemType.Character).Count; i++)
        {
            characterContentParent.transform.GetChild(i).GetComponent<CharacterContentModel>().SetUp(FactoryManager.Instance.itemsFactory.GetItems(EnumClass.ItemType.Character)[i]);
        }
    }

    private void LoadSkillData()
    {
        for (int i = 0; i < FactoryManager.Instance.itemsFactory.GetItems(EnumClass.ItemType.Skill).Count; i++)
        {
            skillContentParent.transform.GetChild(i).GetComponent<SkillContentModel>().SetUp(FactoryManager.Instance.itemsFactory.GetItems(EnumClass.ItemType.Skill)[i]);
        }
    }

    private void LoadArenaData()
    {
        for (int i = 0; i < FactoryManager.Instance.itemsFactory.GetItems(EnumClass.ItemType.Level).Count; i++)
        {
            arenaContentParent.transform.GetChild(i).GetComponent<ArenaContentModel>().SetUp(FactoryManager.Instance.itemsFactory.GetItems(EnumClass.ItemType.Level)[i]);
        }
    }

    private void UpdateCoinText()
    {
        StartCoroutine(LoadCoinValue());
    }

    IEnumerator LoadCoinValue()
    {
        using (UnityWebRequest www = UnityWebRequest.Get(ApiConstants.apiBaseUrl + "/api/v2/wallet"))
        {
            www.SetRequestHeader("x-auth-token", PlayerPrefs.GetString("TokenID"));
            www.SetRequestHeader("alpha-sec-key", ApiConstants.alphaSecKey);

            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
                Debug.Log(www.downloadHandler.text);

                Debug.Log("check internet connection and try again");
            }
            else
            {
                Debug.Log("Form upload complete!");

                Debug.Log(www.downloadHandler.text);

                JSONNode jsondata = JSON.Parse(System.Text.Encoding.UTF8.GetString(www.downloadHandler.data));

                coinDisplay.text = jsondata["data"]["balance"];
            }
        }
    }
}
