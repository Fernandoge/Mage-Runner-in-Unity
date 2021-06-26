using System.Collections.Generic;
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

        public float distanceToSpawn => _distanceToSpawn;
        public float distanceBetweenPlayerX => _distanceBetweenPlayerX;
        public int activeGestures { get; set; }

        private void OnDestroy()
        {
            if (_enablesLevelLoop)
                GameManager.Instance.level.StopLooping();
        }

        private void Awake()
        {
            _currentHealthpoints = _healthpoints;
            LoadGestures();
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
            foreach (Gesture gesture in gestures.ToArray())
            {
                gestures.Remove(gesture);
                GesturePattern pattern = PickRandomGesture(gesture.difficulty);
                gesture.iconRenderer.sprite = pattern.icon;
                Gesture loadedGesture = new Gesture(gesture.spell, gesture.difficulty, gesture.iconRenderer, pattern, gameObject, false);
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

        public GesturePattern PickRandomGesture(EGestureDifficulty difficulty)
        {
            switch (difficulty)
            {
                case EGestureDifficulty.Easy:
                    GesturePattern[] easyGestures = GameManager.Instance.gesturesDifficultyData.easy;
                    return easyGestures[Random.Range(0, easyGestures.Length)];
                case EGestureDifficulty.Medium:
                    GesturePattern[] mediumGestures = GameManager.Instance.gesturesDifficultyData.medium;
                    return mediumGestures[Random.Range(0, mediumGestures.Length)];
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
