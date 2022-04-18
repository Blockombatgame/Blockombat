using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackAudio : MonoBehaviour, ISound
{
    public AudioClip attackSound, gruntSound;
    public AudioSource audioSource;
    [Range(0, 1)]
    public float audioVolume;

    private void Start()
    {
        EventManager.Instance.OnSettingsDataChanged += UpdateSetup;
        SetupSound();

        UpdateSetup();

    }

    private void UpdateSetup()
    {
        if (PlayerPrefs.GetInt(EnumClass.SettingsType.BackgroundMusic.ToString(), 0) == 0)
        {
            UnMuteAudio();
        }
        else if (PlayerPrefs.GetInt(EnumClass.SettingsType.BackgroundMusic.ToString(), 0) == 1)
        {
            MuteAudio();
        }
    }

    public void MuteAudio()
    {
        audioSource.mute = true;
    }

    public void PlayAttackSound()
    {
        audioSource.clip = attackSound;
        Play();
    }
    
    public void PlayGruntSound()
    {
        audioSource.clip = gruntSound;
        Play();
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
