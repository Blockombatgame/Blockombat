using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class HeaderMenu : Menu
{
    public Button login;
    public Button setting;
    public Text coinDisplay, userNameDisplay;

    private void Start()
    {
        login.onClick.AddListener(() => LoadLoginMenu());
        setting.onClick.AddListener(() => LoadSettingMenu());

        if(PlayerPrefs.GetString("GameStart") == "loggedIn")
        {
            login.gameObject.SetActive(false);
            userNameDisplay.text = PlayerPrefs.GetString("Username");
        }
        else
        {
            userNameDisplay.gameObject.SetActive(false);
        }


    }

    private void OnEnable()
    {
        UpdateCoinText();
    }

    private void LoadLoginMenu()
    {
        EventManager.Instance.Click();
        List<string> menuTagNames = new List<string>();
        menuTagNames.Add("login");
        LoadMenu(login, menuTagNames);
    }

    public void LoadSettingMenu()
    {
        EventManager.Instance.Click();
        List<string> menuTagNames = new List<string>();
        menuTagNames.Add("header");
        menuTagNames.Add("settings");
        LoadMenu(setting, menuTagNames);
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

                //if(jsondata["message"] == "no score yet")
                //{
                //    Debug.Log("no score yet ooo");
                //    coinDisplay.text = "0";
                //}
            }
        }
    }
}
