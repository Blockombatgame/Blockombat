using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadMenu : Menu
{
    private List<string> skies = new List<string>{ "sky1", "sky2", "sky3", "sky4" };
    private void Start()
    {
        StartCoroutine(LoadWait());
    }

    private void LoadLevel()
    {
        List<Models.SceneLoadModel> sceneLoadModel = new List<Models.SceneLoadModel>();
        string sky = skies[Random.Range(0, skies.Count)];
        Debug.Log(sky);
        sceneLoadModel.Add(GameSceneLoader.LoadSceneInstance(sky, 0));
        sceneLoadModel.Add(GameSceneLoader.LoadSceneInstance("SoloLevel", 1));
        sceneLoadModel.Add(GameSceneLoader.LoadSceneInstance("GameUIs", 1));
        GameSceneLoader.AsyncLoad(sceneLoadModel);
    }

    IEnumerator LoadWait()
    {
        FactoryManager.Instance.ClearAllPools();
        yield return new WaitForSeconds(2);
        LoadLevel();
    }
}
