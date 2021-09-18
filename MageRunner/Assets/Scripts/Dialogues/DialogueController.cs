using System.Collections.Generic;
using System.Runtime.InteropServices;
using MageRunner.Managers.GameManager;
using UnityEngine;

namespace MageRunner.Dialogues
{
    public class DialogueController : MonoBehaviour
    {
        [SerializeField] private List<DialogueSection> _dialogueSections = new List<DialogueSection>();

        private int _currentSectionIndex;
        
        private void OnTriggerEnter2D(Collider2D col)
        {
            if (col.gameObject.layer != LayerMask.NameToLayer("Player")) 
                return;

            // Always reset the index if the player collides with the trigger since for now it will always means a checkpoint respawn
            _currentSectionIndex = 0;

            StartDialogue();
        }

        public void StartDialogue()
        {
            GameManager.Instance.dialoguePlaying = this;

            if (_dialogueSections[_currentSectionIndex].isCompanionChatBubble)
            {
                DialogueSection dialogueSectionCopy = _dialogueSections[_currentSectionIndex];
                dialogueSectionCopy.chatBubble = GameManager.Instance.player.companionChatBubble;
                _dialogueSections[_currentSectionIndex] = dialogueSectionCopy;
            }                                                                                                                           

            if (_dialogueSections[_currentSectionIndex].chatBubble.chatBubbleText.gameObject.activeSelf)
                _dialogueSections[_currentSectionIndex].chatBubble.ForceClose();

            foreach (Message message in _dialogueSections[_currentSectionIndex].messages)
            {
                _dialogueSections[_currentSectionIndex].chatBubble.StartChatCoroutine(message);
            }
        }

        public void NextSection()
        {
            if (_currentSectionIndex == _dialogueSections.Count)
            {
                StartDialogue();
                _currentSectionIndex += 1;
            }
            else
                GameManager.Instance.dialoguePlaying = null;
        }
    }
}
