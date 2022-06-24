using System.Collections.Generic;
using GestureRecognizer;
using MageRunner.Dialogues;
using MageRunner.Gestures;
using MageRunner.Managers.GameManager;
using UnityEngine;

namespace MageRunner.FTUE
{
    public class Ftue_6_FastFall : FtueSection
    {
        [SerializeField] private GameObject _ftueHand;
        [SerializeField] private AnimationClip fastFallGestureAnimation;
        [SerializeField] private GesturePattern _ftuePattern;

        private List<GesturePattern> _activePatterns;

        protected override void FirstStep()
        {
            ChatBubbleController chatBubble = GameManager.Instance.player.companionChatBubble;
            chatBubble.StartChatCoroutine(new Message("this seems a hard fall", 0f, 2f, changeLevelMovingState: true, onTextClosed: () => GameManager.Instance.level.EnableMovement()));
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
            
            UnlockBasicSpell(_ftuePattern);
            _activePatterns = GameManager.Instance.recognizer.patterns;
            GameManager.Instance.recognizer.patterns = new List<GesturePattern>(1) { _ftuePattern };
            
            Animator handAnimator = _ftueHand.GetComponent<Animator>();
            AnimatorOverrideController handAnimatorController = (AnimatorOverrideController) handAnimator.runtimeAnimatorController;
            handAnimatorController["Gestures FTUE"] = fastFallGestureAnimation;
            _ftueHand.SetActive(true);

            GameManager.Instance.player.gestureSpellsController.fastFallCallBack += ThirdStep;
        }

        private void ThirdStep()
        {
            GameManager.Instance.player.gestureSpellsController.fastFallCallBack -= ThirdStep;
            GameManager.Instance.recognizer.patterns = _activePatterns;
            _ftueHand.SetActive(false);
            Time.timeScale = 1;
        }
    }
}
