using MageRunner.Dialogues;
using MageRunner.Managers.GameManager;
using UnityEngine;

namespace MageRunner.FTUE
{
    public class Ftue_4_Gestures : FtueSection
    {
        [SerializeField] private DialogueController _firstStepDialogue;
        [SerializeField] private GameObject _ftueHand;
        
        public override void FirstStep()
        {
            _firstStepDialogue.StartDialogue();
        }

        // Called after the first step dialogue finishes
        public void SecondStep()
        {
            _ftueHand.SetActive(true);
        }

        public override void StepAfterDestroyed()
        {
            _ftueHand.SetActive(false);
            GameManager.Instance.level.EnableMovement();
            GameManager.Instance.player.companionChatBubble.StartChatCoroutine(new Message("wow you still got it", 1.2f, 1f));
            GameManager.Instance.player.companionChatBubble.StartChatCoroutine(new Message("prepare for more!", 1.2f, 1f));
        }
    }
}
