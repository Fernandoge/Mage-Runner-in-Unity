using System;
using System.Collections;
using System.Collections.Generic;
using Febucci.UI;
using MageRunner.Managers.GameManager;
using MageRunner.Player;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace MageRunner.Dialogues
{
    public class ChatBubbleController : MonoBehaviour
    {
        public TMP_Text chatBubbleText;
    
        [SerializeField] private Animator _animator;
        [SerializeField] private TextAnimatorPlayer _animatorPlayer;

        private bool _isTyping;
        private float _secondsToContinue;
        private bool _changeLevelMovingState;
        private bool _waitForAction;
        private bool _levelMovingStateChangedInThisChat;
        private bool _isFullyClosed = true;
        private readonly List<Message> messagesQueued = new List<Message>();
        private UnityAction _onTextShowedListener;
        private Action _onTextClosed;

        public void StartChatCoroutine(Message message) => StartCoroutine(Chat(message));

        private IEnumerator Chat(Message message)
        {
            if (_isTyping)
                messagesQueued.Add(message);
            else
            {
                _isTyping = true;
                yield return new WaitForSeconds(message.secondsToStart);
                
                // If chat bubble is fully closed let the bubble animation enable it,
                // otherwise since the chat bubble may still be open, enable it by code
                if (_isFullyClosed == false)
                    EnableText();
                
                _animator.SetBool("Enabled", true);
                _isFullyClosed = false;
                _levelMovingStateChangedInThisChat = false;  
                
                chatBubbleText.text = message.text;
                _secondsToContinue = message.secondsToContinue;
                _changeLevelMovingState = message.changeLevelMovingState;
                _waitForAction = message.waitForAction;
                _onTextClosed = message.onTextClosed;
                if (message.onTextShowed != null)
                {
                    _animatorPlayer.onTextShowed.AddListener(message.onTextShowed.Invoke);
                    _onTextShowedListener = message.onTextShowed.Invoke;
                }
                GameManager.Instance.player.idleCastEnabled = message.enablePlayerIdleCast;

                if (_changeLevelMovingState && GameManager.Instance.level.isMoving)
                {
                    GameManager.Instance.level.DisableMovement();
                    _levelMovingStateChangedInThisChat = true;
                }
            }
        }

        private void NextChat()
        {
            if (_onTextShowedListener != null)
                _animatorPlayer.onTextShowed.RemoveListener(_onTextShowedListener);
            
            if (messagesQueued.Count == 0)
            {
                _animator.SetBool("Enabled", false);
                chatBubbleText.gameObject.SetActive(false);
                if (GameManager.Instance.dialoguePlaying != null)
                    GameManager.Instance.dialoguePlaying.NextSection();
            }
            else
            {
                StartChatCoroutine(messagesQueued[0]);
                messagesQueued.RemoveAt(0);
            }
        }

        public void StartCloseCurrentChatCoroutine() => StartCoroutine(CloseCurrentChat());
    
        private IEnumerator CloseCurrentChat()
        {
            yield return new WaitForSeconds(_secondsToContinue);
            _isTyping = false;
            
            if (_changeLevelMovingState && GameManager.Instance.level.isMoving == false && _levelMovingStateChangedInThisChat == false)
                GameManager.Instance.level.EnableMovement();

            if (_waitForAction == false)
                NextChat();

            _onTextClosed?.Invoke();
        }

        public void ForceClose()
        {
            StopAllCoroutines();
            _isTyping = false;
            messagesQueued.Clear();
            NextChat();
        }
    
        // Methods called in the chat bubble animation too
        public void EnableText() => chatBubbleText.gameObject.SetActive(true);

        public void ChatBubbleFullyClosed() => _isFullyClosed = true;

    }
}
