using System.Collections.Generic;
using GestureRecognizer;
using MageRunner.Dialogues;
using MageRunner.Managers.GameManager;
using UnityEngine;

namespace MageRunner.FTUE
{
    public class Ftue_5_HighJump : FtueSection
    {
        [SerializeField] private DialogueController _firstStepDialogue;
        [SerializeField] private GameObject _ftueHand;
        [SerializeField] private AnimationClip _highJumpGestureAnimation;
        [SerializeField] private GesturePattern _ftuePattern;
        
        private List<GesturePattern> _activePatterns;

        protected override void FirstStep() => _firstStepDialogue.StartDialogue();
        
        // Called after the first step dialogue finishes
        public void SecondStep()
        {
            Animator handAnimator = _ftueHand.GetComponent<Animator>();
            AnimatorOverrideController handAnimatorController = (AnimatorOverrideController) handAnimator.runtimeAnimatorController;
            handAnimatorController["Gestures FTUE"] = _highJumpGestureAnimation;
            _ftueHand.SetActive(true);

            UnlockBasicSpell(_ftuePattern);
            _activePatterns = GameManager.Instance.recognizer.patterns;
            GameManager.Instance.recognizer.patterns = new List<GesturePattern>(1) { _ftuePattern };
            GameManager.Instance.player.gestureSpellsController.highJumpCallback += ThirdStep;
        }

        private void ThirdStep()
        {
            GameManager.Instance.recognizer.patterns = _activePatterns;
            GameManager.Instance.player.gestureSpellsController.highJumpCallback -= ThirdStep;
            GameManager.Instance.level.EnableMovement();
            _ftueHand.SetActive(false);
        }
    }
}
