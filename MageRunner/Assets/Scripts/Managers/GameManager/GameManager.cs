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
        
        public Camera mainCamera;
        public PlayerController player;
        public LevelController level;
        public GesturesDifficultyData gesturesDifficultyData;
        public Recognizer recognizer;
        public Text currencyText;
        
        [SerializeField] private GameObject _spellsDrawArea;
        
        [NonSerialized] public DialogueController dialoguePlaying;
        [NonSerialized] public int basicSpellsUnlocked;

        private int _currency;
        private Vector3 _cameraInitialPosition;

        public List<Gesture> gameActiveGestures { get; } = new List<Gesture>();
        public List<PlayerAttackSpell> gameActivePlayerSpells = new List<PlayerAttackSpell>();

        private void Awake()
        {
            if (Instance == null)
                Instance = this;

            _cameraInitialPosition = mainCamera.transform.localPosition;
        }

        public void AddGesture(Gesture gesture)
        {
            gameActiveGestures.Add(gesture);
            recognizer.patterns.Add(gesture.pattern);
        }

        public void RemoveGesture(Gesture gesture)
        {
            gameActiveGestures.Remove(gesture);
            recognizer.patterns.Remove(gesture.pattern);
        }

        public void ResetGestures() => recognizer.patterns = recognizer.patterns.GetRange(0, basicSpellsUnlocked);

        public void ResetCameraPosition() => mainCamera.transform.localPosition = _cameraInitialPosition;
        
        public void UpdateCurrency(int value)
        {
            _currency += value;
            currencyText.text = _currency.ToString();
        }

        public void ToggleCinematicMode(bool state)
        {
            player.jumpButton.gameObject.SetActive(!state);
            player.healthController.gameObject.SetActive(!state);
            player.manaController.gameObject.SetActive(!state);
            _spellsDrawArea.gameObject.SetActive(!state);
        }
    }
}
