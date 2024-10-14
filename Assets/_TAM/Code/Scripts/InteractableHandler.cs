using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InteractableHandler : MonoBehaviour
{
    public HallBoothData hallBoothData;

    [Header("Hall Attributes")]
    public string hallTargetKey;

    [Header("Game-Booth Attributes")]
    public GameObject boothOngoing;
    public GameObject boothIsDone;
    public List<GameBoothId> gameBooths;

    [Header("Conditional State")]
    public bool isHall;
    public bool isBooth;
    public bool isOnlyGame;

    [HideInInspector] public UnityEvent whenInteract;

    public void SetupBoothClear(bool clear)
    {
        if (clear)
        {
            GameManager.instance.SetupInteractButton(false, null);
            GetComponent<PolygonCollider2D>().enabled = false;
        }

        boothOngoing.SetActive(!clear);
        boothIsDone.SetActive(clear);
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
