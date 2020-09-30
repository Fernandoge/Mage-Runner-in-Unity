using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CompanionController : MonoBehaviour
{
    [SerializeField] private GameObject _chatBubble;
    [SerializeField] private TMP_Text _chatBubbleText;

    private bool _isChatting;
    private List<Tuple<string, float>> textsQueued = new List<Tuple<string, float>>();
    
    public void StartChat(string message, float duration)
    {
        StartCoroutine(Chat(message, duration));
    }

    private IEnumerator Chat(string message, float duration)
    {
        if (_isChatting) 
            textsQueued.Add(Tuple.Create(message, duration));
        else
        {
            if (_chatBubble.activeSelf == false)
                _chatBubble.SetActive(true);
            
            _isChatting = true;
            _chatBubbleText.text = message;
            
            yield return new WaitForSeconds(duration);
            
            _chatBubble.SetActive(false);
            _isChatting = false;
            NextChat();
        }

    }

    private void NextChat()
    {
        if (textsQueued.Count == 0)
            return;
            
        StartCoroutine(Chat(textsQueued[0].Item1, textsQueued[0].Item2));
        textsQueued.RemoveAt(0);
    }
}
