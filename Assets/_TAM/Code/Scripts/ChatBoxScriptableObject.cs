using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[Serializable]
public class ChatBoxDetail
{
    [TextArea(5, 5)] public string contentText;
    public bool npcTurn;
}

[CreateAssetMenu(fileName = "Chat Box Detail", menuName = "ScriptableObjects/MakeChatBoxContent", order = 1)]
public class ChatBoxScriptableObject : ScriptableObject
{
    [Header("ChatBox Attribute")]
    public bool npcFlip;
    public Sprite npcSprite;
    [SerializeField] public List<ChatBoxDetail> chatboxEnDetails;
    [SerializeField] public List<ChatBoxDetail> chatboxIdDetails;
}