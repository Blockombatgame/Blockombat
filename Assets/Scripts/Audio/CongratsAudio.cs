using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CongratsAudio : MonoBehaviour, ISound
{
    public AudioClip[] winSounds, looseSounds;
    public AudioSource audioSource;
    [Range(0, 1)]
    public float audioVolume;

    private void Start()
    {
        EventManager.Instance.OnSettingsDataChanged += UpdateSetup;

        SetupSound();
        EventManager.Instance.OnWinGame += PlayCongratsSound;

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

    public void PlayCongratsSound(string playerTag)
    {
        if(playerTag == "player1")
            audioSource.clip = winSounds[Random.Range(0, winSounds.Length)];
        else if(playerTag == "player2")
            audioSource.clip = looseSounds[Random.Range(0, looseSounds.Length)];

        StartCoroutine(LoadNextAudio(audioSource.clip));
        Play();
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

    IEnumerator LoadNextAudio(AudioClip clip)
    {
        yield return new WaitForSeconds(clip.length);
        EventManager.Instance.PlayBgAudio();
    }
}
