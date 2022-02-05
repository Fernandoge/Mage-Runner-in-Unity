using MageRunner.Dialogues;
using MageRunner.Managers.FtueManager;
using MageRunner.Managers.GameManager;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MageRunner.FTUE
{
    public class Ftue_1_JumpButton : FtueSection
    {
        [SerializeField] private Button _ftueJumpButton;
        [SerializeField] private DialogueController _firstStepDialogue;

        // Dialogue to start FTUE
        public override void FirstStep()
        {
            GameManager.Instance.ToggleCinematicMode(true);
            _firstStepDialogue.StartDialogue();
        }
        
        // Highlighting jump button and activating the FTUE hand
        // Called in the last message of FirstStep dialogue
        public void SecondStep()
        {
            GameManager.Instance.ToggleCinematicMode(false);
            FtueManager.Instance.ftuePanel.SetActive(true);
            _ftueJumpButton.gameObject.SetActive(true);
            _ftueJumpButton.onClick.AddListener(ThirdStep);
            _ftueJumpButton.transform.SetParent(FtueManager.Instance.ftuePanel.transform); 
        }

        // Jump button was pressed
        // Called in Jump Button FTUE GameObject
        public void ThirdStep()
        {
            GameManager.Instance.level.EnableMovement();
            GameManager.Instance.player.companionChatBubble.ForceClose();
            FtueManager.Instance.ftuePanel.SetActive(false);
            _ftueJumpButton.gameObject.SetActive(false);
            _ftueJumpButton.onClick.RemoveListener(ThirdStep);
            
            GameManager.Instance.player.Jump();
            
            GameManager.Instance.player.companionChatBubble.StartChatCoroutine(new Message("that was good", 1.5f, 1f));
        }
    }
}
