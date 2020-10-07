using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ChatBubbleController : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private TMP_Text _chatBubbleText;

    private bool _isTyping;
    private float _secondsToContinue;
    private readonly List<Tuple<string, float>> textsQueued = new List<Tuple<string, float>>();

    public void StartChat(string message, float secondsToContinue)
    {
        if (_isTyping)
            textsQueued.Add(Tuple.Create(message, secondsToContinue));
        else
        {
            _animator.SetBool("Enabled", true);
            _isTyping = true;
            _chatBubbleText.text = message;
            _secondsToContinue = secondsToContinue;
        }
    }
    
    private void NextChat()
    {
        if (textsQueued.Count == 0)
        {
            _animator.SetBool("Enabled", false);
            _chatBubbleText.gameObject.SetActive(false);
        }
        else
        {
            StartChat(textsQueued[0].Item1, textsQueued[0].Item2);
            textsQueued.RemoveAt(0);
        }
    }

    public void WaitForText() => StartCoroutine(WaitSecondsToContinue());
    
    private IEnumerator WaitSecondsToContinue()
    {
        yield return new WaitForSeconds(_secondsToContinue);
        _isTyping = false;
        
        NextChat();
    }
    
    // Method called in the chat bubble animations
    private void EnableText() => _chatBubbleText.gameObject.SetActive(true);
}
