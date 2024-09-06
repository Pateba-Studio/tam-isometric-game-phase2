using UnityEngine;
using UnityEngine.Events;
using System.Runtime.InteropServices;

public class PlatformDetector : MonoBehaviour
{
    public bool onDevelopment;
    public UnityEvent WhenMobileUsed;
    public UnityEvent WhenDesktopUsed;
    bool isMobilePlatform;

    [DllImport("__Internal")]
    private static extern bool IsMobile();

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