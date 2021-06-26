using MageRunner.Dialogues;
using MageRunner.Managers.FtueManager;
using MageRunner.Managers.GameManager;
using UnityEngine;
using UnityEngine.UI;

namespace MageRunner.FTUE
{
    public class Ftue_3_AirJump : FtueSection
    {
        [SerializeField] private Selectable _jumpButton;
        [SerializeField] private Button _ftueJumpButton;
        [SerializeField] private DialogueController _firstStepDialogue;
        
        // Dialogue to start FTUE
        public override void FirstStep()
        {
            _jumpButton.gameObject.SetActive(false);
            _firstStepDialogue.StartDialogue();
        }
        
        private void OnTriggerEnter2D(Collider2D other) => SecondStep();

        private void SecondStep()
        {
            Time.timeScale = 0;
            FtueManager.Instance.ftuePanel.SetActive(true);
            _ftueJumpButton.gameObject.SetActive(true);
            _ftueJumpButton.onClick.AddListener(ThirdStep);
            _ftueJumpButton.transform.SetParent(FtueManager.Instance.ftuePanel.transform); 
        }

        
        public void ThirdStep()
        {
            Time.timeScale = 1;
            FtueManager.Instance.ftuePanel.SetActive(false);
            _ftueJumpButton.onClick.RemoveListener(ThirdStep);
            _jumpButton.gameObject.SetActive(true);
            
            GameManager.Instance.player.Jump();
            
            GameManager.Instance.player.companionChatBubble.StartChatCoroutine(new Message
                ("Great!", 1.5f, 1f, false, false, false, null));
        }
    }
}
