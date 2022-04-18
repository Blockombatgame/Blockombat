using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using System.IO;

public class RoomManager : MonoBehaviourPunCallbacks
{
    public static RoomManager roomInstance;

    private void Awake()
    {
        if(roomInstance != null)
        {
            Destroy(roomInstance.gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
        roomInstance = this;
    }

    public override void OnEnable()
    {
        base.OnEnable();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public override void OnDisable()
    {
        base.OnDisable();
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        if(scene.name == "MultiplayerLevel")
        {
            GameObject go = PhotonNetwork.Instantiate(Path.Combine("Manager", "photonPlayersManager"), Vector3.zero, Quaternion.identity);
            go.GetComponent<PhotonPlayersManager>().playersManager = FindObjectOfType<PlayersManager>();
            go.GetComponent<PhotonPlayersManager>().roundManager = FindObjectOfType<RoundManager>();

            Destroy(gameObject);
        }
        else if (scene.buildIndex != 1)
        {
            Destroy(gameObject);
        }
    }
}
