using System;
using UnityEditor;
using UnityEngine;

[Serializable]
public struct DialogueSection
{
    [Header("If")]
    public bool isCompanionChatBubble;
    [Header("Else")]
    public ChatBubbleController chatBubble;
    [Header("")]
    public Message[] messages;
}