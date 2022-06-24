using MageRunner.Dialogues;
using MageRunner.Managers.GameManager;
using UnityEngine;

namespace MageRunner.FTUE
{
    public class Ftue_4_Gestures : FtueSection
    {
        [SerializeField] private DialogueController _firstStepDialogue;
        [SerializeField] private GameObject _ftueHand;
        [SerializeField] private AnimationClip gesturesFtueAnimation;

        protected override void FirstStep() => _firstStepDialogue.StartDialogue();

        // Called after the first step dialogue finishes
        public void SecondStep()
        {
            Animator handAnimator = _ftueHand.GetComponent<Animator>();
            AnimatorOverrideController handAnimatorController = (AnimatorOverrideController) handAnimator.runtimeAnimatorController;
            handAnimatorController["Gestures FTUE"] = gesturesFtueAnimation;
            _ftueHand.SetActive(true);
        }

        public override void StepAfterDestroyed()
        {
            _ftueHand.SetActive(false);
            
            GameManager.Instance.level.EnableMovement();
            GameManager.Instance.player.companionChatBubble.StartChatCoroutine(new Message("wow you still got it", 1.2f, 1f));
        }
    }
}
