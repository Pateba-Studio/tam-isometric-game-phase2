using System;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

[Serializable]
public class TeleportData
{
    public HallState targetHallState;
    public InteractHandler interactHandler;
    public GameObject unlockedGate;
    public GameObject lockedGate;
    public GameObject missionCheckIcon;
    public Button buttonToTeleport;
    public List<GameObject> notifOnTeleportBtn;

    public bool canTeleport;
}

public class TeleportState : MonoBehaviour
{
    [SerializeField] public TeleportData teleportData;

    [Header("Add on for Hall B")]
    public string gameScene;

    public void SetCanTeleport()
    {
        teleportData.canTeleport = true;
        teleportData.lockedGate.SetActive(false);
        teleportData.unlockedGate.SetActive(true);

        if (teleportData.missionCheckIcon != null) teleportData.missionCheckIcon.SetActive(true);
        if (teleportData.buttonToTeleport != null) teleportData.buttonToTeleport.interactable = true;

        GameState.instance.SetNotifNextHall();
    }

    public void ChangeHall()
    {
        int targetId = teleportData.targetHallState.hallData.hallID;
        GameState.instance.SetupHall(targetId);
    }

    public void TeleportToHallB()
    {
        GameManager.instance.TeleportChecker(DataHolder.instance.currGameSessionDetail.hallDetails[2].hallId);
    }

    public void EnterGame()
    {
        SceneManager.LoadScene(gameScene);
    }
}
