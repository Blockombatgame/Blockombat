using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

public class SpashScreenLoad : MonoBehaviour
{
    public VideoClip videoClip;

    private void Start()
    {
        //StartCoroutine(LoadTime());
    }

    private void LoadNextScene()
    {
        SceneManager.LoadScene("Start");
    }

    IEnumerator LoadTime()
    {
        yield return new WaitForSeconds((float)videoClip.length);
        GetComponent<Animator>().Play("FadeOut");
        yield return new WaitForSeconds(0.5f);
        //LoadNextScene();
        //Debug.Log("called me");
    }
}
