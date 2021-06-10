using System;
using System.Collections.Generic;
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

        public List<Gesture> activeGestures = new List<Gesture>();
        public GesturesDifficultyData gesturesDifficultyData;
        public Text currencyText;

        [NonSerialized] public PlayerController player;
        [NonSerialized] public LevelController level;
        [NonSerialized] public DialogueController dialoguePlaying;

        private int _currency;

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
        }

        public void UpdateCurrency(int value)
        {
            _currency += value;
            currencyText.text = _currency.ToString();
        }
    }
}
