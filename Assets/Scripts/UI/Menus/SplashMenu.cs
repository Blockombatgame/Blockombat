using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class SplashMenu : Menu
{
    public VideoClip videoClip;
    public Image panel;
    public GameObject vidPlayer;

    private void Start()
    {
        StartCoroutine(LoadTime());
    }

    IEnumerator LoadTime()
    {
        yield return new WaitForSeconds((float)videoClip.length);
        panel.GetComponent<Animator>().Play("FadeOut");
        vidPlayer.SetActive(false);
        yield return new WaitForSeconds(0.5f);
        LoadStartMenu();
    }

    private void LoadStartMenu()
    {
        List<string> menuTagNames = new List<string>();
        menuTagNames.Add("start");
        LoadMenu(null, menuTagNames);
    }
}
