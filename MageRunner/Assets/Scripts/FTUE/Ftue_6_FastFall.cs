using System;
using MageRunner.Dialogues;
using MageRunner.Managers.GameManager;
using UnityEngine;

namespace MageRunner.FTUE
{
    public class Ftue_6_FastFall : FtueSection
    {
        [SerializeField] private GameObject _ftueHand;
        [SerializeField] private AnimationClip fastFallGestureAnimation;

        public override void FirstStep()
        {
            ChatBubbleController chatBubble = GameManager.Instance.player.companionChatBubble;
            chatBubble.StartChatCoroutine(new Message("this seems a hard fall", 0f, 2f, changeLevelMovingState: true, onTextClosed: () => GameManager.Instance.level.EnableMovement()));
        }
        
        private void OnTriggerEnter2D(Collider2D other) => SecondStep();
        
        private void SecondStep()
        {
            Time.timeScale = 0;
            
            Animator handAnimator = _ftueHand.GetComponent<Animator>();
            AnimatorOverrideController handAnimatorController = (AnimatorOverrideController) handAnimator.runtimeAnimatorController;
            handAnimatorController["Gestures FTUE"] = fastFallGestureAnimation;
            _ftueHand.SetActive(true);

            GameManager.Instance.player.gestureSpellsController.fastFallCallBack += ThirdStep;
        }

        private void ThirdStep()
        {
            GameManager.Instance.player.gestureSpellsController.fastFallCallBack -= ThirdStep;
            _ftueHand.SetActive(false);
            Time.timeScale = 1;
        }
    }
}
