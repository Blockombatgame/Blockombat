using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class LoginMenu : Menu
{
    public InputField walletID;
    public Button register, back;
    public Text errorText;

    private void Start()
    {
        register.onClick.AddListener(() => RegisterWalletID());
        back.onClick.AddListener(() => LoadBackHomeMenu());
    }

    public void RegisterWalletID()
    {
        register.interactable = false;
        if(PlayerPrefs.GetString("WalletID") == "")
        {
            if (walletID.text.ToString() != "")
            {
                StartCoroutine(LoginWallet());
            }
            else
            {
                errorText.text = "Field is empty. Fill in the field.";
                register.interactable = true;
            }
        }
        else
        {
            errorText.text = "You can only have one account logged In. You cannot register again.";
            register.interactable = true;
        }
    }

    private void LoadHomeMenu()
    {
        EventManager.Instance.Click();
        List<string> menuTagNames = new List<string>();
        menuTagNames = previousMenuName;
        LoadMenu(register, menuTagNames);
    }

    private void LoadBackHomeMenu()
    {
        EventManager.Instance.Click();
        List<string> menuTagNames = new List<string>();
        menuTagNames = previousMenuName;
        LoadMenu(back, menuTagNames);
    }

    IEnumerator LoginWallet()
    {
        WWWForm form = new WWWForm();
        form.AddField("walletId", walletID.text);

        using (UnityWebRequest www = UnityWebRequest.Post("https://backend.alphakombat.com/api/v2/auth/login", form))
        {
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
                Debug.Log(www.downloadHandler.text);

                errorText.text = "check internet connection and try again";
                register.interactable = true;
            }
            else
            {
                Debug.Log("Form upload complete!");
                PlayerPrefs.SetFloat("Logged In", 1);

                Debug.Log(www.downloadHandler.text);

                JSONNode jsondata = JSON.Parse(System.Text.Encoding.UTF8.GetString(www.downloadHandler.data));

                string jsonArray = www.downloadHandler.text;

                PlayerPrefs.SetString("Username", jsondata["data"]["username"]);

                PlayerPrefs.SetString("playerId", jsondata["data"]["id"]);

                PlayerPrefs.SetString("TokenID", jsondata["data"]["token"]);

                PlayerPrefs.SetString("WalletID", walletID.text.ToString());

                LoadHomeMenu();

                register.interactable = true;
            }
        }
    }
}
