using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class DuelInvitesMenu : Menu
{
    public Button back;
    public Transform duelListParent;
    public Text errorText;
    public float duelListParentX;
    public Transform empty;

    private void Start()
    {
        duelListParentX = duelListParent.GetComponent<RectTransform>().sizeDelta.x;
        back.onClick.AddListener(() => BackCallback());
    }

    private void OnEnable()
    {
        StartCoroutine(DuelListQuery());
    }

    private void BackCallback()
    {
        DisableDuelUIData();
        EventManager.Instance.Click();
        List<string> menuTagNames = new List<string>();
        menuTagNames.Add("header");
        menuTagNames.Add("duelHome");
        LoadMenu(back, menuTagNames);
    }

    private void OpenLoadMenu()
    {
        MenuManager.Instance.OpenMenu("popMenu", "wait");
    }

    private void OnDisable()
    {
        DisableDuelUIData();
        errorText.text = "";
    }

    IEnumerator DuelListQuery()
    {
        using (UnityWebRequest www = UnityWebRequest.Get("https://backend.alphakombat.com/api/v2/duel"))
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
                Debug.Log("Duel Invite list form sent!");

                //Debug.LogError(www.downloadHandler.text);

                JSONNode jsondata = JSON.Parse(System.Text.Encoding.UTF8.GetString(www.downloadHandler.data));

                //Debug.Log(jsondata["data"][0]);


                for (int i = 0; i < jsondata["data"].Count; i++)
                {
                    //Debug.Log("succesful");
                    if (!jsondata["data"][i]["isRejected"])
                    {
                        if (i > 4)
                            duelListParent.GetComponent<RectTransform>().sizeDelta = new Vector2(duelListParent.GetComponent<RectTransform>().sizeDelta.x + 500f, duelListParent.GetComponent<RectTransform>().sizeDelta.y);

                        DuelInviteContentModel duelInviteContentModel = FactoryManager.Instance.prefabsFactory.GetItem(FactoryManager.Instance.prefabsFactory.duelInviteContentModel);
                        duelInviteContentModel.transform.SetParent(duelListParent);
                        duelInviteContentModel.transform.localPosition = Vector3.zero;
                        duelInviteContentModel.transform.localScale = Vector3.one;
                        duelInviteContentModel.SetUp(jsondata["data"][i]["host"]["username"], jsondata["data"][i]["DuelStake"]["hostPlayer_stake"], jsondata["data"][i]["host"]["id"], jsondata["data"][i]["id"]);

                        duelInviteContentModel.RegisterCallback(() => StartCoroutine(JoinDuelMultiplayerOnServer(duelInviteContentModel.stakeAmount)), () => DeclineDuel());
                    }
                }

                if (duelListParent.childCount == 0)
                {
                    errorText.text = "No Duel Invite For You.";
                }
            }
        }
    }

    IEnumerator JoinDuelMultiplayerOnServer(string stakeAmount)
    {
        WWWForm form = new WWWForm();
        form.AddField("stake", stakeAmount);
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

                StartCoroutine(ProcessPayment(null));
            }
        }
    }

    private void DeclineDuel()
    {
        StartCoroutine(RejectDuel());
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
                }
                else
                {
                    errorText.text = jsondata["message"];
                }

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

                DisableDuelUIData();
                DuelListQuery();
            }
        }
    }

    private void DisableDuelUIData()
    {
        for (int i = 0; i < duelListParent.childCount; i++)
        {
            DuelInviteContentModel duelInviteContentModel = duelListParent.GetChild(0).GetComponent<DuelInviteContentModel>();

            if (duelInviteContentModel != null)
            {
                FactoryManager.Instance.prefabsFactory.Recycle(duelInviteContentModel);
            }

            duelInviteContentModel.transform.SetParent(empty);
        }

        duelListParent.GetComponent<RectTransform>().sizeDelta = new Vector2(duelListParentX, duelListParent.GetComponent<RectTransform>().sizeDelta.y);
    }
}
