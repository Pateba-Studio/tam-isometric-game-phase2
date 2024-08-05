using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using static UnityEngine.GraphicsBuffer;

public class Shooting : MonoBehaviour
{
    [SerializeField] private Animator _resultAnimator;

    private Ray _ray;
    private bool _ready;

    private float _delay = 1;

    private bool _markOnShoot;
    public bool OnShoot { get; private set; }
    public Vector3 ClickTarget { get; private set; }
    public Transform ClickedObject { get; private set; }

    private bool _letterSet;

    void Start()
    {
        _ready = true;
    }

    private void FixedUpdate() 
    {
        if (_markOnShoot)
        {
            _markOnShoot = false;
            OnShoot = true;
        }
    }

    private void Update()
    {
       if (!SequenceManager.Instance.IsMediaIntro)
        {
            if (Input.GetMouseButtonDown(0) && _ready)
            {
                Vector3 mousePosWithDepth = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane);
                _ray = Camera.main.ScreenPointToRay(mousePosWithDepth);

                Debug.Log("Shooting");

                RaycastHit hit;
                if (Physics.Raycast(_ray, out hit))
                {
                    _ready = false;

                    //  Notify shoot trigger
                    _markOnShoot = true;
                    ClickedObject = hit.transform;
                    ClickTarget = hit.transform.position;

                    // Change ClickedObject
                    if (hit.transform.GetComponent<TargetObject>())
                    {
                        if (hit.transform.GetComponent<TargetObject>().RealObject != null)
                        {
                            ClickedObject = hit.transform.GetComponent<TargetObject>().RealObject;
                        }
                    }

                    //  Determine right answers
                    TargetObject target = hit.collider.gameObject.GetComponent<TargetObject>();

                    if (target.GetComponent<Animator>())
                    {
                        target.GetComponent<Animator>().CrossFade("falling", 0);
                    }

                    StartCoroutine(ShowResult(target.isTrue, target.AnswerId));

                    

                    //Invoke("Reload", 3f);
                }
            }
        }
    }

    private void LateUpdate() 
    {
        if (OnShoot) OnShoot = false;
    }

    private IEnumerator ShowResult(bool isTrue, int targetAnswerId)
    {
        if (SequenceManager.Instance.GetCorrectAnswer() != null)
        {
            if (SequenceManager.Instance.GetCorrectAnswer() != "")
            {
                _resultAnimator.GetComponent<ResultComponent>().SetAnswerText(SequenceManager.Instance.CorrectLetter, SequenceManager.Instance.GetCorrectAnswer());
            }
        }

        yield return new WaitForSeconds (1f);

        if (_resultAnimator)
        {
            if (SequenceManager.Instance.ScoreType == "point")
            {
                _resultAnimator.CrossFade("result_saved", 0f);
            }
            else
            {
                if (isTrue)
                {
                    _resultAnimator.CrossFade("result_right", 0f);
                }
                else
                {
                    _resultAnimator.CrossFade("result_wrong", 0f);
                    _delay += 5f;
                    Invoke("InvokeShowCorrect", 1.5f);
                }
            }

            if (SequenceManager.Instance.IsSequence)
            {
                if (SequenceManager.Instance._currentQuestionId == SequenceManager.Instance.GetQuestionCount())
                Invoke("InvokeStageClear", 1.5f);
            }

            SequenceManager.Instance.NextQuestion(targetAnswerId, _delay);
        }
    }

    private void InvokeShowCorrect()
    {
        _resultAnimator.CrossFade("result_showcorrect", 0f);
    }

    private void InvokeStageClear()
    {
        _resultAnimator.transform.GetComponent<ResultAnimationAssetSource>().ReplaceSprite(SequenceManager.Instance.HallBId);
        _resultAnimator.CrossFade("result_clear", 0f);
    }
}
