using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class VideoController : MonoBehaviour
{
    public static VideoController instance;
    public bool nextQuestion;
    public bool setupIsDone;
    public bool isDone;

    private UnityAction onVideoEndCallback;

    private void Awake()
    {
        instance = this;
    }

    public void PlayVideo(string videoUrl, UnityAction onVideoEnd = null, bool nextQuestion = false)
    {
        isDone = false;
        setupIsDone = false;

        onVideoEndCallback = onVideoEnd;
        this.nextQuestion = nextQuestion;

        AudioManager.instance.SetAudioState(true);
        Application.ExternalCall("playVideoFromUnity", videoUrl);
        StartCoroutine(CheckForVideoEnd());
    }

    public void OnVideoEnd()
    {
        if (!setupIsDone)
        {
            setupIsDone = true;
            isDone = true;
        }   
    }

    private IEnumerator CheckForVideoEnd()
    {
        while (!isDone)
        {
            yield return null;
        }

        if (nextQuestion)
        {
            GameManager.instance.SetLoadingText("Please Wait For Next Question");
            GameManager.instance.loadingPanel.SetActive(true);
            yield return new WaitForSeconds(2.5f);
            GameManager.instance.loadingPanel.SetActive(false);
        }

        AudioManager.instance.SetAudioState(false);
        onVideoEndCallback?.Invoke();
        setupIsDone = false;
        isDone = false;
    }
}
