using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InteractableHandler : MonoBehaviour
{
    public DialogueData dialogueData;
    [HideInInspector] public UnityEvent whenInteract;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            whenInteract.RemoveAllListeners();
            whenInteract.AddListener(() => GameManager.instance.SetDialoguePanel(dialogueData));
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
