using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TournamentContentModel : MonoBehaviour
{
    public Image tournamentImage, isActive, isNotActive, Registered;
    public Text tournamentName, tournamentNoOfParticipants;
    public Button register;
    public string tournamentID;

    public void RegisterCallback(Action callback)
    {
        register.onClick.RemoveAllListeners();
        register.onClick.AddListener(() => callback?.Invoke());
    }

    public void SetUp(string _tournamentName, string _tournamentNoOfParticipants, bool _isRegistered, string _tournamentID)
    {
        tournamentName.text = _tournamentName;
        tournamentNoOfParticipants.text = _tournamentNoOfParticipants;
        isActive.gameObject.SetActive(isActive);
        isNotActive.gameObject.SetActive(!isActive);
        Registered.gameObject.SetActive(_isRegistered);
        register.gameObject.SetActive(!_isRegistered);
        tournamentID = _tournamentID;
    }
}
