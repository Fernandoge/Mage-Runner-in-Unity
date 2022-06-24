using MageRunner.Dialogues;
using MageRunner.Managers.FtueManager;
using MageRunner.Managers.GameManager;
using UnityEngine;
using UnityEngine.UI;

namespace MageRunner.FTUE
{
    public class Ftue_3_AirJump : FtueSection
    {
        [SerializeField] private Button _ftueJumpButton;
        [SerializeField] private DialogueController _firstStepDialogue;
        
        // Dialogue to start FTUE
        protected override void FirstStep()
        {
            GameManager.Instance.ToggleCinematicMode(true);
            _firstStepDialogue.StartDialogue();
        }

        private new void OnTriggerEnter2D(Collider2D other)
        {
            if (_ftueStarted)
                SecondStep();
            else
                base.OnTriggerEnter2D(other);
        }

        private void SecondStep()
        {
            Time.timeScale = 0;
            FtueManager.Instance.ftuePanel.SetActive(true);
            _ftueJumpButton.gameObject.SetActive(true);
            _ftueJumpButton.onClick.AddListener(ThirdStep);
            _ftueJumpButton.transform.SetParent(FtueManager.Instance.ftuePanel.transform); 
        }

        private void ThirdStep()
        {
            Time.timeScale = 1;
            GameManager.Instance.ToggleCinematicMode(false);
            FtueManager.Instance.ftuePanel.SetActive(false);
            _ftueJumpButton.onClick.RemoveListener(ThirdStep);
            
            GameManager.Instance.player.Jump();
            
            GameManager.Instance.player.companionChatBubble.StartChatCoroutine(new Message("Great!", 1.5f, 1f));
        }
    }
}
