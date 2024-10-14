using UnityEngine;
using UnityEngine.Events;
using System.Runtime.InteropServices;

public class PlatformDetector : MonoBehaviour
{
    public static PlatformDetector instance;

    public bool onDevelopment;
    public bool isMobilePlatform;
    public UnityEvent WhenMobileUsed;
    public UnityEvent WhenDesktopUsed;

    [DllImport("__Internal")]
    private static extern bool IsMobile();

    private void Awake()
    {
        instance = this;
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

        if (isMobilePlatform) WhenMobileUsed.Invoke();
        else WhenDesktopUsed.Invoke();
    }
}