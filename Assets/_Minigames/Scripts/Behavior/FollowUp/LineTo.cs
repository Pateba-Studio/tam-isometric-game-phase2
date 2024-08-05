using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineTo : MonoBehaviour
{
    [SerializeField] private LineRenderer _lineRenderer;
    [SerializeField] private Transform _lineFrom;
    [SerializeField] private Transform _lineTo;

    private void Update() 
    {
        _lineRenderer.positionCount = 2;
        _lineRenderer.SetPosition(0, _lineFrom.position);
        _lineRenderer.SetPosition(1, _lineTo.position);
    }
}
