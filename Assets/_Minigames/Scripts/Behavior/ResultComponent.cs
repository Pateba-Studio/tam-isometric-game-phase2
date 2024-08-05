using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ResultComponent : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _correctLetter;
    [SerializeField] private TextMeshProUGUI _correctText;

    public void SetAnswerText(string answerLetter, string answerText)
    {
        _correctLetter.text = answerLetter;
        _correctText.text = answerText;
    }
}
