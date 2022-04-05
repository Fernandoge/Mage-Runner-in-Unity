using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GestureRecognizer;
using MageRunner.FTUE;
using MageRunner.Managers.GameManager;
using MageRunner.Player;
using UnityEngine;
using Random = UnityEngine.Random;

namespace MageRunner.Gestures
{
    public class GesturesHolderController : MonoBehaviour
    {
        public List<Gesture> gestures = new List<Gesture>();
        
        [Header("")]
        [SerializeField] private float _distanceToSpawn;
        // [SerializeField] private Transform _healthpointsBarHolder;
        
        private float _distanceBetweenPlayerX;
        private float _timeInvisible;
        private Vector3 _originalPosition;
        private Transform _originalParent;
        private bool _gesturesDeactivated;
        private bool _isInvisible;

        [NonSerialized] public Gesture previousGesture;

        public bool loadGesturesManually;
        public bool infiniteGestures;
        [SerializeField] private int _healthpoints;
        
        public float distanceToSpawn => _distanceToSpawn;
        public float distanceBetweenPlayerX => _distanceBetweenPlayerX;
        public int currentHealthpoints { get; set; }
        public bool gesturesLoaded { get; set; }
        public int activeGestures { get; set; }
        public bool isMoving { get; set; }
        
        public event Action deactivate;
        public event Action zeroHpCallback;

        private void OnDestroy()
        {
            if (GameManager.Instance.level.isLooping)
                GameManager.Instance.level.isLooping = false;
        }
        
        private void OnBecameInvisible()
        {
            if (_distanceBetweenPlayerX > 0)
                return;
            
            _isInvisible = true;
            _timeInvisible = 2f;
        }

        protected void Awake()
        {
            currentHealthpoints = _healthpoints;
            _originalPosition = transform.position;
            _originalParent = transform.parent;
        }

        private void Update()
        {
            _distanceBetweenPlayerX = transform.position.x - GameManager.Instance.player.transform.position.x;

            if (_distanceBetweenPlayerX < -0.5 && _gesturesDeactivated == false)
            {
                _gesturesDeactivated = true;
                DeactivateGestures();
            }
            
            if (_isInvisible)
            {
                _timeInvisible -= Time.deltaTime;
                if (_timeInvisible < 0f)
                    gameObject.SetActive(false);
            }
            
#if UNITY_EDITOR
            DebugDistanceOnClick();
#endif
        }

        public void ResetOriginalPosition()
        {
            transform.position = _originalPosition;
            transform.SetParent(_originalParent);
        }
        
        public void ZeroHpCallback() => zeroHpCallback?.Invoke();

        public void ResetHP() => currentHealthpoints = _healthpoints;
        
        public void Deactivate()
        {
            deactivate?.Invoke();
            gameObject.SetActive(false);
        }

        private void LoadFtueGesture(ForceFtueGesture forceFtueGesture)
        {
            gestures.Remove(forceFtueGesture.gesture);
            Gesture loadedFtueGesture = new Gesture(forceFtueGesture.gesture.spell, forceFtueGesture.gesture.difficulty, forceFtueGesture.gesture.iconRenderer, forceFtueGesture.gesturePattern, this, false);
            gestures.Add(loadedFtueGesture);
            activeGestures += 1;
            GameManager.Instance.AddGesture(loadedFtueGesture);
            loadedFtueGesture.iconRenderer.gameObject.SetActive(true);
        }

        public void LoadGestures()
        {
            if (gesturesLoaded)
                return;

            _isInvisible = false; 
            
            ForceFtueGesture forceFtueGesture = GetComponent<ForceFtueGesture>();
            if (forceFtueGesture != null)
                LoadFtueGesture(forceFtueGesture);
            else
            {
                activeGestures = 0;
                
                GesturesDifficultyData gesturesData = Instantiate(GameManager.Instance.gesturesDifficultyData);
                foreach (Gesture gesture in GameManager.Instance.gameActiveGestures)
                    RemoveGestureFromData(gesture, gesturesData);
                
                if (infiniteGestures)
                    RemoveGestureFromData(previousGesture, gesturesData);
                
                foreach (Gesture gesture in gestures.ToArray())
                {
                    gestures.Remove(gesture);
                    GesturePattern pattern = PickRandomGesture(gesture.difficulty, gesturesData);
                    gesture.iconRenderer.sprite = pattern.icon;
                    Gesture loadedGesture = new Gesture(gesture.spell, gesture.difficulty, gesture.iconRenderer, pattern, this, false);
                    RemoveGestureFromData(loadedGesture, gesturesData);
                    gestures.Add(loadedGesture);
                
                    activeGestures += 1;
                    GameManager.Instance.AddGesture(loadedGesture);
                    gesture.iconRenderer.gameObject.SetActive(true);
                }
            }
            gesturesLoaded = true;
        }

        private void DeactivateGestures()
        {
            foreach (Gesture gesture in gestures)
            {
                gesture.iconRenderer.gameObject.SetActive(false);
                GameManager.Instance.RemoveGesture(gesture);
            }
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
                    return RandomGesture(modifiedGestureDifficultyData.easy, GameManager.Instance.gesturesDifficultyData.easy);
                case EGestureDifficulty.Medium:
                    return RandomGesture(modifiedGestureDifficultyData.medium, GameManager.Instance.gesturesDifficultyData.medium);
                default:
                    return ScriptableObject.CreateInstance<GesturePattern>();
            }
        }

        private GesturePattern RandomGesture(List<GesturePattern> gesturePatterns, List<GesturePattern> originalGesturePatterns)
        {
            if (gesturePatterns.Count > 0)
                return gesturePatterns[Random.Range(0, gesturePatterns.Count)];
            
            print("Too many gestures on screen, repeating one");
            return originalGesturePatterns[Random.Range(0, originalGesturePatterns.Count)];
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
