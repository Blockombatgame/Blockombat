using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : Menu
{
    public Button resume, restart, settings, quit;

    public void Start()
    {
        restart.onClick.AddListener(() => LoadLevel());
        settings.onClick.AddListener(() => LoadSettingsMenu());
        quit.onClick.AddListener(() => LoadStartLevel());
        resume.onClick.AddListener(() => ResumeGame());
    }

    private void LoadLevel()
    {
        EventManager.Instance.Click();
        FactoryManager.Instance.ClearAllPools();

        Time.timeScale = 1;
        List<Models.SceneLoadModel> sceneLoadModel = new List<Models.SceneLoadModel>();

        switch (PlayerPrefs.GetInt("ArenaSelected"))
        {
            case 0:
                sceneLoadModel.Add(GameSceneLoader.LoadSceneInstance("Level1", 0));
                break;
            case 1:
                sceneLoadModel.Add(GameSceneLoader.LoadSceneInstance("Level2", 0));
                break;
            case 2:
                sceneLoadModel.Add(GameSceneLoader.LoadSceneInstance("Level3", 0));
                break;
            case 3:
                sceneLoadModel.Add(GameSceneLoader.LoadSceneInstance("Level4", 0));
                break;
            case 4:
                sceneLoadModel.Add(GameSceneLoader.LoadSceneInstance("Level5", 0));
                break;
            case 5:
                sceneLoadModel.Add(GameSceneLoader.LoadSceneInstance("Level6", 0));
                break;
            case 6:
                sceneLoadModel.Add(GameSceneLoader.LoadSceneInstance("Level7", 0));
                break;
            case 7:
                sceneLoadModel.Add(GameSceneLoader.LoadSceneInstance("Level8", 0));
                break;
            default:
                break;
        }

        sceneLoadModel.Add(GameSceneLoader.LoadSceneInstance("GameUIs", 1));
        GameSceneLoader.AsyncLoad(sceneLoadModel);
    }

    private void LoadStartLevel()
    {
        EventManager.Instance.Click();
        FactoryManager.Instance.ClearAllPools();

        Time.timeScale = 1;
        List<Models.SceneLoadModel> sceneLoadModel = new List<Models.SceneLoadModel>();
        sceneLoadModel.Add(GameSceneLoader.LoadSceneInstance("Start", 0));
        GameSceneLoader.AsyncLoad(sceneLoadModel);
    }

    private void ResumeGame()
    {
        EventManager.Instance.Click();
        Time.timeScale = 1;
        List<string> menuTagNames = new List<string>();
        menuTagNames.Add("");
        EventManager.Instance.MenuChange(menuTagNames);
    }

    private void LoadSettingsMenu()
    {
        EventManager.Instance.Click();
        List<string> menuTagNames = new List<string>();
        menuTagNames.Add("settings");
        LoadMenu(settings, menuTagNames);
    }
}
