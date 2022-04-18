using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISound
{
    void SetupSound();
    void Play();
    void MuteAudio();
    void UnMuteAudio();
}
