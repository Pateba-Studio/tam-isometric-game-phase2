using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class VideoController : MonoBehaviour
{
    public static VideoController instance;
    public bool isDone;

    private UnityAction onVideoEndCallback;

    private void Awake()
    {
        instance = this;
    }

    public void PlayVideo(string videoUrl, UnityAction onVideoEnd = null)
    {
        onVideoEndCallback = onVideoEnd;

        Debug.Log("Trying Play: " + videoUrl);
        Application.ExternalCall("playVideoFromUnity", videoUrl);

        StartCoroutine(CheckForVideoEnd());
        StartCoroutine(AfterSeconds(.5f));
    }

    public void OnVideoEnd()
    {
        isDone = true;
    }

    private IEnumerator AfterSeconds(float second)
    {
        yield return new WaitForSeconds(second);
        onVideoEndCallback?.Invoke();
        isDone = false;
    }

    private IEnumerator CheckForVideoEnd()
    {
        while (!isDone)
        {
            yield return null;
        }

        onVideoEndCallback?.Invoke();
        isDone = false;
    }
}
