using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InteractableHandler : MonoBehaviour
{
    public bool isHall;
    public bool isBooth;
    public bool isOnlyGame;
    public HallBoothData hallBoothData;
    [HideInInspector] public UnityEvent whenInteract;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            whenInteract.RemoveAllListeners();
            whenInteract.AddListener(() =>
            {
                if (isHall) GameManager.instance.TeleportHall(hallBoothData.hallTargetKey);
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
