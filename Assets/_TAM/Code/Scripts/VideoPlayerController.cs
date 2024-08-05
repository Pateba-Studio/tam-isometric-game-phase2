using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.Video;

[Serializable]
public class ButtonsAfterFinishPlay
{
    public Button playAgainBtn;
    public Button nextVideoBtn;
    public Button closeVideoBtn;
}

public class VideoPlayerController : MonoBehaviour
{
    [Header("URLs")]
    public string videoURL;
    public string audioURL;

    [Header("UIs")]
    public bool isFullScreen;
    public AudioSource audioSource;
    public RectTransform videoFullScreenPanel;
    public List<VideoPlayer> videoPlayers;
    public List<ButtonsAfterFinishPlay> buttonsAfterFinishPlays;

    [DllImport("__Internal")]
    private static extern void PlayVideo(string str);

    public void PlayVideoExternal(string url)
    {
        Debug.Log("Trying play " + url);
        PlayVideo(url);
    }

    public void PlayVideoWithoutSound()
    {
        foreach (VideoPlayer vidp in videoPlayers)
        {
            vidp.Stop();
            vidp.url = videoURL;
            vidp.audioOutputMode = VideoAudioOutputMode.None;
            vidp.loopPointReached += OnVideoFinished;
            vidp.Play();
        }
    }

    public void PlayVideoWithSound()
    {
        foreach (VideoPlayer vidp in videoPlayers)
        {
            vidp.Stop();
            vidp.url = videoURL;
            vidp.audioOutputMode = VideoAudioOutputMode.None;
        }

        StartCoroutine(AudioDownloader(res =>
        {
            audioSource.clip = res;
            audioSource.Play();

            foreach (VideoPlayer vidp in videoPlayers)
            {
                vidp.Stop();
                vidp.url = videoURL;
                vidp.loopPointReached += OnVideoFinished;
                vidp.Play();
            }
        }));
    }

    public void ChangeVideoScreen()
    {
        if (isFullScreen) videoFullScreenPanel.localScale = Vector3.zero;
        else videoFullScreenPanel.localScale = Vector3.one;
        isFullScreen = !isFullScreen;
    }

    public void ResetButtons()
    {
        foreach (ButtonsAfterFinishPlay btns in buttonsAfterFinishPlays)
        {
            btns.playAgainBtn.gameObject.SetActive(false);
            btns.nextVideoBtn.gameObject.SetActive(false);
            btns.closeVideoBtn.gameObject.SetActive(false);
        }
    }

    public void CloseVideoPanel()
    {
        MultipleChoiceHandler.instance.videoIndex = 0;
        //MultipleChoiceHandler.instance.videoPlayerPanel.SetActive(false);
        MultipleChoiceHandler.instance.multipleChoicePanel.SetActive(true);
        videoFullScreenPanel.localScale = Vector3.zero;

        foreach (ButtonsAfterFinishPlay btns in buttonsAfterFinishPlays)
        {
            btns.playAgainBtn.gameObject.SetActive(false);
            btns.nextVideoBtn.gameObject.SetActive(false);
            btns.closeVideoBtn.gameObject.SetActive(false);
        }
    }

    private void OnVideoFinished(VideoPlayer source)
    {
        foreach (ButtonsAfterFinishPlay btns in buttonsAfterFinishPlays)
        {
            btns.playAgainBtn.gameObject.SetActive(true);
            btns.nextVideoBtn.gameObject.SetActive(MultipleChoiceHandler.instance.CanNextVideo());
            btns.closeVideoBtn.gameObject.SetActive(!MultipleChoiceHandler.instance.CanNextVideo());
        }
    }

    public IEnumerator AudioDownloader(Action<AudioClip> result)
    {
        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(audioURL, AudioType.MPEG))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error downloading audio: " + www.error);
            }
            else
            {
                AudioClip audioClip = DownloadHandlerAudioClip.GetContent(www);
                result(audioClip);
            }
        }
    }
}
