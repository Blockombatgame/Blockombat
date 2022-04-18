using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StoreCharacterData : MonoBehaviour
{
    public List<Models.CharacterSelectData> characterSelectDatas = new List<Models.CharacterSelectData>();

    private void Start()
    {
        for (int i = 0; i < 2; i++)
        {
            Models.CharacterSelectData characterSelectData = new Models.CharacterSelectData();
            characterSelectData.playerIdentity = (EnumClass.PlayerIdentity)i;
            characterSelectDatas.Add(characterSelectData);
        }

        MultiplayerLauncher.Instance.PhotonNetworkSendCharacterData += AddCharacter;
        EventManager.Instance.OnCharacterDataAdded += AddCharacter;
        EventManager.Instance.OnFactoryReset += SaveData;
    }

    private void AddCharacter(EnumClass.PlayerIdentity playerIdentity, string characterTag)
    {
        characterSelectDatas.Where(x => x.playerIdentity == playerIdentity).FirstOrDefault().selectedCharacterTag = characterTag;
        characterSelectDatas.Where(x => x.playerIdentity == playerIdentity).FirstOrDefault().full = true;

        if (CheckAllDataFilled())
        {
            EventManager.Instance.FactoryReset();
            StartCoroutine(DelayLoad());
        }
    }

    private void SaveData()
    {
        PlayerPrefs.SetString("Player1Multiplayer", characterSelectDatas[0].selectedCharacterTag);
        PlayerPrefs.SetString("Player2Multiplayer", characterSelectDatas[1].selectedCharacterTag);
    }

    private bool CheckAllDataFilled()
    {
        bool filled = true;

        for (int i = 0; i < characterSelectDatas.Count; i++)
        {
            if (!characterSelectDatas[i].full)
            {
                filled = false;
            }
        }

        if (filled)
            return true;

        return false;
    }

    IEnumerator DelayLoad()
    {
        yield return new WaitForSeconds(1.5f);
        if (MultiplayerLauncher.Instance.IsMasterClient())
        {
            MultiplayerLauncher.Instance.StartGame();
        }
    }
}
