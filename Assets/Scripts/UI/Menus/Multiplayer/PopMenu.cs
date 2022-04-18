using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopMenu : Menu
{
    public Button accept, decline;
    public Text displayText1, displayText2;

    public void SetPopMenu(Action acceptCallback, Action declineCallback, string hostName, string stakeAmount)
    {
        accept.onClick.RemoveAllListeners();
        accept.onClick.AddListener(delegate { acceptCallback?.Invoke(); } );
        decline.onClick.RemoveAllListeners();
        decline.onClick.AddListener(delegate { declineCallback?.Invoke(); } );
        displayText1.text = hostName + " has challenged you to a duel.";
        displayText2.text = stakeAmount + " Alkom";
    }
}
