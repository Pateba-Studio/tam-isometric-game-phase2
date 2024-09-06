using UnityEngine;

public class VideoController : MonoBehaviour
{
    public static VideoController instance;
    public bool isDone;

    private void Awake()
    {
        instance = this;    
    }

    public void OnVideoEnd()
    {
        Debug.Log("Video has ended, triggered from HTML!");
        isDone = true;
    }

    public void PlayVideo(string videoUrl)
    {
        Application.ExternalCall("showVideoPlayer", videoUrl);
#if UNITY_EDITOR
        isDone = true;
#endif
    }

    public void ShowVideoAlbum(string[] videoUrl)
    {
        Application.ExternalCall("showVideoPlayer", videoUrl);
    }
}
