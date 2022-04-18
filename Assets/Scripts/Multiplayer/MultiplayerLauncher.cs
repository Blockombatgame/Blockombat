using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public delegate void OnPhotonNetworkConnectionStarted();
public delegate void OnPhotonNetworkConnectedToServer();
public delegate void OnPhotonNetworkJoinedLobby();
public delegate void OnPhotonNetworkRoomCreated(string roomName);
public delegate void OnPhotonNetworkJoinedRoom(Player[] players, bool isMasterClient);
public delegate void OnPhotonNetworkJoinedRoom2(Player[] players);
public delegate void OnPhotonNetworkEnteredRoom();
public delegate void OnPhotonNetworkMasterClientSwitched();
public delegate void OnPhotonNetworkRoomCreationFailed(string message);
public delegate void OnPhotonNetworkJoinRoom();
public delegate void OnPhotonNetworkLeftRoom();
public delegate void OnPhotonNetworkRoomListUpdate(List<RoomInfo> roomInfos);
public delegate void OnPhotonNetworkDisconnected();
public delegate void OnPhotonNetworkPlayerEnteredRoom(Player player);
public delegate void OnPhotonNetworkPlayerLeftRoom(Player player);
public delegate void OnPhotonNetworkEnded();
public delegate void OnPhotonNetworkLoadGame();
public delegate void OnPhotonNetworkSendCharacterData(EnumClass.PlayerIdentity playerIdentity, string playerTag);

public class MultiplayerLauncher : MonoBehaviourPunCallbacks
{
    public OnPhotonNetworkConnectionStarted PhotonNetworkConnectionStarted;
    public OnPhotonNetworkConnectedToServer PhotonNetworkConnectedToServer;
    public OnPhotonNetworkJoinedLobby PhotonNetworkJoinedLobby;
    public OnPhotonNetworkRoomCreated PhotonNetworkRoomCreated;
    public OnPhotonNetworkJoinedRoom PhotonNetworkJoinedRoom;
    public OnPhotonNetworkMasterClientSwitched PhotonNetworkMasterClientSwitched;
    public OnPhotonNetworkRoomCreationFailed PhotonNetworkRoomCreationFailed;
    public OnPhotonNetworkJoinRoom PhotonNetworkJoinRoom;
    public OnPhotonNetworkLeftRoom PhotonNetworkLeftRoom;
    public OnPhotonNetworkRoomListUpdate PhotonNetworkRoomListUpdate;
    public OnPhotonNetworkDisconnected PhotonNetworkDisconnected;
    public OnPhotonNetworkPlayerEnteredRoom PhotonNetworkPlayerEnteredRoom;
    public OnPhotonNetworkPlayerLeftRoom PhotonNetworkPlayerLeftRoom;
    public OnPhotonNetworkEnded PhotonNetworkEnded;
    public OnPhotonNetworkEnteredRoom PhotonNetworkEnteredRoom;
    public OnPhotonNetworkJoinedRoom2 PhotonNetworkJoinedRoom2;
    public OnPhotonNetworkLoadGame PhotonNetworkLoadGame;
    public OnPhotonNetworkSendCharacterData PhotonNetworkSendCharacterData;

    public static MultiplayerLauncher Instance;

    public EnumClass.ConnectionState connectionState = EnumClass.ConnectionState.NotConnected;
    public List<RoomInfo> roomInfos = new List<RoomInfo>();
    private bool launched = false;

    private void Awake()
    {
        Instance = this;
    }

    public void StartMultiplayer()
    {
        //UiManager.Instance.NetworkCreateRoom += CreateRoom;
        //UiManager.Instance.NetworkLeaveRoom += LeaveRoom;
        Debug.Log("Connecting to master");
        PhotonNetworkConnectionStarted?.Invoke();
        PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.AutomaticallySyncScene = true;
        launched = true;
    }

    public void Update()
    {
        if (launched && connectionState == EnumClass.ConnectionState.Disconnected)
        {
            PhotonNetwork.Reconnect();
        }
    }

    public void SetPhotonPlayerName()
    {
        PhotonNetwork.NickName = PlayerPrefs.GetString("Username");
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to master");
        PhotonNetworkConnectedToServer?.Invoke();
        connectionState = EnumClass.ConnectionState.InitialConnection;
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Joined Lobby");
        PhotonNetworkJoinedLobby?.Invoke();
        SetPhotonPlayerName();
    }

    public void CreateRoom(string roomName)
    {
        RoomOptions newRoomOptions = new RoomOptions();
        newRoomOptions.MaxPlayers = 2;

        PhotonNetwork.CreateRoom(roomName, newRoomOptions);
    }

