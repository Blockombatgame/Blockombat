using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PhotonPlayersManager : MonoBehaviourPunCallbacks
{
    PhotonView pv;
    public PlayersManager playersManager;
    public RoundManager roundManager;

    private void Awake()
    {
        pv = GetComponent<PhotonView>();
    }

    private void Start()
    {
        if (pv.IsMine)
        {
            SpawnCharacter();
            SceneManager.LoadScene("GameUIsMultiplayer", LoadSceneMode.Additive);
        }

        EventManager.Instance.OnRoundFinished += SendRoundDetailsToOtherPlayer;
    }

    private void SpawnCharacter()
    {
        GameObject go = PhotonNetwork.Instantiate(Path.Combine("Characters", PlayerPrefs.GetString("Player1Multiplayer")), Vector3.zero, Quaternion.identity);
        go.SetActive(false);

        if(PhotonNetwork.IsMasterClient)
            go.transform.parent = playersManager.playerPoint;
        else
            go.transform.parent = playersManager.aiPoint;

        go.GetComponent<FighterControllerBase>().playerTag = "player1";

        go.transform.localPosition = Vector3.zero;
        go.transform.localRotation = Quaternion.identity;
        go.transform.localScale = Vector3.one;
        go.transform.parent = null;

        go.GetComponent<FighterControllerBase>().aiController = false;

        playersManager.cameraController.targets.Add(go.transform);

        playersManager.spawnedCharacter = go.transform;
    }

    public void SendRoundDetailsToOtherPlayer(string playerTagName)
    {
        Debug.Log("sent match details");
        if (pv.IsMine)
            GetComponent<PhotonView>().RPC("SendRoundDetails", RpcTarget.Others, playerTagName);
    }

    [PunRPC]
    public void SendRoundDetails(string playerTagName)
    {
        if (pv.IsMine)
            roundManager.AddRoundWinnerScore(playerTagName);
    }
}
