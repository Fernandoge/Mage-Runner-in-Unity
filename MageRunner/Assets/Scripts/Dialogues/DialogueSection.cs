using System;

[Serializable]
public struct DialogueSection
{
    public ChatBubbleController chatBubble;
    public Message[] messages;
}