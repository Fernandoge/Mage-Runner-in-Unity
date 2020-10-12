using System;
using System.Collections.Generic;
using UnityEngine;

public class DialogueController : MonoBehaviour
{
    [SerializeField] private List<DialogueSection> _dialogueSections = new List<DialogueSection>();
    
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.layer != LayerMask.NameToLayer("Player")) 
            return;
        
        StartDialogue();
    }

    private void StartDialogue()
    {
        GameManager.Instance.dialoguePlaying = this;
        
        if (_dialogueSections[0].chatBubble.chatBubbleText.gameObject.activeSelf)
            _dialogueSections[0].chatBubble.ForceClose();

        foreach (Message message in _dialogueSections[0].messages)
        {
            _dialogueSections[0].chatBubble.StartChat(message);
        }
    }

    public void NextSection()
    {
        _dialogueSections.RemoveAt(0);
        if (_dialogueSections.Count > 0)
            StartDialogue();
        else
            GameManager.Instance.dialoguePlaying = null;
    }
}
