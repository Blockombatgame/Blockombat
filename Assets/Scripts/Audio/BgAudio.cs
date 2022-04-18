using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BgAudio : MonoBehaviour, ISound
{
    public AudioClip[] bgSounds;
    public AudioSource audioSource;
    [Range(0, 1)]
    public float audioVolume;

    private void Start()
    {
        SetupSound();
        Play();
        EventManager.Instance.OnWinGame += StopMusic;
        EventManager.Instance.OnPlayBgAudio += Play;
        EventManager.Instance.OnSettingsDataChanged += UpdateSetup;

        UpdateSetup();

    }

    private void UpdateSetup()
    {
        if(PlayerPrefs.GetInt(EnumClass.SettingsType.BackgroundMusic.ToString(), 0) == 0)
        {
            UnMuteAudio();
        }else if (PlayerPrefs.GetInt(EnumClass.SettingsType.BackgroundMusic.ToString(), 0) == 1)
        {
            MuteAudio();
        }
    }

    public void StopMusic(string playerTag)
    {
        audioSource.Stop();
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
        audioSource.playOnAwake = true;
        audioSource.volume = audioVolume;
        audioSource.loop = true;
        audioSource.clip = bgSounds[Random.Range(0, bgSounds.Length)];
    }

    public void UnMuteAudio()
    {
        audioSource.mute = false;
    }
}
