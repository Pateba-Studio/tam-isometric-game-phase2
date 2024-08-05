using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinigameManager : MonoBehaviour
{
    public static MinigameManager instance;

    public bool Pause;

    private void Awake() 
    {
        instance = this;    
    }

    private void Update() 
    {
        if (Pause) Time.timeScale = 0;
        else Time.timeScale = 1;    
    }
}
