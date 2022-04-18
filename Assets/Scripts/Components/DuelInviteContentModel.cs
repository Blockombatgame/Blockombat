using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DuelInviteContentModel : MonoBehaviour
{
    public Text challengerName, tokenStaked;
    public Button accept, reject;
    internal string hostId, stakeAmount, duelId;

    public void RegisterCallback(Action acceptCallBack, Action rejectCallback)
    {
        accept.onClick.RemoveAllListeners();
        accept.onClick.AddListener(() => acceptCallBack?.Invoke());

        reject.onClick.RemoveAllListeners();
        reject.onClick.AddListener(() => rejectCallback?.Invoke());
    }

    public void SetUp(string _challengerName, string _stakeAmount, string _hostId, string _duelId)
    {
        challengerName.text = _challengerName;
        tokenStaked.text = _stakeAmount;
        hostId = _hostId;
        duelId = _duelId;
        stakeAmount = _stakeAmount;
    }
}
