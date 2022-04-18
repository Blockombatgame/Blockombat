using Photon.Pun;
using System;
using UnityEngine;

public delegate void OnDeath(FighterControllerBase player);
public class PlayersManager : MonoBehaviour
{
    public OnDeath Death;
    public static PlayersManager Instance;

    public TestCameraControls cameraController;
    public Transform playerPoint, aiPoint;
    internal Transform spawnedCharacter, aiSpawnedCharacter;
    public bool multiplayerMode;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        if(!multiplayerMode)
        {
            SpawnCharacter();
            SpawnAICharacter();

            spawnedCharacter.GetComponent<FighterMovementController>().otherFighter = aiSpawnedCharacter;
            aiSpawnedCharacter.GetComponent<FighterMovementController>().otherFighter = spawnedCharacter;

            cameraController.LoadCamera();
        }

        EventManager.Instance.OnTimerEndGame += TimerEndGameForLoser;
    }

    public void AddSpawnedPlayer(GameObject spawnedPlayer)
    {
        //dont set the synced player position cause the network takes care of it already
        spawnedPlayer.SetActive(false);

        aiSpawnedCharacter = spawnedPlayer.transform;
        cameraController.targets.Add(aiSpawnedCharacter);

        aiSpawnedCharacter.GetComponent<Rigidbody>().isKinematic = false;

        aiSpawnedCharacter.GetComponent<FighterControllerBase>().playerTag = "player2";

        if (IsAllPlayersSpawned())
        {
            SetUp();
        }
    }

    private void SetUp()
    {
        spawnedCharacter.GetComponent<FighterMovementController>().otherFighter = aiSpawnedCharacter;
        aiSpawnedCharacter.GetComponent<FighterMovementController>().otherFighter = spawnedCharacter;

        spawnedCharacter.gameObject.SetActive(true);
        aiSpawnedCharacter.gameObject.SetActive(true);
        cameraController.LoadCamera();
    }

    private bool IsAllPlayersSpawned()
    {
        return (spawnedCharacter != null) && (aiSpawnedCharacter != null);
    }

    private void SpawnCharacter()
    {
        string characterName = PlayerPrefs.GetString("CharacterSelected");

        GameObject go = FactoryManager.Instance.prefabsFactory.GetPlayerPrefab((EnumClass.PlayerTag)Enum.Parse(typeof(EnumClass.PlayerTag), characterName));
        go.transform.parent = playerPoint;
        go.transform.localPosition = Vector3.zero;
        go.transform.localRotation = Quaternion.identity;
        go.transform.localScale = Vector3.one;
        go.transform.parent = null;

        go.GetComponent<FighterControllerBase>().aiController = false;
        go.GetComponent<FighterControllerBase>().playerTag = "player1";

        cameraController.targets.Add(go.transform);

        spawnedCharacter = go.transform;
    }

    private void SpawnAICharacter()
    {
        string characterName = PlayerPrefs.GetString("aiCharacterSelected");

        GameObject go = FactoryManager.Instance.prefabsFactory.GetPlayerPrefab((EnumClass.PlayerTag)Enum.Parse(typeof(EnumClass.PlayerTag), characterName));
        go.transform.parent = aiPoint;
        go.transform.localPosition = Vector3.zero;
        go.transform.localRotation = Quaternion.identity;
        go.transform.localScale = Vector3.one;
        go.transform.parent = null;

        cameraController.targets.Add(go.transform);

        go.GetComponent<FighterControllerBase>().aiController = true;
        go.GetComponent<FighterControllerBase>().playerTag = "player2";

        aiSpawnedCharacter = go.transform;
    }

    private void TimerEndGameForLoser()
    {
        if(aiSpawnedCharacter.GetComponent<LivingEntity>().currentHealth > spawnedCharacter.GetComponent<LivingEntity>().currentHealth)
        {
            Death?.Invoke(spawnedCharacter.GetComponent<FighterControllerBase>());
        }
        else if (aiSpawnedCharacter.GetComponent<LivingEntity>().currentHealth < spawnedCharacter.GetComponent<LivingEntity>().currentHealth)
        {
            Death?.Invoke(aiSpawnedCharacter.GetComponent<FighterControllerBase>());
        }
        else
        {
            if(UnityEngine.Random.Range(0, 2) == 0)
            {
                Death?.Invoke(spawnedCharacter.GetComponent<FighterControllerBase>());
            }
            else
            {
                Death?.Invoke(aiSpawnedCharacter.GetComponent<FighterControllerBase>());
            }
        }
    }
}
