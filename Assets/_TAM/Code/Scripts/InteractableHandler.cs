using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class InteractableHandler : MonoBehaviour
{
    public HallBoothData hallBoothData;

    [Header("Hall Attributes")]
    public string hallTargetKey;

    [Header("Booth Attributes")]
    public int boothIndex;
    public Image boothCheck;
    public GameObject boothOngoing;
    public GameObject boothIsDone;
    public GameObject missionOngoing;
    public GameObject missionIsDone;

    [Header("Conditional State")]
    public bool isHall;
    public bool isBooth;
    public bool isOnlyGame;

    [HideInInspector] public UnityEvent whenInteract;

    public void SetupBoothClear(bool clear)
    {
        if (clear)
        {
            boothCheck.sprite = GameManager.instance.boothCheckDatas[boothIndex].boothCheckDone;
            GameManager.instance.SetupInteractButton(false, null);
            GetComponent<PolygonCollider2D>().enabled = false;
        }
        else
        {
            boothCheck.sprite = GameManager.instance.boothCheckDatas[boothIndex].boothCheckUndone;
        }

        boothOngoing.SetActive(!clear);
        missionOngoing.SetActive(clear);

        boothIsDone.SetActive(clear);
        missionIsDone.SetActive(!clear);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            whenInteract.RemoveAllListeners();
            whenInteract.AddListener(() =>
            {
                if (isHall) GameManager.instance.TeleportHall(hallTargetKey);
                if (isBooth) GameManager.instance.SetupBooth(hallBoothData, true);
                if (isOnlyGame) GameManager.instance.SetupRoleplay(hallBoothData);
            });

            GameManager.instance.SetupInteractButton(true, whenInteract);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            GameManager.instance.SetupInteractButton(false, null);
        }
    }
}
