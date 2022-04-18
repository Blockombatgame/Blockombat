using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadLevelManager : MonoBehaviour
{
    private void Start()
    {
        //GameObject environment = Instantiate(environments[PlayerPrefs.GetInt("ArenaSelected")], Vector3.zero, Quaternion.identity);
        GameObject environment = FactoryManager.Instance.prefabsFactory.GetItem(FactoryManager.Instance.prefabsFactory.levels[PlayerPrefs.GetInt("ArenaSelected")]).gameObject;
        environment.SetActive(true);
    }
}
