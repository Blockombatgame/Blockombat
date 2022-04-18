using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TournamentMatchesModel : MonoBehaviour
{
    public Text player1Name, player2Name;
    public Button startMatch;
    internal string teamID;
    internal int matchID, playerId;

    public void RegisterCallback(Action callback)
    {
        startMatch.onClick.RemoveAllListeners();
        startMatch.onClick.AddListener(() => callback?.Invoke());
    }

    public void SetUp(string _player1Name, string _player2Name, string _teamID, int _matchID, int _playerId, bool isActive)
    {
        player1Name.text = _player1Name;
        player2Name.text = _player2Name;
        startMatch.interactable = isActive;
        teamID = _teamID;
        matchID = _matchID;
        playerId = _playerId;
    }
}
