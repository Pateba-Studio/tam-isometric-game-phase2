using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class HallData
{
    public int hallID;
    public string HallName;
    public GameObject HallObj;
    public GameObject HallIndicatorPanel;
    public GameObject scrollView;
    public Transform spawnPoint;
    public TeleportState teleportTarget;
    public List<BoothState> booths;

    [Header("Add On For Disable Hall")]
    public GameObject selectionTeleport;
    public TeleportState departureTeleport;
    public List<QuestChecker> questCheckers;
}

public class HallState : MonoBehaviour
{
    [SerializeField] public HallData hallData;

    [Header("Condition")]
    public bool hallIsActive;

    public void SetHallState()
    {
        if (hallData.selectionTeleport != null) hallData.selectionTeleport.SetActive(hallIsActive);
        if (hallData.departureTeleport != null) hallData.departureTeleport.gameObject.SetActive(hallIsActive);
        if (!hallIsActive) hallData.questCheckers?.ForEach(quest => quest.checkIcon.SetActive(!hallIsActive));
    }
}
