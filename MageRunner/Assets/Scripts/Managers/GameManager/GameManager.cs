﻿using System;
using System.Collections.Generic;
using GestureRecognizer;
using MageRunner.Dialogues;
using MageRunner.Gestures;
using MageRunner.Levels;
using MageRunner.Player;
using MageRunner.Scenes;
using UnityEngine;
using UnityEngine.UI;

namespace MageRunner.Managers.GameManager
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;
        
        public Camera mainCamera;
        public GesturesDifficultyData gesturesDifficultyData;
        public Text currencyText;
        
        [SerializeField] private GameObject _spellsDrawArea;
        [SerializeField] private Recognizer _recognizer;
        
        [NonSerialized] public PlayerController player;
        [NonSerialized] public LevelSceneController levelScene;
        [NonSerialized] public LevelController level;
        [NonSerialized] public DialogueController dialoguePlaying;

        private int _currency;
        private Vector3 _cameraInitialPosition;

        public List<Gesture> activeGestures { get; } = new List<Gesture>();

        private void Awake()
        {
            if (Instance == null)
                Instance = this;

            _cameraInitialPosition = mainCamera.transform.localPosition;
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

        public void ResetGestures() => _recognizer.patterns = _recognizer.patterns.GetRange(0, player.gestureSpellsController.basicSpellsDict.Count);

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
