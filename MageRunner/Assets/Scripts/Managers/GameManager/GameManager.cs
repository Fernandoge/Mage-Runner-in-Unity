using System;
using System.Collections.Generic;
using GestureRecognizer;
using MageRunner.Dialogues;
using MageRunner.Gestures;
using MageRunner.Levels;
using MageRunner.Player;
using UnityEngine;
using UnityEngine.UI;

namespace MageRunner.Managers.GameManager
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;
        
        public GesturesDifficultyData gesturesDifficultyData;
        public Text currencyText;

        [SerializeField] private Recognizer _recognizer;
        
        [NonSerialized] public PlayerController player;
        [NonSerialized] public LevelController level;
        [NonSerialized] public DialogueController dialoguePlaying;

        private int _currency;

        public List<Gesture> activeGestures { get; } = new List<Gesture>();

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
        }

        public void AddGesture(Gesture gesture)
        {
            activeGestures.Add(gesture);
            _recognizer.patterns.Add(gesture.pattern);
        }

        public void RemoveGesture(Gesture gesture)
        {
            activeGestures.Remove(gesture);
            _recognizer.patterns.Remove(gesture.pattern);
        }
            
        public void UpdateCurrency(int value)
        {
            _currency += value;
            currencyText.text = _currency.ToString();
        }
        
    }
}
