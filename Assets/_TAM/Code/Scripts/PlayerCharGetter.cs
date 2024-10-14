using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class PlayerCharGetter : MonoBehaviour
{
    void Start()
    {
        GetComponent<Image>().sprite = 
            GameManager.instance.charSprites
            [GameManager.instance.currentCharIndex];
    }
}
