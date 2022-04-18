using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = Constants.Editor_Menu_Prefix + "/Factories/" + nameof(PrefabsFactory))]
public class PrefabsFactory : ScriptableObject
{
    public BloodIdentity bloodEffect;
    public List<LevelIdentity> levels = new List<LevelIdentity>();
    public List<FighterControllerBase> playersPrefabs = new List<FighterControllerBase>();
    public TournamentContentModel tournamentContentModel;
    public TournamentMatchesModel tournamentMatchesModel;
    public DuelInviteContentModel duelInviteContentModel;

    public List<Models.PlayersStorageData> playersStorageDatas = new List<Models.PlayersStorageData>();

    public Dictionary<Type, Queue<GameObject>> prefabsPool = new Dictionary<Type, Queue<GameObject>>();
    public Dictionary<EnumClass.PlayerTag, Queue<GameObject>> playerPrefabsPool = new Dictionary<EnumClass.PlayerTag, Queue<GameObject>>();

    public T GetItem<T>(T item) where T : MonoBehaviour
    {
        T ins = null;
        if (prefabsPool.TryGetValue(item.GetType(), out Queue<GameObject> items))
        {
            if (items.Count > 0)
            {
                ins = items.Dequeue().GetComponent<T>();
                ins.gameObject.SetActive(true);
                return ins;
            }
        }

        ins = Instantiate(item, Vector3.zero, Quaternion.identity);
        return ins;
    }

    public GameObject GetPlayerPrefab(EnumClass.PlayerTag playerTag, GameObject _prefab)
    {
        GameObject go = null;
        if (playerPrefabsPool.TryGetValue(playerTag, out Queue<GameObject> prefab))
        {
            if (prefab.Count > 0)
            {
                go = prefab.Dequeue();
                go.SetActive(true);
                return go;
            }
        }

        go = Instantiate(_prefab, Vector3.zero, Quaternion.identity);
        return go;
    }

    public void Recycle<T>(T obj) where T : MonoBehaviour
    {
        obj.gameObject.SetActive(false);
        if (prefabsPool.ContainsKey(obj.GetType()))
        {
            prefabsPool[obj.GetType()].Enqueue(obj.gameObject);
        }
        else
        {
            prefabsPool.Add(obj.GetType(), new Queue<GameObject>());
            prefabsPool[obj.GetType()].Enqueue(obj.gameObject);
        }
    }

    public void RecyclePlayersPrefab(EnumClass.PlayerTag playerTag, GameObject spawnedPrefab)
    {
        spawnedPrefab.SetActive(false);
        if (playerPrefabsPool.ContainsKey(playerTag))
        {
            playerPrefabsPool[playerTag].Enqueue(spawnedPrefab);
        }
        else
        {
            playerPrefabsPool.Add(playerTag, new Queue<GameObject>());
            playerPrefabsPool[playerTag].Enqueue(spawnedPrefab);
        }
    }

    public void ResetPool()
    {
        prefabsPool.Clear();
        playerPrefabsPool.Clear();
    }

    public GameObject GetPlayerPrefab(EnumClass.PlayerTag playerTag)
    {
        return GetPlayerPrefab (playerTag, playersStorageDatas.Where(x => x.playerTag == playerTag).FirstOrDefault().playerPrefab);
    }
}
