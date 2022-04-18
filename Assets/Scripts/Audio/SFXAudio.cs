using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXAudio : MonoBehaviour, ISound
{
    public AudioClip clickSound;
    public AudioSource audioSource;
    [Range(0, 1)]
    public float audioVolume;

    private void Start()
    {
        EventManager.Instance.OnSettingsDataChanged += UpdateSetup;

        EventManager.Instance.OnClick += Play;
        SetupSound();

        UpdateSetup();

    }

    private void UpdateSetup()
    {
        if (PlayerPrefs.GetInt(EnumClass.SettingsType.SFX.ToString(), 0) == 0)
        {
            UnMuteAudio();
        }
        else if (PlayerPrefs.GetInt(EnumClass.SettingsType.SFX.ToString(), 0) == 1)
        {
            MuteAudio();
        }
    }

    public void MuteAudio()
    {
        audioSource.mute = true;
    }

    public void Play()
    {
        audioSource.Play();
    }

    public void SetupSound()
    {
        audioSource.playOnAwake = false;
        audioSource.volume = audioVolume;
        audioSource.loop = false;
    }

    public void UnMuteAudio()
    {
        audioSource.mute = false;
    }
}
