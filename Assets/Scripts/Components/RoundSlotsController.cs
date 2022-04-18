using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class RoundSlotsController : MonoBehaviour
{
    public string playerTag;
    public List<GameObject> winSlots = new List<GameObject>();
    public bool multiplayerMode;

    private void Start()
    {
        EventManager.Instance.OnRoundSlotChange += LoadWinSlot;

        //if (multiplayerMode)
        //{
        //    if (!PhotonNetwork.IsMasterClient)
        //    {
        //        if (playerTag == "player1")
        //        {
        //            playerTag = "player2";
        //        }
        //        else
        //        {
        //            playerTag = "player1";
        //        }
        //    }
        //}
    }

    public void LoadWinSlot(int score, string _playerTag)
    {
        if(playerTag == _playerTag)
            winSlots[score - 1].gameObject.SetActive(true);
    }
}
