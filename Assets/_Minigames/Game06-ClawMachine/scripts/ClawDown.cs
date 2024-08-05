using UnityEngine;
using DG.Tweening;

public class ClawDown : MonoBehaviour
{
    [SerializeField] private Shooting _triggerInput;
    [SerializeField] private Transform _armExtend;
    [SerializeField] private Transform _thisTransform;
    
    private float _initY;
    // private Transform _crawling;
    private bool extending = false;

    private void Start()
    {
        _initY = _armExtend.position.y;
    }

    private void Update()
    {
        if (!extending)
        {
            if (_triggerInput != null)
            {
                if (_triggerInput.OnShoot)
                {
                    ExtendArm();
                }
            }
        }

        /**
        if (_crawling != null)
        {
            _crawling.position = _armExtend.position;
        }
        */
    }

    private void ExtendArm()
    {
        extending = true;
        float distance = _thisTransform.position.y - _triggerInput.ClickedObject.position.y;

        _thisTransform.DOKill();
        _armExtend.DOLocalMoveX(_triggerInput.ClickedObject.position.x, 0.9f);
        _armExtend.DOLocalMoveY(-1f * distance, 1f)
            .OnComplete(() => { _armExtend.DOKill(); });

        // Invoke("PullArm", 1f);
    }

    /**
    private void PullArm()
    {
        transform.DOKill();
        _crawling = _triggerInput.ClickedObject;
        _armExtend.DOLocalMoveY(-1f * _initY, 5f);
    }
    */
}
