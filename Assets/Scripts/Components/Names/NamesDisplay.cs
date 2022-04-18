using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NamesDisplay : MonoBehaviour
{
    public string tagName;
    public Text display;

    private void Start()
    {
        for (int i = 0; i < FindObjectsOfType<FighterControllerBase>().Length; i++)
        {
            if(tagName == FindObjectsOfType<FighterControllerBase>()[i].playerTag)
            {
                display.text = FindObjectsOfType<FighterControllerBase>()[i].playerName;
            }
        }
    }
}
