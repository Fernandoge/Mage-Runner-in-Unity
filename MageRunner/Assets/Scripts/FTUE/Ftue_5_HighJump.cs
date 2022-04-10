using MageRunner.Dialogues;
using MageRunner.Managers.GameManager;
using UnityEngine;

namespace MageRunner.FTUE
{
    public class Ftue_5_HighJump : FtueSection
    {
        [SerializeField] private DialogueController _firstStepDialogue;
        [SerializeField] private GameObject _ftueHand;
        [SerializeField] private AnimationClip highJumpGestureAnimation;
        
        public override void FirstStep() => _firstStepDialogue.StartDialogue();
        
        // Called after the first step dialogue finishes
        public void SecondStep()
        {
            Time.timeScale = 0;
            
            Animator handAnimator = _ftueHand.GetComponent<Animator>();
            AnimatorOverrideController handAnimatorController = (AnimatorOverrideController) handAnimator.runtimeAnimatorController;
            handAnimatorController["Gestures FTUE"] = highJumpGestureAnimation;
            _ftueHand.SetActive(true);

            GameManager.Instance.player.gestureSpellsController.highJumpCallback += ThirdStep;
        }

        private void ThirdStep()
        {
            GameManager.Instance.player.gestureSpellsController.highJumpCallback -= ThirdStep;
            GameManager.Instance.level.EnableMovement();
            _ftueHand.SetActive(false);
            Time.timeScale = 1;
            
        }
    }
}
