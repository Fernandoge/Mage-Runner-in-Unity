using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChatBubbleController : MonoBehaviour
{
    [SerializeField] private Image _chatBubbleImage;
    [SerializeField] private TMP_Text _chatBubbleText;

    private bool _isTyping;
    private readonly List<Tuple<string, float>> textsQueued = new List<Tuple<string, float>>();
    
    public void StartChat(string message, float duration)
    {
        StartCoroutine(Chat(message, duration));
    }

    private IEnumerator Chat(string message, float duration)
    {
        if (_isTyping) 
            textsQueued.Add(Tuple.Create(message, duration));
        else
        {
            if (_chatBubbleImage.enabled == false)
                ActivateChatBubble(true);
                
            _chatBubbleText.text = message;
            
            _isTyping = true;
            yield return new WaitForSeconds(duration);
            _isTyping = false;
            
            NextChat();
        }
    }

    private void NextChat()
    {
        if (textsQueued.Count == 0)
            ActivateChatBubble(false);
        else
        {
            StartCoroutine(Chat(textsQueued[0].Item1, textsQueued[0].Item2));
            textsQueued.RemoveAt(0);
        }
    }

    private void ActivateChatBubble(bool state)
    {
        _chatBubbleImage.enabled = state;
        _chatBubbleText.gameObject.SetActive(state);
    }
    
}
