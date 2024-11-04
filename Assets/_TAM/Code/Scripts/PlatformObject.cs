using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformObject : MonoBehaviour
{
    public GameObject objectMobile;
    public GameObject objectDesktop;

    private void OnEnable()
    {
        Check();
    }

    public void Start()
    {
        Check();
    }

    private void Check()
    {
        objectMobile.SetActive(false);
        objectDesktop.SetActive(false);

        switch (PlatformDetector.instance.isMobilePlatform)
        {
            case true:
                objectMobile.SetActive(true);
                break;
            case false:
                objectDesktop.SetActive(true);
                break;
        }
    }
}
