using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MultiplayerCharacterSelect : Menu
{
    public Button proceed;
    public Transform buttonsParent;
    public Transform playerCharacterPoint;

    private GameObject spawnedCharacter;
    private bool characterSelected = false, loaded = false, characterLoaded = false;
    private string tag;
    public Text playerDisplay;

    private void Start()
    {
        proceed.onClick.AddListener(() => LoadWaitLogic());

        proceed.interactable = false;
        EventManager.Instance.OnCharacterSelect += CharacterSelect;
        EventManager.Instance.OnMenuChange += RecyclePlayersPrefabs;

        LoadCharacterData();
        loaded = true;

        if (PhotonNetwork.IsMasterClient)
        {
            int levelIndex = UnityEngine.Random.Range(0, 8);
            PlayerPrefs.SetInt("ArenaSelected", levelIndex);
            MultiplayerLauncher.Instance.SendMultiplayerArena(levelIndex);
        }
    }

    public void LoadWaitLogic()
    {
        EventManager.Instance.CharacterDataAdded(EnumClass.PlayerIdentity.Player1, tag);
        MultiplayerLauncher.Instance.SendPlayerData(EnumClass.PlayerIdentity.Player2, tag);
        LoadLoadingMenu();
    }

    public void LoadLoadingMenu()
    {
        EventManager.Instance.Click();
        List<string> menuTagNames = new List<string>();
        menuTagNames.Add("wait");
        LoadMenu(proceed, menuTagNames);
    }

    private void CharacterSelect(string characterName)
    {
        for (int i = 0; i < 6; i++)
        {
            if (buttonsParent.GetChild(i).GetComponent<CharacterButton>().playerTag == (EnumClass.PlayerTag)Enum.Parse(typeof(EnumClass.PlayerTag), characterName))
                buttonsParent.GetChild(i).GetComponent<CharacterButton>().ActivateHighlight();
            else
                buttonsParent.GetChild(i).GetComponent<CharacterButton>().DeactivateHighlight();
        }

        SpawnCharacter(characterName);

        if (!characterSelected)
        {
            proceed.interactable = true;
            characterSelected = true;
        }
    }

    private void LoadCharacterData()
    {
        for (int i = 0; i < FactoryManager.Instance.itemsFactory.GetItems(EnumClass.ItemType.Character).Count; i++)
        {
            switch (FactoryManager.Instance.itemsFactory.GetItems(EnumClass.ItemType.Character)[i].itemPurchaseState)
            {
                case EnumClass.ItemPurchaseState.NotBought:
                    buttonsParent.GetChild(i).GetComponent<CharacterButton>().button.interactable = false;
                    break;
                case EnumClass.ItemPurchaseState.Bought:
                    buttonsParent.GetChild(i).GetComponent<CharacterButton>().Setup(FactoryManager.Instance.itemsFactory.GetItems(EnumClass.ItemType.Character)[i].iconImage, (EnumClass.PlayerTag)Enum.Parse(typeof(EnumClass.PlayerTag), FactoryManager.Instance.itemsFactory.GetItems(EnumClass.ItemType.Character)[i].itemTagName));
                    buttonsParent.GetChild(i).GetComponent<CharacterButton>().button.interactable = true;
                    break;
                case EnumClass.ItemPurchaseState.ComingSoon:
                    buttonsParent.GetChild(i).GetComponent<CharacterButton>().button.interactable = false;
                    break;
                default:
                    break;
            }
        }
    }

    private void OnEnable()
    {
        if (characterSelected)
        {
            proceed.interactable = false;
            characterSelected = false;
        }

        if (loaded)
            LoadCharacterData();
    }

    private void SpawnCharacter(string characterName)
    {
        if (characterLoaded && characterName == PlayerPrefs.GetString("CharacterSelected"))
            return;

        if (spawnedCharacter != null)
        {
            FactoryManager.Instance.prefabsFactory.RecyclePlayersPrefab((EnumClass.PlayerTag)Enum.Parse(typeof(EnumClass.PlayerTag), PlayerPrefs.GetString("CharacterSelected")), spawnedCharacter);
        }

        GameObject go = FactoryManager.Instance.prefabsFactory.GetPlayerPrefab((EnumClass.PlayerTag)Enum.Parse(typeof(EnumClass.PlayerTag), characterName));
        go.transform.parent = playerCharacterPoint;
        go.transform.localPosition = Vector3.zero;
        go.transform.localRotation = Quaternion.identity;
        go.transform.localScale = Vector3.one;
        go.GetComponent<FighterControllerBase>().displayForUI = true;
        spawnedCharacter = go;

        PlayerPrefs.SetString("CharacterSelected", characterName);
        tag = characterName;


        playerDisplay.gameObject.SetActive(true);
        playerDisplay.text = go.GetComponent<FighterControllerBase>().playerName;


        characterLoaded = true;
    }

    private void RecyclePlayersPrefabs(List<string> empty)
    {
        if (spawnedCharacter != null)
        {
            FactoryManager.Instance.prefabsFactory.RecyclePlayersPrefab((EnumClass.PlayerTag)Enum.Parse(typeof(EnumClass.PlayerTag), PlayerPrefs.GetString("CharacterSelected")), spawnedCharacter);
        }
    }
}
