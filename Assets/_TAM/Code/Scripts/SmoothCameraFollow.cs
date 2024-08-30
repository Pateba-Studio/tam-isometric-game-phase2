using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothCameraFollow : MonoBehaviour
{
    public Transform target;  // Target yang akan diikuti oleh kamera (misalnya karakter)
    public Vector3 offset;    // Offset dari posisi target
    public float smoothTime = 0.3f;  // Waktu untuk mencapai target posisi

    private Vector3 velocity = Vector3.zero;

    void LateUpdate()
    {
        // Tentukan posisi target kamera
        Vector3 targetPosition = target.position + offset;

        // Gunakan SmoothDamp untuk bergerak menuju target dengan halus
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
    }
}
