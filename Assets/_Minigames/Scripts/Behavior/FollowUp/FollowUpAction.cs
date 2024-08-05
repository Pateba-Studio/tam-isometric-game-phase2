using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class FollowUpAction : MonoBehaviour
{
    [SerializeField] private Shooting _triggerInput;
    [SerializeField] private Animator _animator;

    private Vector3 _initPos;

    private void Start()
    {
        _initPos = transform.position;
    }

    private void Update()
    {
        if (_triggerInput)
        {
            if (_triggerInput.OnShoot)
            {
                if (_animator)
                {
                    _animator.CrossFade("shoot", 0f);
                    transform.DOMoveX(_triggerInput.ClickTarget.x, 0.5f);

                    Invoke("ResetState", 1f);
                }
            }
        }
    }

    private void ResetState()
    {
        transform.position = _initPos;
        _animator.CrossFade("init", 0f);
    }
}
