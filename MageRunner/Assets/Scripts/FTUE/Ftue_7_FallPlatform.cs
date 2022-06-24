using System.Collections.Generic;
using GestureRecognizer;
using MageRunner.Dialogues;
using MageRunner.Managers.GameManager;
using UnityEngine;
using UnityEngine.Events;

namespace MageRunner.FTUE
{
    public class Ftue_7_FallPlatform : FtueSection
    {
        [SerializeField] private GameObject _ftueHand;
        [SerializeField] private AnimationClip _platformFallGestureAnimation;
        [SerializeField] private GesturePattern _ftuePattern;
        [SerializeField] private UnityEvent _textShownEvent;
        
        private ChatBubbleController chatBubble;
        private List<GesturePattern> _activePatterns;

        protected override void FirstStep()
        {
            GameManager.Instance.ToggleCinematicMode(true);
            chatBubble = GameManager.Instance.player.companionChatBubble;
            chatBubble.StartChatCoroutine(new Message("you can use fast fall to get down platforms", 0f, 2f));
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
            GameManager.Instance.ToggleCinematicMode(false);
            chatBubble.StartChatCoroutine(new Message("cast fast fall twice to fall down the platform", 0f, 1f, changeLevelMovingState: true, onTextShowed: _textShownEvent, enablePlayerIdleCast: true, waitForAction: true));
        }

        // UnityEvent textShownEvent
        public void ThirdStep()
        {
            _activePatterns = GameManager.Instance.recognizer.patterns;
            GameManager.Instance.recognizer.patterns = new List<GesturePattern>(1) { _ftuePattern };
            
            Animator handAnimator = _ftueHand.GetComponent<Animator>();
            AnimatorOverrideController handAnimatorController = (AnimatorOverrideController) handAnimator.runtimeAnimatorController;
            handAnimatorController["Gestures FTUE"] = _platformFallGestureAnimation;
            _ftueHand.SetActive(true);

            GameManager.Instance.player.gestureSpellsController.fastFallCallBack += FourthStep;
        }

        private void FourthStep()
        {
            if (!GameManager.Instance.player.readyToPlatformFall) 
                return;
            
            GameManager.Instance.player.gestureSpellsController.fastFallCallBack -= FourthStep;
            GameManager.Instance.recognizer.patterns = _activePatterns;
            GameManager.Instance.level.EnableMovement();
            _ftueHand.SetActive(false);
            chatBubble.ForceClose();
        }
    }
}
