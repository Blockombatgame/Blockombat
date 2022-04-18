using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class DuelStartMenu : Menu
{
    public Button create, join, back;
    public Text errorText;
    private bool loaded = false, menuActive = false;

    private void Start()
    {
        errorText.text = "";
        loaded = true;
        back.onClick.AddListener(() => ExitDuel() );
        create.onClick.AddListener(() => LoadCreateMenu() );
        LoadPhotonConnection();
        MultiplayerLauncher.Instance.PhotonNetworkJoinedLobby += LoadDuelMenu;
        MultiplayerLauncher.Instance.PhotonNetworkEnded += DisconnectPhotonConnection;
        MultiplayerLauncher.Instance.PhotonNetworkDisconnected += LoadLoadingMenu;
        MultiplayerLauncher.Instance.PhotonNetworkEnteredRoom += LoadWaitingRoomMenu;

        join.onClick.AddListener(() => JoinRoomLogic());

    }

    private void OnEnable()
    {
        if (MultiplayerLauncher.Instance.connectionState == EnumClass.ConnectionState.NotConnected && loaded)
        {
            //MultiplayerLauncher.Instance.PhotonNetworkJoinedLobby += LoadDuelMenu;
            MultiplayerLauncher.Instance.PhotonNetworkEnded += DisconnectPhotonConnection;
            LoadPhotonConnection();
        }
        errorText.text = "";
        join.interactable = true;
        create.interactable = true;
        back.interactable = true;

        menuActive = true;
    }

    private void OnDisable()
    {
        menuActive = false;
    }

    private void LoadPhotonConnection()
    {
        LoadLoadingMenu();
        MultiplayerLauncher.Instance.StartMultiplayer();
    }

    private void DisconnectPhotonConnection()
    {
        LoadHomeMenu();
        //MultiplayerLauncher.Instance.PhotonNetworkEnded -= LoadHomeMenu;
    }

    private void ExitDuel()
    {
        EventManager.Instance.Click();
        LoadLoadingMenu();
        //MultiplayerLauncher.Instance.PhotonNetworkJoinedLobby -= LoadDuelMenu;
        MultiplayerLauncher.Instance.ExitMultiplayer();
    }

    private void LoadDuelMenu()
    {
        if (menuActive)
        {
            List<string> menuTagNames = new List<string>();
            menuTagNames.Add("header");
            menuTagNames.Add("duelHome");
            LoadMenu(null, menuTagNames);
        }
        else
        {
            MenuManager.Instance.OpenPreviousMenu();
        }
    }

    private void LoadHomeMenu()
    {
        List<string> menuTagNames = new List<string>();
        menuTagNames.Add("header");
        menuTagNames.Add("home");
        LoadMenu(back, menuTagNames);
    }

    public void LoadLoadingMenu()
    {
        List<string> menuTagNames = new List<string>();
        menuTagNames.Add("wait");
        LoadMenu(null, menuTagNames);
    }
    
    private void LoadCreateMenu()
    {
        EventManager.Instance.Click();
        List<string> menuTagNames = new List<string>();
        menuTagNames.Add("header");
        menuTagNames.Add("duelCreate");
        LoadMenu(create, menuTagNames);
    }
    
    private void LoadDeulInviteMenu()
    {
        EventManager.Instance.Click();
        List<string> menuTagNames = new List<string>();
        menuTagNames.Add("header");
        menuTagNames.Add("duelInvite");
        LoadMenu(create, menuTagNames);
    }

    private void LoadWaitingRoomMenu()
    {
        create.interactable = true;
        join.interactable = true;
        back.interactable = true;
        List<string> menuTagNames = new List<string>();
        menuTagNames.Add("header");
        menuTagNames.Add("duelWaitRoom");
        LoadMenu(create, menuTagNames);
    }

    private void JoinRoomLogic()
    {
        errorText.text = "";
        //StartCoroutine(ConfirmJoinDuelOnServer());
        LoadDeulInviteMenu();
    }

    IEnumerator ConfirmJoinDuelOnServer()
    {
        WWWForm form = new WWWForm();
        //form.AddField("stake", "20");
        form.AddField("invitedPlayerId", PlayerPrefs.GetString("playerId"));

        using (UnityWebRequest www = UnityWebRequest.Post("https://backend.alphakombat.com/api/v2/duel/join", form))
        {
            www.SetRequestHeader("x-auth-token", PlayerPrefs.GetString("TokenID"));

            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
                Debug.Log(www.downloadHandler.text);
                errorText.text = www.downloadHandler.text;
                join.interactable = true;
            }
            else
            {
                JSONNode jsondata = JSON.Parse(System.Text.Encoding.UTF8.GetString(www.downloadHandler.data));

                PlayerPrefs.SetString("stakeAmount", jsondata["data"]["hostPlayer_stake"]);
                PlayerPrefs.SetString("duelID", jsondata["data"]["id"]);

                Debug.Log("Join Data retrieval complete!");

                //SetPopUpMenu();
                StartCoroutine(GetDuelHostName());

                Debug.Log(www.downloadHandler.text);
            }
        }
    }

    IEnumerator GetDuelHostName()
    {
        using (UnityWebRequest www = UnityWebRequest.Get("https://backend.alphakombat.com/api/v2/duel/" + PlayerPrefs.GetString("duelID")))
        {
            www.SetRequestHeader("x-auth-token", PlayerPrefs.GetString("TokenID"));
            www.SetRequestHeader("alpha-sec-key", "f55da6945d6b8676eff0ae15690cc260d3c64d31a8aa7c6ffb665b855aecd80b5b2a1331a3868a8e11289771f3614d0d");

            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
                Debug.Log(www.downloadHandler.text);

                //errorText.text = "check internet connection and try again";
            }
            else
            {
                Debug.Log("Reject form sent!");

                Debug.LogError(www.downloadHandler.text);

                JSONNode jsondata = JSON.Parse(System.Text.Encoding.UTF8.GetString(www.downloadHandler.data));

                PlayerPrefs.SetString("hostName", jsondata["data"]["host"]["username"].ToString());

                SetPopUpMenu();
            }
        }
    }

    private void SetPopUpMenu()
    {
        EventManager.Instance.OpenPopMenuAction(() => StartCoroutine(JoinDuelMultiplayerOnServer()), () => DeclineDuel(), PlayerPrefs.GetString("hostName"), PlayerPrefs.GetString("stakeAmount"));
    }

    private void DeclineDuel()
    {
        create.interactable = true;
        join.interactable = true;
        back.interactable = true;

        StartCoroutine(RejectDuel());
        EventManager.Instance.ClosePopMenu();
    }

    private void OpenLoadMenu()
    {
        MenuManager.Instance.OpenMenu("popMenu", "wait");
    }

    IEnumerator JoinDuelMultiplayerOnServer()
    {
        WWWForm form = new WWWForm();
        form.AddField("stake", PlayerPrefs.GetString("stakeAmount"));
        form.AddField("invitedPlayerId", PlayerPrefs.GetString("playerId"));

        using (UnityWebRequest www = UnityWebRequest.Post("https://backend.alphakombat.com/api/v2/duel/join", form))
        {
            www.SetRequestHeader("x-auth-token", PlayerPrefs.GetString("TokenID"));
            www.SetRequestHeader("alpha-sec-key", "f55da6945d6b8676eff0ae15690cc260d3c64d31a8aa7c6ffb665b855aecd80b5b2a1331a3868a8e11289771f3614d0d");

            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
                Debug.Log(www.downloadHandler.text);
                errorText.text = www.downloadHandler.text;

                join.interactable = true;
            }
            else
            {
                JSONNode jsondata = JSON.Parse(System.Text.Encoding.UTF8.GetString(www.downloadHandler.data));

                PlayerPrefs.SetString("duelRoomName", jsondata["data"]["roomName"]);
                PlayerPrefs.SetString("duelID", jsondata["data"]["id"]);

                PlayerPrefs.SetString("stake", jsondata["data"]["hostPlayer_stake"]);
                PlayerPrefs.SetString("hostPlayerID", jsondata["data"]["hostPlayer"]);
                PlayerPrefs.SetString("invitedPlayerID", jsondata["data"]["invitedPlayer"]);

                Debug.Log("Join Data retrieval complete!");

                Debug.Log(www.downloadHandler.text);
                errorText.text = www.downloadHandler.text;

                StartCoroutine(ProcessPayment(join));
            }
        }
    }

    IEnumerator ProcessPayment(Button button)
    {
        WWWForm form = new WWWForm();
        form.AddField("amount", PlayerPrefs.GetString("stake"));
        form.AddField("walletAddress", PlayerPrefs.GetString("WalletID"));
        form.AddField("hash", "ForDuelCreation");

        using (UnityWebRequest www = UnityWebRequest.Post("https://backend.alphakombat.com/api/v2/wallet/withdraw", form))
        {
            www.SetRequestHeader("x-auth-token", PlayerPrefs.GetString("TokenID"));
            www.SetRequestHeader("alpha-sec-key", "f55da6945d6b8676eff0ae15690cc260d3c64d31a8aa7c6ffb665b855aecd80b5b2a1331a3868a8e11289771f3614d0d");

            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
                Debug.Log(www.downloadHandler.text);

                Debug.Log("check internet connection and try again");
                errorText.text = "check internet connection and try again";
                StartCoroutine(RejectDuel());
            }
            else
            {
                Debug.Log("Form upload complete!");

                Debug.Log(www.downloadHandler.text);

                JSONNode jsondata = JSON.Parse(System.Text.Encoding.UTF8.GetString(www.downloadHandler.data));


                if (jsondata["message"] != "insufficient fund")
                {
                    PlayerPrefs.SetString("MultiplayerMode", "Duel Mode");

                    OpenLoadMenu();

                    yield return new WaitForSeconds(0.05f);

                    MultiplayerLauncher.Instance.JoinRoom(PlayerPrefs.GetString("duelRoomName"));
                    create.interactable = false;
                    join.interactable = false;
                    back.interactable = false;
                }
                else
                {
                    errorText.text = jsondata["message"];
                }

                button.interactable = true;
            }
        }
    }

    IEnumerator DeleteDuel(Button button)
    {
        using (UnityWebRequest www = UnityWebRequest.Delete("https://backend.alphakombat.com/api/v2/duel/" + PlayerPrefs.GetString("duelID")))
        {
            www.SetRequestHeader("x-auth-token", PlayerPrefs.GetString("TokenID"));
            www.SetRequestHeader("alpha-sec-key", "f55da6945d6b8676eff0ae15690cc260d3c64d31a8aa7c6ffb665b855aecd80b5b2a1331a3868a8e11289771f3614d0d");

            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
                Debug.Log(www.downloadHandler.text);

                Debug.Log("check internet connection and try again");
                button.interactable = true;
            }
            else
            {
                Debug.Log("Delete complete!");

                Debug.Log(www.downloadHandler.text);

                button.interactable = true;
            }
        }
    }
    
    IEnumerator RejectDuel()
    {
        WWWForm form = new WWWForm();
        form.AddField("invitedPlayer", PlayerPrefs.GetString("Username"));


        using (UnityWebRequest www = UnityWebRequest.Post("https://backend.alphakombat.com/api/v2/duel/reject", form))
        {
            www.SetRequestHeader("x-auth-token", PlayerPrefs.GetString("TokenID"));
            www.SetRequestHeader("alpha-sec-key", "f55da6945d6b8676eff0ae15690cc260d3c64d31a8aa7c6ffb665b855aecd80b5b2a1331a3868a8e11289771f3614d0d");

            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
                Debug.Log(www.downloadHandler.text);

                Debug.Log("check internet connection and try again");
            }
            else
            {
                Debug.Log("duel rejected!");

                Debug.Log(www.downloadHandler.text);
            }
        }
    }
}
