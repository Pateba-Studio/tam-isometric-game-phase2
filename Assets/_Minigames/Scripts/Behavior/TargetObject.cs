using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TargetObject : MonoBehaviour
{
    //[SerializeField] private string _letter;
    [SerializeField] private TextMeshPro _labelLetter;
    [SerializeField] private TextMeshPro _text;
    [SerializeField] private TextMeshProUGUI _textUI;

    [Header("2 choice properties")]
    [SerializeField] private bool _isTrueFalseOnly = false;
    [SerializeField] private SpriteRenderer _panel;
    [SerializeField] private Color _yesPanelColor = Color.white;
    [SerializeField] private Color _yesTextColor = Color.white;
    [SerializeField] private Color _noPanelColor = Color.white;
    [SerializeField] private Color _noTextColor = Color.white;

    [HideInInspector] public bool isTrue;
    [HideInInspector] public int AnswerId;
    public Transform RealObject;

    public void SetText(string letter, string text)
    {
        if (_labelLetter)
        {
            if (_isTrueFalseOnly)
            {
                _labelLetter.text = text;
                if (text == "Ya")
                {
                    _labelLetter.color = _yesTextColor;
                    _panel.color = _yesPanelColor;
                }
                else
                {
                    _labelLetter.color = _noTextColor;
                    _panel.color = _noPanelColor;
                }
            }
            else
            {
                _labelLetter.text = letter;
            }
        }

        //if (_textUI) _textUI.text = letter + ". " + text;
        if (_text) _text.text = text;
    }
}
