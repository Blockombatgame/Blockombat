using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class SoloMenu : Menu
{
    public Button playToEarn, freeGame, back;
    public Text errorText;

    private void Start()
    {
        playToEarn.onClick.AddListener(() => PlayToEarn());
        freeGame.onClick.AddListener(() => FreeGame());
        back.onClick.AddListener(() => ExitBack());
    }

    private void PlayToEarn()
    {
        EventManager.Instance.Click();
        PlayerPrefs.SetString("SoloType", "Earn");
        StartCoroutine(GetEarnAmount(playToEarn));
    }

    private void FreeGame()
    {
        EventManager.Instance.Click();
        PlayerPrefs.SetString("SoloType", "Free");
        LoadCharacterMenu(freeGame);
    }

    private void ExitBack()
    {
        EventManager.Instance.Click();
        LoadHomeMenu();
    }

    private void LoadHomeMenu()
    {
        List<string> menuTagNames = new List<string>();
        menuTagNames.Add("header");
        menuTagNames.Add("home");
        LoadMenu(back, menuTagNames);
    }

    private void LoadCharacterMenu(Button button)
    {
        EventManager.Instance.Click();
        List<string> menuTagNames = new List<string>();
        menuTagNames.Add("header");
        menuTagNames.Add("character");
        LoadMenu(button, menuTagNames);
    }

    IEnumerator ProcessPayment(Button button)
    {
        WWWForm form = new WWWForm();
        form.AddField("amount", PlayerPrefs.GetFloat("EarnValue").ToString());
        form.AddField("walletAddress", PlayerPrefs.GetString("WalletID"));
        form.AddField("hash", "ForPlayToEarnEntry");

        using (UnityWebRequest www = UnityWebRequest.Post(ApiConstants.apiBaseUrl + "/api/v2/wallet/withdraw", form))
        {
            www.SetRequestHeader("x-auth-token", PlayerPrefs.GetString("TokenID"));
            www.SetRequestHeader("alpha-sec-key", ApiConstants.alphaSecKey);

            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
                Debug.Log(www.downloadHandler.text);

                Debug.Log("check internet connection and try again");
                errorText.text = "check internet connection and try again";
                button.interactable = true;
            }
            else
            {
                Debug.Log("Form upload complete!");

                Debug.Log(www.downloadHandler.text);
                JSONNode jsondata = JSON.Parse(System.Text.Encoding.UTF8.GetString(www.downloadHandler.data));


                if (jsondata["message"] != "insufficient fund")
                {
                    LoadCharacterMenu(playToEarn);
                }
                else
                {
                    errorText.text = jsondata["message"];
                }

                button.interactable = true;
            }
        }
    }

    IEnumerator GetEarnAmount(Button button)
    {
        using (UnityWebRequest www = UnityWebRequest.Get(ApiConstants.apiBaseUrl + "/api/v2/earn/alkom"))
        {
            www.SetRequestHeader("x-auth-token", PlayerPrefs.GetString("TokenID"));
            www.SetRequestHeader("alpha-sec-key", ApiConstants.alphaSecKey);

            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
                Debug.Log(www.downloadHandler.text);

                //errorText.text = "check internet connection and try again";
            }
            else
            {
                Debug.Log("earn form sent!");

                Debug.LogError(www.downloadHandler.text);

                JSONNode jsondata = JSON.Parse(System.Text.Encoding.UTF8.GetString(www.downloadHandler.data));

                PlayerPrefs.SetFloat("EarnValue", jsondata["data"]);

                StartCoroutine(ProcessPayment(button));
            }
        }
    }
}
