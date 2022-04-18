using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CharacterMenu : Menu
{
    public Button back, proceed;
    public Transform buttonsParent, blockScreen;
    public Transform aiCharacterPoint, playerCharacterPoint;

    private GameObject spawnedCharacter, aiSpawnedCharacter;
    private bool characterSelected = false, loaded = false, characterLoaded = false;
    public Text[] playersDisplay;

    private void Start()
    {
        back.onClick.AddListener(() => LoadBackMenu());
        proceed.onClick.AddListener(() => LoadArenaMenu());
        proceed.interactable = false;
        EventManager.Instance.OnCharacterSelect += CharacterSelect;
        EventManager.Instance.OnMenuChange += RecyclePlayersPrefabs;

        LoadCharacterData();
        loaded = true;
    }

    private void LoadBackMenu()
    {
        EventManager.Instance.Click();
        RecyclePlayersPrefabs(null);

        LoadMenu(back, previousMenuName);
    }

    private void LoadArenaMenu()
    {
        EventManager.Instance.Click();
        SpawnAICharacter();
        blockScreen.gameObject.SetActive(true);
        StartCoroutine(LoadWaitTime());
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

        if(loaded)
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

        playersDisplay[0].gameObject.SetActive(true);
        playersDisplay[0].text = go.GetComponent<FighterControllerBase>().playerName;

        PlayerPrefs.SetString("CharacterSelected", characterName);

        characterLoaded = true;
    }

    private void SpawnAICharacter()
    {
        List<Item> items = new List<Item>();
        items = FactoryManager.Instance.itemsFactory.GetBoughtCharacters();

        items.Remove(items.Where(x => x.itemTagName == PlayerPrefs.GetString("CharacterSelected")).FirstOrDefault());

        string characterName = items[UnityEngine.Random.Range(0, items.Count)].itemTagName;

        if (aiSpawnedCharacter != null)
        {
            FactoryManager.Instance.prefabsFactory.RecyclePlayersPrefab((EnumClass.PlayerTag)Enum.Parse(typeof(EnumClass.PlayerTag), PlayerPrefs.GetString("aiCharacterSelected")), aiSpawnedCharacter);
        }

        GameObject go = FactoryManager.Instance.prefabsFactory.GetPlayerPrefab((EnumClass.PlayerTag)Enum.Parse(typeof(EnumClass.PlayerTag), characterName));
        go.transform.parent = aiCharacterPoint;
        go.transform.localPosition = Vector3.zero;
        go.transform.localRotation = Quaternion.identity;
        go.transform.localScale = Vector3.one;
        go.GetComponent<FighterControllerBase>().displayForUI = true;
        aiSpawnedCharacter = go;

        playersDisplay[1].gameObject.SetActive(true);
        playersDisplay[1].text = go.GetComponent<FighterControllerBase>().playerName;

        PlayerPrefs.SetString("aiCharacterSelected", characterName);
    }

    private void RecyclePlayersPrefabs(List<string> empty)
    {
        if (aiSpawnedCharacter != null)
        {
            FactoryManager.Instance.prefabsFactory.RecyclePlayersPrefab((EnumClass.PlayerTag)Enum.Parse(typeof(EnumClass.PlayerTag), PlayerPrefs.GetString("CharacterSelected")), aiSpawnedCharacter);
        }

        if (spawnedCharacter != null)
        {
            FactoryManager.Instance.prefabsFactory.RecyclePlayersPrefab((EnumClass.PlayerTag)Enum.Parse(typeof(EnumClass.PlayerTag), PlayerPrefs.GetString("CharacterSelected")), spawnedCharacter);
        }
    }

    IEnumerator LoadWaitTime()
    {
        yield return new WaitForSeconds(2);
        List<string> menuTagNames = new List<string>();
        menuTagNames.Add("header");
        menuTagNames.Add("arena");
        LoadMenu(proceed, menuTagNames);
        blockScreen.gameObject.SetActive(false);
    }
}
