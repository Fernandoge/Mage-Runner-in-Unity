﻿using System;
using System.Collections.Generic;
using System.Linq;
using GestureRecognizer;
using MageRunner.Managers.GameManager;
using UnityEngine;
using Random = UnityEngine.Random;

namespace MageRunner.Gestures
{
    public class GesturesHolderController : MonoBehaviour
    {
        public List<Gesture> gestures = new List<Gesture>();
    
        [Header("")]
        [SerializeField] private float _distanceToSpawn;
        [SerializeField] private int _healthpoints;
        [SerializeField] bool _enablesLevelLoop;
        [SerializeField] private Transform _healthpointsBarHolder;

        private float _currentHealthpoints;
        private float _distanceBetweenPlayerX;
        private List<GesturesHolderController> _gesturesHoldersNear = new List<GesturesHolderController>();

        public GesturesDifficultyData gesturesDifficultyDataModified;

        public float distanceToSpawn => _distanceToSpawn;
        public float distanceBetweenPlayerX => _distanceBetweenPlayerX;
        public int activeGestures { get; set; }

        private void OnDestroy()
        {
            if (_enablesLevelLoop)
                GameManager.Instance.level.StopLooping();
        }

        protected virtual void Awake()
        {
            _currentHealthpoints = _healthpoints;
            
        }

        private void Update()
        {
            _distanceBetweenPlayerX = transform.position.x - GameManager.Instance.player.transform.position.x;

            if (_distanceBetweenPlayerX < -0.5)
                DeactivateGestures();
        
#if UNITY_EDITOR
            DebugDistanceOnClick();
#endif
        }

        public void LoadGestures()
        {
            gesturesDifficultyDataModified = RemoveNearGesturesFromData();

            foreach (Gesture gesture in gestures.ToArray())
            {
                gestures.Remove(gesture);
                GesturePattern pattern = PickRandomGesture(gesture.difficulty, gesturesDifficultyDataModified);
                gesture.iconRenderer.sprite = pattern.icon;
                Gesture loadedGesture = new Gesture(gesture.spell, gesture.difficulty, gesture.iconRenderer, pattern, gameObject, false);

                RemoveGestureFromData(loadedGesture, gesturesDifficultyDataModified);
                gestures.Add(loadedGesture);
            }
        }

        public void ActivateGestures()
        {
            foreach (Gesture gesture in gestures)
            {
                activeGestures += 1;
                GameManager.Instance.activeGestures.Add(gesture);
            }
        }

        private void DeactivateGestures()
        {
            foreach (Gesture gesture in gestures)
            {
                GameManager.Instance.activeGestures.Remove(gesture);
                gesture.iconRenderer.gameObject.SetActive(false);
            }
        }

        public void FindGesturesHoldersNear(List<GesturesHolderController> timeFrameGesturesHolders)
        {
            int gesturesHolderIndex = timeFrameGesturesHolders.IndexOf(this);
            
            // Get the ones in the left
            for (int i = gesturesHolderIndex - 1; i >= 0; i--)
            {
                if (transform.position.x - timeFrameGesturesHolders[i].transform.position.x < 40)
                    _gesturesHoldersNear.Add(timeFrameGesturesHolders[i]);
                else
                    break;
            }
            
            // Get the ones in the right
            for (int i = gesturesHolderIndex + 1; i < timeFrameGesturesHolders.Count; i++)
            {
                if (transform.position.x - timeFrameGesturesHolders[i].transform.position.x > -40)
                    _gesturesHoldersNear.Add(timeFrameGesturesHolders[i]);
                else
                    break;
            }
        }

        private List<Gesture> GetActiveGesturesNear()
        {
            List<Gesture> activeGesturesNear = new List<Gesture>();
            foreach (GesturesHolderController gesturesHolder in _gesturesHoldersNear)
            {
                foreach (var gesture in gesturesHolder.gestures.Where(
                    gesture => gesture.pattern != null && activeGesturesNear.Contains(gesture) == false))
                {
                    activeGesturesNear.Add(gesture);
                }
            }

            return activeGesturesNear;
        }
        
        private GesturesDifficultyData RemoveNearGesturesFromData()
        {
            List<Gesture> activeGesturesNear = GetActiveGesturesNear();
            GesturesDifficultyData gesturesDataModified = Instantiate(GameManager.Instance.gesturesDifficultyData);

            foreach (Gesture gesture in activeGesturesNear)
                RemoveGestureFromData(gesture, gesturesDataModified);
            
            return gesturesDataModified;
        }

        private void RemoveGestureFromData(Gesture gesture, GesturesDifficultyData gesturesDataModified)
        {
            switch (gesture.difficulty)
            {
                case EGestureDifficulty.Easy:
                    gesturesDataModified.easy.Remove(gesture.pattern); return;
                case EGestureDifficulty.Medium:
                    gesturesDataModified.medium.Remove(gesture.pattern); return;
            }
        }

        private GesturePattern PickRandomGesture(EGestureDifficulty difficulty, GesturesDifficultyData modifiedGestureDifficultyData)
        {
            switch (difficulty)
            {
                case EGestureDifficulty.Easy:
                    List<GesturePattern> easyGestures = modifiedGestureDifficultyData.easy;
                    return easyGestures[Random.Range(0, easyGestures.Count)];
                case EGestureDifficulty.Medium:
                    List<GesturePattern> mediumGestures = modifiedGestureDifficultyData.medium;
                    return mediumGestures[Random.Range(0, mediumGestures.Count)];
                default:
                    return ScriptableObject.CreateInstance<GesturePattern>();
            }
        }
    
        private void DebugDistanceOnClick()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity);
                if (hit.collider?.transform == transform)
                {
                    print(_distanceBetweenPlayerX);
                }
            }
        }
    }
}
