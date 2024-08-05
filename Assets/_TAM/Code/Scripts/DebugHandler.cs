using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugHandler : MonoBehaviour
{
    public static DebugHandler instance;

    public GameObject debugPanel;
    public Text debugText;

    private void Awake()
    {
        instance = this;    
    }
}
