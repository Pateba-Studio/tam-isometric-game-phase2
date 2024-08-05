using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class MediaIntroManager : MonoBehaviour
{
    public int videoIndex;
    public GameObject afterVideoFinishedPanel;

    private bool _isIntro;
    private void Start()
    {
        if (SequenceManager.Instance.IsMediaIntro &&
            SequenceManager.Instance.MediaSrc != null)
        {


            if (SequenceManager.Instance.MediaSrc.data.Count > 0)
            {
                if (!SequenceManager.Instance.MediaSrc.watch_intro)
                {
                    _isIntro = true;
                    StartCoroutine(SetupVideoPanel());
                }
                else
                {
                    StartCoroutine(SetupCaseVideoPanel());
                }
            }
        }
    }

    public IEnumerator SetupVideoPanel()
    {
        SequenceManager.Instance.IsMediaIntro = true;

        Debug.Log("Playing Intro Video " + SequenceManager.Instance.MediaSrc.data[videoIndex].media);
        HTMLVideoTrigger.instance.PlayVideoExternal(SequenceManager.Instance.MediaSrc.data[videoIndex].media);

        yield return new WaitForSeconds(.5f);
        afterVideoFinishedPanel.SetActive(true);
    }

    public IEnumerator SetupCaseVideoPanel()
    {
        yield return new WaitUntil(() => SequenceManager.Instance.CaseVideoReady);

        if (SequenceManager.Instance.MediaCaseSrc.data.Count > 0)
        {
            if (SequenceManager.Instance.MediaCaseSrc.data[0].media != "")
            {
                Debug.Log("Playing Case Intro Video " + SequenceManager.Instance.MediaCaseSrc.data[0].media);
                HTMLVideoTrigger.instance.PlayVideoExternal(SequenceManager.Instance.MediaCaseSrc.data[0].media);

                yield return new WaitForSeconds(.5f);
                afterVideoFinishedPanel.SetActive(true);
                SequenceManager.Instance.CaseVideoReady = false;
            }
        }
    }

    public void NextVideoPlayer()
    {
        SequenceManager.Instance.IsMediaIntro = false;

        if (CanNextVideo())
        {
            videoIndex++;
            StartCoroutine(SetupVideoPanel());
        }
        else
        {
            CloseVideoPanel();
        }
    }

    public bool CanNextVideo()
    {
        if (videoIndex < SequenceManager.Instance.MediaSrc.data.Count - 1) return true;
        else return false;
    }

    public void CloseVideoPanel()
    {
        videoIndex = 0;
        afterVideoFinishedPanel.SetActive(false);
        StartCoroutine(PostWatchVideoIntro());

        if (_isIntro)
        {
            _isIntro = false;
            StartCoroutine(SetupCaseVideoPanel());
        }
        else
        {
            SequenceManager.Instance.IsMediaIntro = false;
        }
    }

    private IEnumerator PostWatchVideoIntro()
    {
        WWWForm form = new();
        form.AddField("ticket", SequenceManager.Instance._ticket);
        form.AddField("master_value_id", SequenceManager.Instance._masterValueById);

        Debug.Log("Getting Intro Value");

        using (UnityWebRequest webRequest = UnityWebRequest.Post(SequenceManager.Instance._server + "/api/master-value/watch-intro", form))
        {
            yield return webRequest.SendWebRequest();

            switch (webRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                    Debug.LogError("Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError("HTTP Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.Success:
                    break;
            }

            webRequest.Dispose();
        }
    }
}
