using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ObjectScroll : MonoBehaviour
{
    [SerializeField] private float _duration;
    [SerializeField] private Ease _easing;
    [SerializeField] private float _standPointX;
    [SerializeField] private Vector2 _scaleEndValue;
    [SerializeField] private Vector2 _positionEndValue;

    private bool _pause;

    private void Start() 
    {
        gameObject.SetActive(false);
    }

    private void BeginShow()
    {
        gameObject.SetActive(true);
        
        Vector3 vec3ScaleEnd = new Vector3(_scaleEndValue.x, _scaleEndValue.y, transform.localScale.z);
        transform.DOScale(vec3ScaleEnd, _duration)
            .SetEase(_easing);

        Vector3 vec3PositionEnd = new Vector3(_positionEndValue.x, _positionEndValue.y, transform.position.z);
        transform.DOMove(vec3PositionEnd, _duration)
            .SetEase(_easing).OnComplete(CompleteCallback);
    }

    private void CompleteCallback()
    {
        gameObject.SetActive(false);
    }

    public float GetStandX()
    {
        return _standPointX;
    }
}
