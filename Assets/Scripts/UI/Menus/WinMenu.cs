using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class WinMenu : Menu
{
    public Button loadButton;
    public GameObject amountWonText;
    public bool multiplayerMode;

    private void Start()
    {
        if (!multiplayerMode)
        {
            loadButton.onClick.AddListener(() => LoadStartLevel());

            if (PlayerPrefs.GetString("SoloType") == "Earn")
            {
                loadButton.interactable = false;
                amountWonText.GetComponent<Text>().text = "You win + " + PlayerPrefs.GetFloat("EarnValue").ToString() + " ALKOM token";
                StartCoroutine(ProcessPayment(loadButton));
            }
            else if (PlayerPrefs.GetString("SoloType") == "Free")
            {
                loadButton.interactable = true;
                //amountWonText.SetActive(false);
            }
        }
        else
        {
            loadButton.onClick.AddListener(() => StartCoroutine(DisconnectPlayer()));

            if (PlayerPrefs.GetString("MultiplayerMode") == "Duel Mode")
                amountWonText.GetComponent<Text>().text = "You win + " + PlayerPrefs.GetString("stake").ToString() + " ALKOM token";
        }

    }

    private void LoadStartLevel()
    {
        EventManager.Instance.Click();
        List<Models.SceneLoadModel> sceneLoadModel = new List<Models.SceneLoadModel>();
        sceneLoadModel.Add(GameSceneLoader.LoadSceneInstance("Start", 0));
        GameSceneLoader.AsyncLoad(sceneLoadModel);
    }

    IEnumerator ProcessPayment(Button button)
    {
        WWWForm form = new WWWForm();
        form.AddField("amount", (PlayerPrefs.GetFloat("EarnValue") * 2).ToString());
        form.AddField("walletAddress", PlayerPrefs.GetString("WalletID"));
        form.AddField("hash", "ForPlayToEarnWonGame");

        using (UnityWebRequest www = UnityWebRequest.Post("https://backend.alphakombat.com/api/v2/wallet/deposit", form))
        {
            www.SetRequestHeader("x-auth-token", PlayerPrefs.GetString("TokenID"));
            www.SetRequestHeader("alpha-sec-key", "f55da6945d6b8676eff0ae15690cc260d3c64d31a8aa7c6ffb665b855aecd80b5b2a1331a3868a8e11289771f3614d0d");

            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
                Debug.Log(www.downloadHandler.text);

                Debug.Log("check internet connection and try again");
                //errorText.text = "check internet connection and try again";
                button.interactable = true;
            }
            else
            {
                Debug.Log("Form upload complete!");

                Debug.Log(www.downloadHandler.text);

                button.interactable = true;
            }
        }
    }

    IEnumerator DisconnectPlayer()
    {
        Photon.Pun.PhotonNetwork.Disconnect();
        while (Photon.Pun.PhotonNetwork.IsConnected)
            yield return null;

        Debug.Log("disconnected from server");
        LoadStartLevel();
    }
}
