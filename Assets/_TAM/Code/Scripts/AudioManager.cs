using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    public GameObject muteOnButton;
    public GameObject muteOffButton;
    public List<AudioSource> audioSources;

    private void Awake()
    {
        instance = this;
    }

    public void SetAudioState(bool cond)
    {
        if (cond) audioSources.ForEach(x => x.mute = true);

        if (muteOnButton.activeSelf && !cond)
            audioSources.ForEach(x => x.mute = false);
    }

    public void SetAudioOn()
    {
        if (muteOffButton.activeSelf) return;

        muteOffButton.SetActive(true);
        muteOnButton.SetActive(false);

        foreach (var item in FindObjectsOfType<AudioSource>())
        {
            item.mute = true;
        };
    }

    public void SetAudioOff()
    {
        if (muteOnButton.activeSelf) return;

        muteOffButton.SetActive(false);
        muteOnButton.SetActive(true);

        foreach (var item in FindObjectsOfType<AudioSource>())
        {
            item.mute = false;
        };
    }
}
