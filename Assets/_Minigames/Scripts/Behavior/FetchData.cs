using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FetchData : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _textQuestion;
    [SerializeField] private TargetObject[] _targets;
    [SerializeField] private RectTransform[] _answerUI;
    List<TargetObject> _tempoObjects;

    [SerializeField] private bool _ingameCanvas;

    [SerializeField] private bool _isTwoAnswerExclusive = false;

    private string[] _letters;

    private void Start() 
    {
        _letters = new string[]{"A","B","C","D"};

        Invoke("Reload", 0.5f);
    }

    private void Reload()
    {
        Structures.QuestionData questionObj = null;
        if (SequenceManager.Instance != null) 
            questionObj = SequenceManager.Instance.GetQuestion();

        if (questionObj != null)
        {
            // Debug.Log(questionObj.game_type);
            SequenceManager.Instance.ScoreType = questionObj.type;
            _textQuestion.text = questionObj.question;

            int startIndex = 0;
            if (questionObj.answers.Length < 4) startIndex = 1;
            if (_isTwoAnswerExclusive) startIndex = 0;

            List<TargetObject> picked = new List<TargetObject>();
            for (int i = 0; i < _targets.Length; i++)
            {
                if (i >= startIndex && i < startIndex + questionObj.answers.Length)
                {
                    _targets[i].gameObject.SetActive(true);
                    picked.Add(_targets[i]);
                }
                else _targets[i].gameObject.SetActive(false);
            }

            for (int i = 0; i < picked.Count; i++)
            {
                if (picked[i].GetComponent<Animator>()) picked[i].GetComponent<Animator>().CrossFade("standing", 0);
                picked[i].SetText(_letters[i], questionObj.answers[i].answer);
                picked[i].AnswerId = questionObj.answers[i].id;
                if (questionObj.answer_correct.id == questionObj.answers[i].id)
                {
                    SequenceManager.Instance.CorrectLetter = _letters[i];
                    picked[i].isTrue = true; 
                }
                if (_answerUI.Length >= picked.Count)
                {
                    _answerUI[i].gameObject.SetActive(true);
                    if (questionObj.answers[i].answer.Length < 20)
                    {
                        RectTransform uiRectTransform = _answerUI[i].gameObject.GetComponent<RectTransform>();
                        uiRectTransform.sizeDelta = new Vector3(
                            300f,
                            uiRectTransform.sizeDelta.y
                            );
                    }
                    _answerUI[i].GetChild(0).GetComponent<TextMeshProUGUI>().text = _letters[i] + ". " + questionObj.answers[i].answer;
                }
            }
        }
        else
        {
            Invoke("Reload", 0.5f);
        }
    }
}
