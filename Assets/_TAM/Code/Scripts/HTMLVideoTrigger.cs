using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class HTMLVideoTrigger : MonoBehaviour
{
    public static HTMLVideoTrigger instance;

    [DllImport("__Internal")]
    private static extern void PlayVideo(string str);

    private void Awake()
    {
        instance = this;
    }

    public void PlayVideoExternal(string url)
    {
#if !UNITY_EDITOR && UNITY_WEBGL
        Debug.Log("Trying play " + url);
        PlayVideo(url);
#else
        Debug.Log("This feature will be activated in WebGL || " + url);
#endif
    }
}
