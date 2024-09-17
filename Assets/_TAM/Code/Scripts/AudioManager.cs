using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

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

    public IEnumerator PlayAudioFromURL(string url, Action events)
    {
        using (UnityWebRequest request = UnityWebRequestMultimedia.GetAudioClip(url, AudioType.MPEG))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Failed to load audio: " + request.error);
            }
            else
            {
                AudioClip clip = DownloadHandlerAudioClip.GetContent(request);
                audioSources[1].clip = clip;
                audioSources[1].Play();

                StartCoroutine(CheckIfAudioFinished(events));
            }
        }
    }

    IEnumerator CheckIfAudioFinished(Action OnAudioFinished)
    {
        yield return new WaitWhile(() => audioSources[1].isPlaying);
        GameManager.instance.loadingPanel.SetActive(false);
        OnAudioFinished?.Invoke();
    }
}
