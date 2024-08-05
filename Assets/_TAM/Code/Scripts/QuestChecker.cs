using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class QuestChecker : MonoBehaviour
{
    public HallState hallState;
    public GameObject checkIcon;

    [Header("Add on for halls")]
    public TeleportState teleportCanBeOpened;

    [Header("Add on for booths")]
    public Transform teleportPos;
    public List<TextMeshProUGUI> informationText;

    [Header("Add on for hall B")]
    public UnityEvent teleportToHallB;
    public bool isTeleportToB;

    private void Start()
    {
        if (teleportPos != null)
            GetComponentInChildren<Button>().onClick.AddListener(() => Teleport());

        if (isTeleportToB)
            GetComponentInChildren<Button>().onClick.AddListener(() => teleportToHallB.Invoke());
    }

    public void SetBoothName(string boothName)
    {
        foreach (TextMeshProUGUI text in informationText)
        {
            text.text = text.text.Replace("{boothName}", boothName);
        }
    }

    public void Teleport()
    {
        GameManager.instance.SetQuestPanelState();
        FindObjectOfType<Simple2DMovement>().transform.position = teleportPos.position;
    }

    public void SetCheckIconState(bool cond)
    {
        SetHallTeleportState();
        checkIcon.SetActive(cond);
        if (teleportCanBeOpened != null) teleportCanBeOpened.SetCanTeleport();
    }

    public bool GetCheckIconState()
    {
        return checkIcon.activeSelf;
    }

    void SetHallTeleportState()
    {
        if (hallState == null) return;
        if (hallState.hallData.teleportTarget == null) return;

        bool isDone = true;
        foreach (BoothState boothState in hallState.hallData.booths)
        {
            if (!boothState.boothData.isDone)
            {
                isDone = false;
                break;
            }
        }

        if (isDone)
        {
            hallState.hallData.teleportTarget.SetCanTeleport();
        }
    }
}
