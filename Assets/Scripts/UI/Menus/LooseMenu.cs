using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LooseMenu : Menu
{
    public Button loadButton;
    public GameObject amountWonText;
    public bool multiplayerMode;

    private void Start()
    {
        if (!multiplayerMode)
        {
            loadButton.onClick.AddListener(() => LoadStartLevel());

            if (PlayerPrefs.GetString("SoloType") == "Free")
            {
                loadButton.interactable = true;
                //amountWonText.SetActive(false);
            }
        }
        else
        {
            loadButton.onClick.AddListener(() => StartCoroutine(DisconnectPlayer()));
            amountWonText.GetComponent<Text>().text = "You lost - " + PlayerPrefs.GetString("stakeAmount").ToString() + " ALKOM token";
        }
    }

    private void LoadStartLevel()
    {
        EventManager.Instance.Click();
        List<Models.SceneLoadModel> sceneLoadModel = new List<Models.SceneLoadModel>();
        sceneLoadModel.Add(GameSceneLoader.LoadSceneInstance("Start", 0));
        GameSceneLoader.AsyncLoad(sceneLoadModel);
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
