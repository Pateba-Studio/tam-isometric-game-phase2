using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class VideoController : MonoBehaviour
{
    public static VideoController instance;
    public int videoId;
    public bool setupIsDone;
    public bool isDone;

    UnityAction onVideoEndCallback;

    private void Awake()
    {
        instance = this;
    }

    public void PlayVideo(int videoId, string videoUrl, UnityAction onVideoEnd = null)
    {
        isDone = false;
        setupIsDone = false;

        onVideoEndCallback = onVideoEnd;
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

        UnityEvent events = new();
        events.AddListener(delegate
        {
            if (videoId != 0)
            {
                string json = $"{{\"ticket_number\":\"{DataHandler.instance.GetUserTicket()}\"," +
                  $"\"roleplay_question_id\":{videoId}}}";
                StartCoroutine(APIManager.instance.PostDataCoroutine(
                    APIManager.instance.SetupSubmitAnswer(),
                    json, res => { }));
                videoId = 0;
            }

            AudioManager.instance.SetAudioState(false);
            onVideoEndCallback?.Invoke();
            setupIsDone = false;
            isDone = false;
        });

        GameManager.instance.UpdateCurrentBooth(events);
    }
}
