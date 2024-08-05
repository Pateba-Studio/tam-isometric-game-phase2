using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class WaypointDetail
{
    public Transform targetPos;
    public bool isForward;
}

public class NPCStrolling : MonoBehaviour
{
    [SerializeField] public List<WaypointDetail> waypointDetails;
    public float movementSpeed = 3f;
    public float turningDelay = 2f;
    public bool isReady;

    Animator animator;
    int currentWaypointIndex = 0;

    private void Start()
    {
        animator = GetComponentInChildren<Animator>();
        StartCoroutine(DelayMovement());
    }

    private void Update()
    {
        if (isReady)
        {
            WaypointDetail wp = waypointDetails[currentWaypointIndex];
            if (Vector3.Distance(animator.transform.position, wp.targetPos.position) < 0.01f)
            {
                animator.SetTrigger("isIdle");
                StartCoroutine(DelayMovement());
            }
            else
            {
                animator.transform.position = Vector3.MoveTowards(
                    animator.transform.position,
                    wp.targetPos.position,
                    movementSpeed * Time.deltaTime);
            }
        }
    }

    public IEnumerator DelayMovement()
    {
        isReady = false;
        yield return new WaitForSeconds(turningDelay);

        if (currentWaypointIndex < waypointDetails.Count - 1) currentWaypointIndex++;
        else currentWaypointIndex--;

        if (waypointDetails[currentWaypointIndex].isForward) animator.SetTrigger("isForward");
        else animator.SetTrigger("isBackward");

        isReady = true;
    }
}
