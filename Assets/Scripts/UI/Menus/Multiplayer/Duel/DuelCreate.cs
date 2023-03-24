using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class DuelCreate : Menu
{
    public InputField matchName, otherPlayerName, wageAmount;
    public Text errorText;
    public Button back, proceed;

    private void Start()
    {
        errorText.text = "";

        back.onClick.AddListener(() => LoadDuelHomeMenu());
        proceed.onClick.AddListener(() => CreateRoom());

        MultiplayerLauncher.Instance.PhotonNetworkRoomCreated += LoadWaitingRoomMenu;
    }

    private void OnEnable()
    {
        errorText.text = "";
    }

    private void LoadDuelHomeMenu()
    {
        List<string> menuTagNames = new List<string>();
        menuTagNames.Add("header");
        menuTagNames.Add("duelHome");
        LoadMenu(back, menuTagNames);
    }

    private void LoadWaitingRoomMenu(string empty)
    {
        List<string> menuTagNames = new List<string>();
        menuTagNames.Add("duelWaitRoom");
        LoadMenu(proceed, menuTagNames);
    }

    public void LoadLoadingMenu()
    {
        List<string> menuTagNames = new List<string>();
        menuTagNames.Add("wait");
        LoadMenu(null, menuTagNames);
    }

    private void CreateRoom()
    {
        EventManager.Instance.Click();

        StartCoroutine(CreateDuelOnServer());
    }

    IEnumerator CreateDuelOnServer()
    {
        WWWForm form = new WWWForm();
        form.AddField("stake", wageAmount.text);
        form.AddField("invitedPlayer", otherPlayerName.text);
        form.AddField("roomName", matchName.text);

        using (UnityWebRequest www = UnityWebRequest.Post(ApiConstants.apiBaseUrl + "/api/v2/duel", form))
        {
            www.SetRequestHeader("x-auth-token", PlayerPrefs.GetString("TokenID"));
            www.SetRequestHeader("alpha-sec-key", ApiConstants.alphaSecKey);

            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
                Debug.Log(www.downloadHandler.text);

                errorText.text = www.downloadHandler.text;
                proceed.interactable = true;
            }
            else
            {
                Debug.Log("Duel Creation complete!");

                Debug.LogError(www.downloadHandler.text);

                JSONNode jsondata = JSON.Parse(System.Text.Encoding.UTF8.GetString(www.downloadHandler.data));

                PlayerPrefs.SetString("duelID", jsondata["data"]["id"]);

                PlayerPrefs.SetString("stake", jsondata["data"]["hostPlayer_stake"]);
                PlayerPrefs.SetString("hostPlayerID", jsondata["data"]["hostPlayer"]);
                PlayerPrefs.SetString("invitedPlayerID", jsondata["data"]["invitedPlayer"]);

                PlayerPrefs.SetString("MultiplayerMode", "Duel Mode");

                MultiplayerLauncher.Instance.CreateRoom(matchName.text);
                LoadLoadingMenu();
            }
        }
    }
}
