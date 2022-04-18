using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class RoundManager : MonoBehaviour
{
    public Transform player1Position, player2Position;
    public List<Models.RoundData> roundDatas = new List<Models.RoundData>();
    public int startCount, maxCount;
    private Coroutine countCoroutine;
    public bool multiplayerMode, paymentProcessed = false;

    private void Start()
    {
        EventManager.Instance.OnRoundFinished += AddRoundWinnerScore;
        EventManager.Instance.OnRoundReset += OnRoundReset;
        EventManager.Instance.OnNetworkPlayerLeave += OnPlayerLeftWin;

        SetCountDown();
    }

    public void OnRoundReset(GameObject player)
    {
        StartCoroutine(DelayRoundReset(player));
    }

    public void AddRoundWinnerScore(string playerTagName)
    {
        if(countCoroutine != null)
            StopCoroutine(countCoroutine);

        for (int i = 0; i < roundDatas.Count; i++)
        {
            if(roundDatas[i].playerTagName == playerTagName)
            {
                Models.RoundData roundData = new Models.RoundData();
                roundData.playerTagName = playerTagName;
                roundData.score = roundDatas[i].score + 1;
                roundDatas[i] = roundData;

                EventManager.Instance.RoundSlotChange(roundDatas[i].score, playerTagName);

                if (roundDatas[i].score == 2)
                {
                    //Call round Over with event
                    StartCoroutine(DelayWinEvent(playerTagName));
                }
                else
                {
                    //Play Next round with event
                    EventManager.Instance.RoundOver();
                }
            }
        }
    }

    IEnumerator DelayWinEvent(string playerTagName)
    {
        if (!multiplayerMode)
        {
            EventManager.Instance.WinGame(playerTagName);
        }
        else
        {
            //send win to server and then activate win system
            FighterControllerBase player1 = FindObjectOfType<PlayersManager>().spawnedCharacter.GetComponent<FighterControllerBase>();
            if (player1.playerTag == playerTagName)
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    if (PlayerPrefs.GetString("MultiplayerMode") == "Duel Mode")
                        StartCoroutine(SendWinnerToServerAtGameEnd(PlayerPrefs.GetString("hostPlayerID")));
                    else if (PlayerPrefs.GetString("MultiplayerMode") == "Tournament Mode")
                        StartCoroutine(SendWinnerToServerAtGameEndForTournament(PlayerPrefs.GetString("PlayerTournamentID")));
                }
                else
                {
                    if (PlayerPrefs.GetString("MultiplayerMode") == "Duel Mode")
                        StartCoroutine(SendWinnerToServerAtGameEnd(PlayerPrefs.GetString("invitedPlayerID")));
                    else if (PlayerPrefs.GetString("MultiplayerMode") == "Tournament Mode")
                        StartCoroutine(SendWinnerToServerAtGameEndForTournament(PlayerPrefs.GetString("PlayerTournamentID")));
                }

                //Start a wait UI or disable the submit button until it is done.
                yield return new WaitUntil(() => paymentProcessed == true);
                EventManager.Instance.NetworkPlayerWin(playerTagName);
            }
            else
            {
                EventManager.Instance.NetworkPlayerLost(playerTagName);
            }
        }
        //network win to server
    }

    private void OnPlayerLeftWin(string playerTag)
    {
        foreach (var fighter in FindObjectsOfType<FighterControllerBase>())
        {
            if (fighter.GetComponent<PhotonView>().IsMine)
            {
                if (playerTag != fighter.playerTag)
                {
                    StartCoroutine(DelayWinEvent(fighter.playerTag));
                }
            }
        }
    }

    IEnumerator DelayRoundReset(GameObject player)
    {
        yield return new WaitForSeconds(4);
        EventManager.Instance.LoadFadePanel();
        yield return new WaitForSeconds(1);

        if (!multiplayerMode)
        {
            if (player.GetComponent<FighterControllerBase>().playerTag == "player1")
            {
                player.transform.position = player1Position.position;
                player.transform.rotation = player1Position.rotation;
            }
            else if (player.GetComponent<FighterControllerBase>().playerTag == "player2")
            {
                player.transform.position = player2Position.position;
                player.transform.rotation = player2Position.rotation;
            }
        }
        else
        {
            if (player.GetComponent<PhotonView>().IsMine)
            {

                if (PhotonNetwork.IsMasterClient)
                    player.transform.parent = player1Position;
                else
                    player.transform.parent = player2Position;

                player.transform.localPosition = Vector3.zero;
                player.transform.localRotation = Quaternion.identity;
                player.transform.localScale = Vector3.one;
                player.transform.parent = null;
            }
        }

        player.GetComponent<LivingEntity>().ResetHealth();
        yield return new WaitForSeconds(1);

        player.GetComponent<FighterControllerBase>().movementController.UnlockMovement();
        player.GetComponent<FighterControllerBase>().lockControls = false;
        player.GetComponent<FighterControllerBase>().animationController.UnlockIdleAnimation();
        player.GetComponent<FighterControllerBase>().isDead = false;
        SetCountDown();

        if (!multiplayerMode)
        {
            yield return new WaitForSeconds(2);

            if (player.GetComponent<FighterControllerBase>().aiController)
            {
                player.GetComponent<FighterControllerBase>().aIBattleSystem.StartAI();
            }
            else
            {
                //player.GetComponent<FighterControllerBase>().SubscribeToInputs();
            }
        }
    }

    public void SetCountDown()
    {
        if (!multiplayerMode)
        {
            switch (PlayerPrefs.GetInt(EnumClass.SettingsType.SetPlayTime.ToString(), 0))
            {
                case 0:
                    maxCount = -1;
                    break;
                case 1:
                    maxCount = 90;
                    break;
                case 2:
                    maxCount = 60;
                    break;
                case 3:
                    maxCount = 30;
                    break;
                default:
                    break;
            }

            startCount = maxCount;
            if (startCount != -1)
            {
                EventManager.Instance.UpdateCountUI(startCount);
                countCoroutine = StartCoroutine(CountDown());
            }
        }
    }

    IEnumerator CountDown()
    {
        yield return new WaitForSeconds(1);
        startCount--;
        EventManager.Instance.UpdateCountUI(startCount);

        if (startCount == 0)
        {
            //Game over
            EventManager.Instance.TimerEndGame();
        }
        else
        {
            StopCoroutine(countCoroutine);
            countCoroutine = StartCoroutine(CountDown());
        }
    }



    //Win conditions For Duel
    IEnumerator ProcessPayment()
    {
        WWWForm form = new WWWForm();
        form.AddField("amount", (int.Parse(PlayerPrefs.GetString("stake")) * 2).ToString());
        form.AddField("walletAddress", PlayerPrefs.GetString("WalletID"));
        form.AddField("hash", "ForDuelWin");

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
            }
            else
            {
                Debug.Log("Form upload complete!");

                paymentProcessed = true;

                Debug.Log(www.downloadHandler.text);
            }
        }
    }

    //End Game After Duel Win
    IEnumerator SendWinnerToServerAtGameEnd(string winnerID)
    {
        WWWForm form = new WWWForm();

        form.AddField("winnerId", winnerID);
        form.AddField("duelId", PlayerPrefs.GetString("duelID"));

        using (UnityWebRequest www = UnityWebRequest.Post("https://backend.alphakombat.com/api/v2/duel/end", form))
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
            }
            else
            {
                Debug.Log("Form upload complete!");

                Debug.Log(www.downloadHandler.text);

                StartCoroutine(ProcessPayment());

            }
        }
    }

    //End Game After Tournament Win
    IEnumerator SendWinnerToServerAtGameEndForTournament(string winnerID)
    {
        WWWForm form = new WWWForm();

        form.AddField("winnerId", winnerID);

        using (UnityWebRequest www = UnityWebRequest.Post("https://backend.alphakombat.com/api/v2/tournaments/3/matches/" + PlayerPrefs.GetInt("MatchID"), form))
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
            }
            else
            {
                Debug.Log("Form upload complete!");

                Debug.Log(www.downloadHandler.text);

                StartCoroutine(ProcessPayment());

            }
        }
    }
}