    public void CreateOrJoinRoom(string roomName)
    {
        RoomOptions roomOpts = new RoomOptions();
        roomOpts.MaxPlayers = (byte)10;

        for (int i = 0; i < roomInfos.Count; i++)
        {
            if (roomInfos[i].Name == roomName)
            {
                //Debug.Log("room already exists");
                PhotonNetwork.JoinRoom("test 1");
                return;
            }
        }
        //Debug.Log("created Room");
        PhotonNetwork.CreateRoom("test 1", roomOpts);
    }

    public override void OnCreatedRoom()
    {
        PhotonNetworkRoomCreated?.Invoke(PhotonNetwork.CurrentRoom.Name);
    }


    public void StartGame()
    {
        PhotonNetwork.LoadLevel("MultiplayerLevel");
    }

    public override void OnJoinedRoom()
    {
        Player[] players = PhotonNetwork.PlayerList;

        //Debug.Log(players.Length);

        PhotonNetworkJoinedRoom?.Invoke(players, PhotonNetwork.IsMasterClient);

        PhotonNetworkEnteredRoom?.Invoke();

        PhotonNetworkJoinedRoom2?.Invoke(players);

        if (CheckCanLoad())
        {
            GetComponent<PhotonView>().RPC("SendEvent", RpcTarget.All);
        }
    }

    public void SendPlayerData(EnumClass.PlayerIdentity playerIdentity, string selectedCharacterTag)
    {
        GetComponent<PhotonView>().RPC("AddPlayerData", RpcTarget.Others, playerIdentity, selectedCharacterTag);
    }

    [PunRPC]
    public void SendEvent()
    {
        PhotonNetworkLoadGame?.Invoke();
    }

    [PunRPC]
    public void AddPlayerData(EnumClass.PlayerIdentity playerIdentity, string selectedCharacterTag)
    {
        PhotonNetworkSendCharacterData?.Invoke(playerIdentity, selectedCharacterTag);
    }

    public bool CheckCanLoad()
    {
        return PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers;
    }

    public bool IsMasterClient()
    {
        return PhotonNetwork.IsMasterClient;
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        PhotonNetworkMasterClientSwitched?.Invoke();
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        //Debug.Log("Room Creation Failed");
        PhotonNetworkRoomCreationFailed?.Invoke(message);
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    public void JoinRoom(string roomName)
    {
        PhotonNetwork.JoinRoom(roomName);
        PhotonNetworkJoinRoom?.Invoke();
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        //base.OnJoinRoomFailed(returnCode, message);
        //Debug.LogError("faled to join");
        //Debug.LogError(message);
        //JoinRoom(PlayerPrefs.GetString("duelRoomName"));
    }

    public override void OnLeftRoom()
    {
        //Debug.Log("have left room");
        PhotonNetworkLeftRoom?.Invoke();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        PhotonNetworkPlayerLeftRoom?.Invoke(otherPlayer);
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        PhotonNetworkRoomListUpdate?.Invoke(roomList);
        roomInfos = roomList;
    }

    public RoomInfo GetRoom(string roomName)
    {
        for (int i = 0; i < roomInfos.Count; i++)
        {
            if(roomInfos[i].Name == roomName)
            {
                return roomInfos[i];
            }
        }

        return null;
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        if (connectionState == EnumClass.ConnectionState.InitialConnection)
        {
            connectionState = EnumClass.ConnectionState.Disconnected;
            PhotonNetworkDisconnected?.Invoke();
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        PhotonNetworkPlayerEnteredRoom?.Invoke(newPlayer);
    }

    public void ExitMultiplayer()
    {
        connectionState = EnumClass.ConnectionState.Exit;
        Debug.Log("disconnecting from server");
        StartCoroutine(DisconnectPlayer());
    }

    IEnumerator DisconnectPlayer()
    {
        PhotonNetwork.Disconnect();
        while (PhotonNetwork.IsConnected)
            yield return null;

        Debug.Log("disconnected from server");
        PhotonNetworkEnded?.Invoke();
        connectionState = EnumClass.ConnectionState.NotConnected;

    }

    public void SendMultiplayerArena(int levelIndex)
    {
        photonView.RPC(nameof(SendLevelData), RpcTarget.Others, levelIndex);
    }

    [PunRPC]
    private void SendLevelData(int loadIndex)
    {
        PlayerPrefs.SetInt("ArenaSelected", loadIndex);
    }
}
