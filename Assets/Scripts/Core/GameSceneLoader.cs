using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Collections;

public static class GameSceneLoader
{
    public static List<Models.SceneLoadModel> _loadedSceneModels = new List<Models.SceneLoadModel>();
    public static List<AsyncOperation> asyncsLoads = new List<AsyncOperation>();

    public static Models.SceneLoadModel LoadSceneInstance(string sceneName, int loadType)
    {
        Models.SceneLoadModel sceneLoadModel = new Models.SceneLoadModel();
        sceneLoadModel.sceneName = sceneName;
        sceneLoadModel.sceneLoadType = loadType;

        return sceneLoadModel;
    }

    public static void AsyncLoad(List<Models.SceneLoadModel> sceneModels)
    {
        asyncsLoads.Clear();

        for (int i = 0; i < sceneModels.Count; i++)
        {
            asyncsLoads.Add(SceneManager.LoadSceneAsync(sceneModels[i].sceneName, (LoadSceneMode)sceneModels[i].sceneLoadType));
            //_loadedSceneModels.Add(sceneModels[i]);
        }
    }

    public static Scene GetActiveScene()
    {
        return SceneManager.GetActiveScene();
    }

    public static void SetActiveScene(Scene scene)
    {
        SceneManager.SetActiveScene(scene);
    }


    public static void MoveGameObjectToActiveScene(GameObject objectToMove, Scene sceneToChangeTo)
    {
        SceneManager.MoveGameObjectToScene(objectToMove, sceneToChangeTo);
    }

    public static bool CheckIfSceneIsLoaded(string sceneName)
    {
        return SceneManager.GetSceneByName(sceneName).isLoaded;
    }

    public static Scene GetScene(string sceneName)
    {
        return SceneManager.GetSceneByName(sceneName);
    }
}
