using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public GameObject muteOnButton;
    public GameObject muteOffButton;

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
