using System;
using UnityEngine;

[Serializable]
public struct Message
{
    [TextArea] public string text;
    public float secondsToContinue;
    public bool changeLevelMovingState;
    
    public Message(string text, float secondsToContinue, bool changeLevelMovingState)
    {
        this.text = text;
        this.secondsToContinue = secondsToContinue;
        this.changeLevelMovingState = changeLevelMovingState;
    }
}
