using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public class ChatBoxHandler : MonoBehaviour
{
    public static ChatBoxHandler instance;
    public GameObject chatboxPanel;
    public GameObject inGamePanel;

    [Header("Sprite Attributes")]
    public bool npcFlip;
    public Sprite npcSprite;
    public Sprite playerSprite;
    public Image playerImagePlaceHolder;
    public Image npcImagePlaceHolder;
    [SerializeField] public List<Sprite> playerSprites;

    [Header("Content Attribute")]
    public GameObject previousBTN;
    public GameObject nextBTN;
    public Text contentText;
    [SerializeField] public List<ChatBoxDetail> chatboxDetails;

    [Header("MultipleChoice Attribute")]
    public ChatBoxScriptableObject currChatBoxContent;
    public ContentHandler contentHandler;
    //public MultipleChoiceHandler multipleChoiceHandler;

    int index;

    private void Start()
    {
        instance = this;
    }

    void Update()
    {
        if (index == 0) previousBTN.SetActive(false);
        else if (index > 0) previousBTN.SetActive(true);
    }

    public void PrevConversation()
    {
        index--;
        SpawnConversation();
    }

    public void NextConversation()
    {
        index++;
        SpawnConversation();
    }

    public void SetupConversation(ChatBoxScriptableObject chatBox)
    {
        index = 0;
        currChatBoxContent = chatBox;

        if (DataHolder.instance.GetCurrentLanguage() == LangID.EN) chatboxDetails = currChatBoxContent.chatboxEnDetails;
        else chatboxDetails = currChatBoxContent.chatboxIdDetails;

        npcSprite = currChatBoxContent.npcSprite;
        npcFlip = currChatBoxContent.npcFlip;

        playerImagePlaceHolder.sprite = playerSprite; 
        npcImagePlaceHolder.sprite = npcSprite; 
        if (npcFlip) npcImagePlaceHolder.rectTransform.localRotation = Quaternion.Euler(0, 180, 0);

        GameManager.instance.movement.IsWork = false;
        chatboxPanel.SetActive(true);
        inGamePanel.SetActive(false);
        SpawnConversation();
    }

    public void SpawnConversation()
    {
        if (chatboxDetails != null && index < chatboxDetails.Count)
        {
            contentText.text = chatboxDetails[index].contentText;

            if (chatboxDetails[index].npcTurn)
            {
                npcImagePlaceHolder.gameObject.SetActive(true);
                playerImagePlaceHolder.gameObject.SetActive(false);
            }
            else
            {
                npcImagePlaceHolder.gameObject.SetActive(false);
                playerImagePlaceHolder.gameObject.SetActive(true);
            }
        }
        else
        {
            inGamePanel.SetActive(true);
            chatboxPanel.SetActive(false);
            contentHandler.SetupContent();
        }
    }
}
