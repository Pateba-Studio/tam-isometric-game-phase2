using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InteractHandler : MonoBehaviour
{
    public bool isInteractable;
    public UnityEvent ClickListeners;

    [Header("Add On For Booth Before Checker")]
    public UnityEvent IfBoothBeforeDidntFinish;

    [Header("Add On For Teleport")]
    public bool isTeleport;
    public TeleportState teleportState;

    void Awake()
    {
        if (!GetComponent<SpriteRenderer>()) return;
        GetComponent<SpriteRenderer>().enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isInteractable) return;

        if (collision.CompareTag("Shadow"))
        {
            if (isTeleport)
            {
                teleportState = GetComponent<TeleportState>();

                if (teleportState.teleportData.canTeleport)
                    GameManager.instance.unlockedPanel.SetActive(true);
                else
                    GameManager.instance.lockedPanel.SetActive(true);

                GameManager.instance.interactTeleportPanel.SetActive(true);
                GameManager.instance.buttonSubmitInteractTeleport.onClick.RemoveAllListeners();
                GameManager.instance.buttonSubmitInteractTeleport.onClick.AddListener(delegate
                {
                    GameManager.instance.interactTeleportPanel.SetActive(false);
                    GameManager.instance.unlockedPanel.SetActive(false);
                    GameManager.instance.lockedPanel.SetActive(false);

                    ClickListeners.Invoke();
                });
            }
            else
            {
                BoothState booth = GetComponent<BoothState>();
                //GameManager.instance.interactAnim.SetTrigger("IsPopUp");
                GameManager.instance.interactAnim.gameObject.SetActive(true);

                if (booth.CheckAllBoothStateBefore())
                {
                    GameManager.instance.currentInteractButton.onClick.AddListener(delegate { ClickListeners.Invoke(); });
                }
                else
                {
                    GameManager.instance.currentInteractButton.onClick.AddListener(delegate { IfBoothBeforeDidntFinish.Invoke(); });
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!isInteractable) return;

        if (collision.CompareTag("Shadow"))
        {
            if (!isTeleport)
            {
                //GameManager.instance.interactAnim.SetTrigger("IsPopOut");
                GameManager.instance.interactAnim.gameObject.SetActive(false);
                GameManager.instance.currentInteractButton.onClick.RemoveAllListeners();
            }
            else
            {
                GameManager.instance.interactTeleportPanel.SetActive(false);
                GameManager.instance.unlockedPanel.SetActive(false);
                GameManager.instance.lockedPanel.SetActive(false);
            }
        }
    }
}
