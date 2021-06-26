using System;
using System.Collections.Generic;
using System.Linq;
using MageRunner.Cameras;
using MageRunner.Gestures;
using MageRunner.Managers.GameManager;
using MageRunner.Player;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MageRunner.Levels
{
    public class LevelController : MonoBehaviour
    {
        public float movingSpeed;
        public Transform movingObjects;
        public TimeFrame[] timeFrames;

        [NonSerialized] public List<MovingBG> movingBgs = new List<MovingBG>();
        [NonSerialized] public List<MovingParticles> movingParticles = new List<MovingParticles>();
        [NonSerialized] public bool isMoving;
    
        [SerializeField] private List<RepeatingSegment> _repeatingSegments;

        private int _currency;
        private bool _looping;
        private float _repeatingStartX;
        private float _repeatingEndX;
        private List<List<GesturesHolderController>> _timeFramesGesturesHolders = new List<List<GesturesHolderController>>();
        private List<GesturesHolderController> _gesturesHolders = new List<GesturesHolderController>();
        private int _currentGesturesHolderIndex;
        private float _distanceBetweenPlayerX;

        public int currentTimeFrameIndex { get; private set; }

        private void Awake() => GameManager.Instance.level = this;

        private void Start()
        {
            InitializeGesturesHolders();
            GameManager.Instance.player.transform.SetParent(movingObjects);
            Camera mainCamera = Camera.main;
            mainCamera.GetComponent<CameraController>().ScaleLevelCamera(transform);
            mainCamera.transform.SetParent(movingObjects);
        } 
    
        private void Update()
        {
            //TODO: Optimize the game objects positions if they are high values to avoid floating-point precision limitations
        
            if (GameManager.Instance.level.isMoving)
                movingObjects.Translate(Vector2.right * movingSpeed * Time.deltaTime);
        
            // Level looping logic
        
            if (_looping && movingObjects.transform.localPosition.x >= _repeatingEndX)
            {
                float distanceReturned = _repeatingEndX - _repeatingStartX;
                transform.position = new Vector2(transform.position.x + distanceReturned, transform.position.y);
                movingObjects.transform.localPosition = new Vector2(_repeatingStartX, movingObjects.transform.localPosition.y);
            }
        
            // Gestures holders activation
            if (_currentGesturesHolderIndex == _gesturesHolders.Count)
                return;
        
            GesturesHolderController currentGesturesHolder = _gesturesHolders[_currentGesturesHolderIndex];
            _distanceBetweenPlayerX = currentGesturesHolder.transform.position.x - GameManager.Instance.player.transform.position.x;
            if (_distanceBetweenPlayerX <= currentGesturesHolder.distanceToSpawn)
            {
                currentGesturesHolder.gameObject.SetActive(true);
                currentGesturesHolder.ActivateGestures();
                _currentGesturesHolderIndex += 1;
                // if (currentEnemy.enablesLevelLoop)
                //     GameManager.Instance.level.StartLooping();
            }
        }

        public void EnableMovement()
        {
            isMoving = true;
            GameManager.Instance.player.stateHandler.DisableState(EPlayerState.Idle);
            GameManager.Instance.player.jumpButton.interactable = true;
            foreach (MovingParticles particle in movingParticles)
                particle.EnableVelocityOverLifetime();
        }

        public void DisableMovement()
        {
            isMoving = false;
            GameManager.Instance.player.stateHandler.EnableState(EPlayerState.Idle);
            GameManager.Instance.player.jumpButton.interactable = false;
            foreach (MovingParticles particle in movingParticles)
                particle.DisableVelocityOverLifetime();
        }

        public void StartLooping()
        {
            _repeatingStartX = _repeatingSegments[0].StartX;
            _repeatingEndX = _repeatingSegments[0].EndX;
            _looping = true;
        }

        public void StopLooping()
        {
            _looping = false;
            _repeatingSegments.RemoveAt(0);
        }
    
        public void ResetLevel()
        {
            Scene currentScene = SceneManager.GetActiveScene(); 
            SceneManager.LoadScene(currentScene.name);
        }

        public void ChangeTimeFrame()
        {
            timeFrames[currentTimeFrameIndex].movingGO.SetActive(false);
            timeFrames[currentTimeFrameIndex].staticGO.SetActive(false);
            GameManager.Instance.level.movingObjects.position = Vector3.zero;
            GameManager.Instance.level.movingBgs.Clear();
            GameManager.Instance.level.movingParticles.Clear();
            currentTimeFrameIndex += 1;
            timeFrames[currentTimeFrameIndex].movingGO.SetActive(true);
            timeFrames[currentTimeFrameIndex].staticGO.SetActive(true);
            ChangeGesturesHoldersList();
        }
    
        private void InitializeGesturesHolders()
        {
            foreach (TimeFrame timeframe in GameManager.Instance.level.timeFrames)
            {
                List<GesturesHolderController> currentTimeFrameGesturesHolder = new List<GesturesHolderController>();
                currentTimeFrameGesturesHolder.AddRange(timeframe.movingGO.GetComponentsInChildren<GesturesHolderController>());
                currentTimeFrameGesturesHolder.AddRange(timeframe.staticGO.GetComponentsInChildren<GesturesHolderController>());
                currentTimeFrameGesturesHolder = currentTimeFrameGesturesHolder.OrderBy(gesturesHolder => gesturesHolder.transform.position.x).ToList();
            
                foreach (GesturesHolderController gesturesHolder in currentTimeFrameGesturesHolder)
                    gesturesHolder.gameObject.SetActive(false);
            
                _timeFramesGesturesHolders.Add(currentTimeFrameGesturesHolder);
            }

            _gesturesHolders = _timeFramesGesturesHolders[0];
        }
    
        private void ChangeGesturesHoldersList()
        {
            _gesturesHolders = _timeFramesGesturesHolders[GameManager.Instance.level.currentTimeFrameIndex];
            _currentGesturesHolderIndex = 0;
        }
    }
}
