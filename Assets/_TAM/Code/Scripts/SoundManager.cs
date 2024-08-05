using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;
    public GameObject muteUnmuteButton;
    public GameObject indicatorAudioOn;
    public GameObject indicatorAudioOff;
    public List<AudioSource> audioSources;
    
    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        if (!DataHolder.instance.isFirstOpen)
        {
            SetFirstAudioState();
        }
    }

    public void SetFirstAudioState()
    {
        if (DataHolder.instance.playerData.audioState == 1) UnMuteAll();
        else MuteAll();
    }

    public void SetMuteCondition()
    {
        if (DataHolder.instance.playerData.audioState == 1) MuteAll();
        else UnMuteAll();
    }

    public void MuteAll()
    {
        DataHolder.instance.playerData.audioState = 0;
        APIManager.instance.PostChangeAudio();

        indicatorAudioOn.SetActive(false);
        indicatorAudioOff.SetActive(true);

        foreach (AudioSource source in audioSources)
        {
            source.mute = true;
        }
    }

    public void UnMuteAll()
    {
        DataHolder.instance.playerData.audioState = 1;
        APIManager.instance.PostChangeAudio();

        indicatorAudioOn.SetActive(true);
        indicatorAudioOff.SetActive(false);

        foreach (AudioSource source in audioSources)
        {
            source.mute = false;
        }
    }

    public void SetVolume(float volume)
    {
        foreach (AudioSource source in audioSources)
        {
            source.volume = volume;
        }
    }
}
