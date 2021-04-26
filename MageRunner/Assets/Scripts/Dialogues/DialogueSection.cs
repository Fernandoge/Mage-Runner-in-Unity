using System;
using UnityEngine;

namespace MageRunner.Dialogues
{
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
}