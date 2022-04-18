using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : Menu
{
    public Button back, save, restore;
    public List<Models.SettingsData> settingsDatas = new List<Models.SettingsData>();
    bool isLoaded;

    private void Start()
    {
        back.onClick.AddListener(() => LoadHomeMenu());
        save.onClick.AddListener(() => SaveChangedSettings());
        restore.onClick.AddListener(() => RestoreSettings());
        EventManager.Instance.OnSettingTypeChange += AddSettingsData;
        LoadSavedData();
    }

    public void LoadHomeMenu()
    {
        EventManager.Instance.Click();
        List<string> menuTagNames = new List<string>();
        menuTagNames = previousMenuName;
        LoadMenu(back, menuTagNames);

        SaveChangedSettings();
    }

    private void OnEnable()
    {
        if(isLoaded)
            LoadSavedData();
    }

    public void AddSettingsData(EnumClass.SettingsType settingsType, int selectionID)
    {
        for (int i = 0; i < settingsDatas.Count; i++)
        {
            if(settingsDatas[i].settingsType == settingsType)
            {
                Models.SettingsData settingData = new Models.SettingsData();
                settingData.settingsType = settingsType;
                settingData.id = selectionID;
                settingData.buttonsParent = settingsDatas[i].buttonsParent;
                settingsDatas[i] = settingData;

                //for (int j = 0; j < settingsDatas[i].buttonsParent.childCount; j++)
                //{
                //    if(j != selectionID)
                //        settingsDatas[i].buttonsParent.GetChild(j).GetComponent<SettingsButton>().DeactivateHighlight();
                //    else
                //        settingsDatas[i].buttonsParent.GetChild(j).GetComponent<SettingsButton>().ActivateHighlight();
                //}
            }
        }
    }

    public void LoadSavedData()
    {
        for (int i = 0; i < settingsDatas.Count; i++)
        {
            Models.SettingsData settingData = new Models.SettingsData();
            settingData.settingsType = settingsDatas[i].settingsType;
            settingData.id = PlayerPrefs.GetInt(settingsDatas[i].settingsType.ToString(), 0);
            settingData.buttonsParent = settingsDatas[i].buttonsParent;
            settingsDatas[i] = settingData;

            //for (int j = 0; j < settingsDatas[i].buttonsParent.childCount; j++)
            //{
            //    if (j != PlayerPrefs.GetInt(settingsDatas[i].settingsType.ToString(), 0))
            //        settingsDatas[i].buttonsParent.GetChild(j).GetComponent<SettingsButton>().DeactivateHighlight();
            //    else
            //        settingsDatas[i].buttonsParent.GetChild(j).GetComponent<SettingsButton>().ActivateHighlight();
            //}
        }
        isLoaded = true;
    }

    private void SaveChangedSettings()
    {
        //EventManager.Instance.Click();
        for (int i = 0; i < settingsDatas.Count; i++)
        {
            PlayerPrefs.SetInt(settingsDatas[i].settingsType.ToString(), settingsDatas[i].id);
        }
        EventManager.Instance.SettingsDataChanged();
    }

    private void RestoreSettings()
    {
        EventManager.Instance.Click();
        for (int i = 0; i < settingsDatas.Count; i++)
        {
            PlayerPrefs.SetInt(settingsDatas[i].settingsType.ToString(), 0);

            for (int j = 0; j < settingsDatas[i].buttonsParent.childCount; j++)
            {
                if (j != PlayerPrefs.GetInt(settingsDatas[i].settingsType.ToString(), 0))
                    settingsDatas[i].buttonsParent.GetChild(j).GetComponent<SettingsButton>().DeactivateHighlight();
                else
                    settingsDatas[i].buttonsParent.GetChild(j).GetComponent<SettingsButton>().ActivateHighlight();
            }
        }
        EventManager.Instance.SettingsDataChanged();
    }
}
