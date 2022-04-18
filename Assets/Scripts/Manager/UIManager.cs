using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public GameObject playerHUD;
    public GameObject winScreen, looseScreen;
    public bool multiplayerMode = false;

    private void Awake()
    {
        if(EventManager.Instance != null)
        {
            EventManager.Instance.OnWinGame += DisplayWinnerandLooseUI;
            EventManager.Instance.OnNetworkPlayerWin += DisplayNetworkWinnerAndLoser;
            EventManager.Instance.OnNetworkPlayerLost += NetworkLostResponse;
        }
    }

    private void NetworkLostResponse(string obj)
    {
        looseScreen.SetActive(true);
    }

    private void DisplayWinnerandLooseUI(string playerName)
    {
        playerHUD.SetActive(false);

        if (playerName == "player1")
        {
            winScreen.SetActive(true);
        }
        else
        {
            looseScreen.SetActive(true);
        }
    }

    private void DisplayNetworkWinnerAndLoser(string playerName)
    {
        playerHUD.SetActive(false);

        foreach (var fighter in FindObjectsOfType<FighterControllerBase>())
        {
            if (fighter.GetComponent<PhotonView>().IsMine)
            {
                if (playerName == fighter.playerTag)
                {
                    winScreen.SetActive(true);
                }
                else
                {
                    looseScreen.SetActive(true);
                }
            }
        }
    }
}
