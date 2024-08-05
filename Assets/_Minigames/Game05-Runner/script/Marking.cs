using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Marking : MonoBehaviour
{
    [SerializeField] private Animator _resultAnimator;
    [SerializeField] private SpriteRenderer _runnerSprite;
    [SerializeField] private ButtonAnswer _buttonAnswerGroup;

    private Ray _ray;
    private bool _ready;

    void Start()
    {
        _buttonAnswerGroup.gameObject.SetActive(false);

        Invoke("PauseScroll", 3f);
    }

    private void Update()
    {
        /**
        if (!SequenceManager.Instance.IsMediaIntro)
        {
            if (Input.GetMouseButtonDown(0) && _ready)
            {
                Vector3 mousePosWithDepth = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane);
                _ray = Camera.main.ScreenPointToRay(mousePosWithDepth);

                RaycastHit hit;
                if (Physics.Raycast(_ray, out hit))
                {
                    _ready = false;

                    ObjectScroll targetObjScroll = hit.transform.GetComponent<ObjectScroll>();
                    if (targetObjScroll)
                    {
                        ResumeScroll(targetObjScroll);
                    }
                }
            }
        }
        */
    }

    private void PauseScroll()
    {
        if (_runnerSprite)
        {
            _runnerSprite.color = new Color(1f, 1f, 1f, 0.5f);
        }

        _ready = true;
        MinigameManager.instance.Pause = true;
        _buttonAnswerGroup.gameObject.SetActive(true);
    }

    public void ResumeScroll(ObjectScroll objScroll)
    {
        if (_runnerSprite)
        {
            _runnerSprite.transform.position = new Vector3(
                 objScroll.GetStandX(),
                _runnerSprite.transform.position.y,
                _runnerSprite.transform.position.z
            );
            _runnerSprite.color = new Color(1f, 1f, 1f, 1f);
        }
        MinigameManager.instance.Pause = false;
        _buttonAnswerGroup.gameObject.SetActive(false);

        TargetObject targetObj = objScroll.transform.GetComponent<TargetObject>();
        if (targetObj) CompareResult(targetObj);
    }

    private void CompareResult(TargetObject targetObj)
    {
        StartCoroutine(ShowResult(targetObj.isTrue, targetObj));

        targetObj.gameObject.SetActive(false);
    }
    
    private IEnumerator ShowResult(bool isTrue, TargetObject targetObj)
    {
        yield return new WaitForSeconds (3f);

        if (_resultAnimator)
        {
            if (SequenceManager.Instance.ScoreType == "point")
            {
                _resultAnimator.CrossFade("result_saved", 0f);
            }
            else
            {
                if (isTrue) _resultAnimator.CrossFade("result_right", 0f);
                else _resultAnimator.CrossFade("result_wrong", 0f);
            }

            if (SequenceManager.Instance.IsSequence)
            {
                if (SequenceManager.Instance._currentQuestionId == SequenceManager.Instance.GetQuestionCount())
                    Invoke("InvokeStageClear", 1.5f);
            }

            SequenceManager.Instance.NextQuestion(targetObj.AnswerId, 1f);
        }
    }

    private void InvokeStageClear()
    {
        _resultAnimator.transform.GetComponent<ResultAnimationAssetSource>().ReplaceSprite(SequenceManager.Instance.HallBId);
        _resultAnimator.CrossFade("result_clear", 0f);
    }
}

