using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class VideoController : MonoBehaviour
{
    public static VideoController instance;
    public int videoId;
    public bool nextQuestion;
    public bool setupIsDone;
    public bool isDone;

    UnityAction onVideoEndCallback;

    private void Awake()
    {
        instance = this;
    }

    public void PlayVideo(int videoId, string videoUrl, UnityAction onVideoEnd = null, bool nextQuestion = false)
    {
        isDone = false;
        setupIsDone = false;

        onVideoEndCallback = onVideoEnd;
        this.nextQuestion = nextQuestion;
        this.videoId = videoId;

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
#if UNITY_EDITOR
        isDone = true;
#endif

        yield return new WaitWhile(() => !isDone);

        if (videoId != 0)
        {
            PreloadManager.instance.SetupInitData();
            string json = $"{{\"ticket_number\":\"{DataHandler.instance.GetUserTicket()}\"," +
              $"\"roleplay_question_id\":{videoId}}}";
            StartCoroutine(APIManager.instance.PostDataCoroutine(
                APIManager.instance.SetupSubmitAnswer(),
                json, res => { }));
            videoId = 0;
        }

        if (nextQuestion)
        {
            //GameManager.instance.SetLoadingText("Please Wait For Next Question");
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
