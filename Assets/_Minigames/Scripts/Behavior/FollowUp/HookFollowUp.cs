using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class HookFollowUp : MonoBehaviour
{
    [SerializeField] private Shooting _triggerInput;
    [SerializeField] private Animator _animator;

    public float DiveTime { get; private set; }

    private Vector3 _initPos;

    private void Start() {
        DiveTime = 0.5f;
    }

    private void Update() 
    {
        if (_triggerInput)
        {
            if (_triggerInput.OnShoot)
            {
                if (_animator)
                {
                    _animator.speed = 0f;
                }

                _initPos = transform.position;
                transform.DOMove(_triggerInput.ClickTarget, DiveTime);
                Invoke("Pull", DiveTime);
            }
        }    
    }

    private void Pull()
    {
        DOTween.Kill(transform);
        transform.DOMove(_initPos + Vector3.down * 1.5f, 3f);
        Invoke("KillTween" , 1f);
    }

    private void KillTween()
    {
        DOTween.Kill(transform);
    }
}
