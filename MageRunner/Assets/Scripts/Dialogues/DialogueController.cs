using System.Collections.Generic;
using MageRunner.Managers.GameManager;
using UnityEngine;

namespace MageRunner.Dialogues
{
    public class DialogueController : MonoBehaviour
    {
        [SerializeField] private bool _startsNewFtue;
        [SerializeField] private List<DialogueSection> _dialogueSections = new List<DialogueSection>();
    
        private void OnTriggerEnter2D(Collider2D col)
        {
            if (col.gameObject.layer != LayerMask.NameToLayer("Player")) 
                return;
        
            StartDialogue();
        }

        public void StartDialogue()
        {
            GameManager.Instance.dialoguePlaying = this;

            if (_dialogueSections[0].isCompanionChatBubble)
            {
                DialogueSection dialogueSectionCopy = _dialogueSections[0];
                dialogueSectionCopy.chatBubble = GameManager.Instance.player.companionChatBubble;
                _dialogueSections[0] = dialogueSectionCopy;
            }

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
            {
                GameManager.Instance.dialoguePlaying = null;
                if (_startsNewFtue)
                    GameManager.Instance.ftueController.NextFtueStep();
            }
        }
    }
}
