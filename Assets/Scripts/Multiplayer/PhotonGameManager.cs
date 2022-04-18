using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;

public class PhotonGameManager : MonoBehaviourPunCallbacks
{
    private void Start()
    {
        MultiplayerLauncher.Instance.PhotonNetworkLoadGame += LoadGame;
    }

    private void LoadGame()
    {
        if (MultiplayerLauncher.Instance.CheckCanLoad())
        {
            Menu menu = MenuManager.Instance.menus.Where(x => x.menuName == "duelWaitRoom").FirstOrDefault();
            DuelWaitRoom duelWaitRoom = (DuelWaitRoom)menu;
            duelWaitRoom.LoadGame();
        }
    }
}
