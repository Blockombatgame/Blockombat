using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class WaitMenu : Menu
{
    public Text hintText, loadingText;
    public List<Models.MultiplayerLoadingData> multiplayerLoadingDatas = new List<Models.MultiplayerLoadingData>();

    private void OnEnable()
    {
        bool loaded = false;

        foreach (var multiplayerLoadingData in multiplayerLoadingDatas)
        {
            for (int i = 0; i < multiplayerLoadingData.connectionStates.Count; i++)
            {
                if(multiplayerLoadingData.connectionStates[i] == MultiplayerLauncher.Instance.connectionState)
                {
                    loadingText.text = multiplayerLoadingData.loadingWord;
                    hintText.text = multiplayerLoadingData.hintWord;
                    loaded = true;
                    break;
                }
            }

            if (loaded)
            {
                break;
            }
        }
    }
}
