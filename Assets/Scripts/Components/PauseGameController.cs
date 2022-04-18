using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseGameController : MonoBehaviour
{
    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(() => LoadPauseMenu());
    }

    private void LoadPauseMenu()
    {
        EventManager.Instance.Click();
        Time.timeScale = 0;
        List<string> menuNames = new List<string>();
        menuNames.Add("pause");
        EventManager.Instance.MenuChange(menuNames);
    }
}
