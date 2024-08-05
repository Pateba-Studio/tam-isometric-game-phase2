using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow2D : MonoBehaviour
{
    public Transform target;
    public float followSpeed = 5.0f;
    public Vector3 offsetPosition;
    public Vector3 targetPosition;

    void LateUpdate()
    {
        targetPosition = Vector3.Lerp(transform.position, target.position + offsetPosition, Time.deltaTime * followSpeed);
        targetPosition.z = transform.position.z;
        transform.position = targetPosition;
    }
}
