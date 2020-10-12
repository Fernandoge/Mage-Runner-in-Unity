using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ChatBubbleController : MonoBehaviour
{
    public TMP_Text chatBubbleText;

    [SerializeField] private Animator _animator;

    private bool _isTyping;
    private float _secondsToContinue;
    private bool _changeLevelMovingState;
    private bool _levelMovingStateChangedInThisChat;
    private readonly List<Message> messagesQueued = new List<Message>();

    public void StartChat(Message message)
    {
        if (_isTyping)
            messagesQueued.Add(message);
        else
        {
            _animator.SetBool("Enabled", true);
            _isTyping = true;
            _levelMovingStateChangedInThisChat = false;  
            chatBubbleText.text = message.text;
            _secondsToContinue = message.secondsToContinue;
            _changeLevelMovingState = message.changeLevelMovingState;
            if (_changeLevelMovingState && GameManager.Instance.level.isMoving)
            {
                GameManager.Instance.level.isMoving = false;
                _levelMovingStateChangedInThisChat = true;
            }
        }
    }
    
    private void NextChat()
    {
        if (messagesQueued.Count == 0)
        {
            _animator.SetBool("Enabled", false);
            chatBubbleText.gameObject.SetActive(false);
            if (GameManager.Instance.dialoguePlaying != null)
                GameManager.Instance.dialoguePlaying.NextSection();
        }
        else
        {
            StartChat(messagesQueued[0]);
            messagesQueued.RemoveAt(0);
        }
    }

    public void WaitForText() => StartCoroutine(WaitSecondsToContinue());
    
    private IEnumerator WaitSecondsToContinue()
    {
        yield return new WaitForSeconds(_secondsToContinue);
        _isTyping = false;
        if (_changeLevelMovingState && GameManager.Instance.level.isMoving == false && _levelMovingStateChangedInThisChat == false)
            GameManager.Instance.level.isMoving = true;
        
        NextChat();
    }

    public void ForceClose()
    {
        StopAllCoroutines();
        _isTyping = false;
        messagesQueued.Clear();
    }
    
    
    // Method called in the chat bubble animations
    private void EnableText() => chatBubbleText.gameObject.SetActive(true);
    
}
