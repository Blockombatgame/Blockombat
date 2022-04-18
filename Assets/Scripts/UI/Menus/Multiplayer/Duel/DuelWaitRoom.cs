using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class DuelWaitRoom : Menu
{
    public GameObject guestJoin, textWaiting, loading;
    public Text errorText;
    public Button back;
    private bool isLoaded = false;

    private void Start()
    {
        errorText.text = "";

        EventManager.Instance.OnWaitRoomLoading += LoadGame;

        StartCoroutine(RejectionCheck());
        isLoaded = true;

        back.onClick.AddListener(() => StartCoroutine(DeleteDuel()));
    }

    private void OnEnable()
    {
        if(isLoaded)
        {
            errorText.text = "";
            StartCoroutine(RejectionCheck());
        }
    }

    public void LoadGame()
    {
        StopAllCoroutines();
        textWaiting.SetActive(false);
        back.gameObject.SetActive(false);
        guestJoin.SetActive(true);
        loading.SetActive(true);
        StartCoroutine(DelayLoad());
    }

    public void LoadCharacterSelectMenu()
    {
        List<string> menuTagNames = new List<string>();
        menuTagNames.Add("header");
        menuTagNames.Add("multiplayerCharacterSelect");
        LoadMenu(null, menuTagNames);
    }
    
    public void LoadDuelHomeMenu()
    {
        List<string> menuTagNames = new List<string>();
        menuTagNames.Add("header");
        menuTagNames.Add("duelHome");
        LoadMenu(null, menuTagNames);
    }

    IEnumerator DelayLoad()
    {
        yield return new WaitForSeconds(0.5f);
        LoadCharacterSelectMenu();
    }

    IEnumerator RejectionCheck()
    {
        yield return new WaitForSeconds(1f);
        StartCoroutine(RejectQuery());
    }

    IEnumerator RejectQuery()
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

                string a = jsondata["data"]["isRejected"].ToString();

                if (a == "false")
                {
                    StartCoroutine(RejectionCheck());
                }
                else if (a == "true")
                {
                    //delete duel and send to duel home
                    errorText.text = "duel declined";
                    yield return new WaitForSeconds(1f);
                    StartCoroutine(DeleteDuel());
                }
            }
        }
    }

    IEnumerator DeleteDuel()
    {
        using (UnityWebRequest www = UnityWebRequest.Delete("https://backend.alphakombat.com/api/v2/duel/" + PlayerPrefs.GetString("duelID")))
        {
            www.SetRequestHeader("x-auth-token", PlayerPrefs.GetString("TokenID"));
            www.SetRequestHeader("alpha-sec-key", "f55da6945d6b8676eff0ae15690cc260d3c64d31a8aa7c6ffb665b855aecd80b5b2a1331a3868a8e11289771f3614d0d");

            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
                //Debug.Log(www.downloadHandler.text);

                Debug.Log("check internet connection and try again");

                //errorText.text = www.downloadHandler.text;
            }
            else
            {
                Debug.Log("Delete complete!");

                //Debug.Log(www.downloadHandler.text);

                MultiplayerLauncher.Instance.LeaveRoom();
                LoadDuelHomeMenu();
            }
        }
    }
}
