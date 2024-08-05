using UnityEngine;
using UnityEngine.Events;
using System.Runtime.InteropServices;

public class PlatformDetector : MonoBehaviour
{
    public static PlatformDetector Instance;

    public bool isDone;
    public bool onDevelopment;
    public bool isMobilePlatform;
    public UnityEvent WhenMobileUsed;
    public UnityEvent WhenDesktopUsed;

    [DllImport("__Internal")]
    private static extern bool IsMobile();

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
#if !UNITY_EDITOR && UNITY_WEBGL
        isMobilePlatform = IsMobile();
#endif

        if (onDevelopment)
        {
            isMobilePlatform = true;
            return;
        }

        isDone = true;
    }
}