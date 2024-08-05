using UnityEngine;

public class IncrementalGCController : MonoBehaviour
{
#if DISABLE_GARBAGE_COLLECTION
    private void Start()
    {
        Unity.Jobs.LowLevel.Unsafe.JobsUtility.JobWorkerCount = 1;
        Debug.Log("Incremental GC disabled.");
    }
#endif
}