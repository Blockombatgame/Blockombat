using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphicsManager : MonoBehaviour
{
    private void Start()
    {
        EventManager.Instance.OnSettingsDataChanged += UpdateSetup;

        UpdateSetup();
    }

    private void UpdateSetup()
    {
        switch (PlayerPrefs.GetInt(EnumClass.SettingsType.Graphics.ToString()))
        {
            case 0:
                QualitySettings.SetQualityLevel(0);
                break;
            case 1:
                QualitySettings.SetQualityLevel(3);
                break;
            default:
                break;
        }
    }
}
