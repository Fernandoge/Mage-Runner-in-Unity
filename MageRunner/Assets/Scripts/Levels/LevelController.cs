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
        [NonSerialized] public CheckpointController currentCheckpoint;
        [NonSerialized] public List<GesturesHolderController> flyingEnemiesGesturesHolderController = new List<GesturesHolderController>();

        [SerializeField] private List<RepeatingSegment> _repeatingSegments;

        private int _currency;
        private bool _looping;
        private float _repeatingStartX;
        private float _repeatingEndX;
        private readonly List<List<GesturesHolderController>> _timeFramesGesturesHolders = new List<List<GesturesHolderController>>();
        private float _distanceBetweenPlayerX;

        public List<GesturesHolderController> gesturesHolders { get; private set; } = new List<GesturesHolderController>();
        public int currentTimeFrameIndex { get; private set; }
        public  int currentGesturesHolderIndex { get; private set; }

        private void Awake() => GameManager.Instance.level = this;

        private void Start()
        {
            InitializeGesturesHolders();
            GameManager.Instance.player.transform.SetParent(movingObjects);
            Camera mainCamera = GameManager.Instance.mainCamera;
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
            if (currentGesturesHolderIndex == gesturesHolders.Count)
                return;
        
            ActivateNextGesturesHolder();
        }

        private void ActivateNextGesturesHolder()
        {
            GesturesHolderController currentGesturesHolder = gesturesHolders[currentGesturesHolderIndex];
            _distanceBetweenPlayerX = currentGesturesHolder.transform.position.x - GameManager.Instance.player.transform.position.x;
            
            if (_distanceBetweenPlayerX > currentGesturesHolder.distanceToSpawn) 
                return;

            if (currentGesturesHolder.activateGesturesManually == false)
                currentGesturesHolder.ActivateGestures();

            currentGesturesHolder.gameObject.SetActive(true);
            currentGesturesHolderIndex += 1;
            
            // if (currentEnemy.enablesLevelLoop)
            //     GameManager.Instance.level.StartLooping();
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
            if (currentCheckpoint == null)
            {
                Scene currentScene = SceneManager.GetActiveScene(); 
                SceneManager.LoadScene(currentScene.name);
            }
            else
            {
                currentCheckpoint.SpawnPlayerInCheckpoint();
                currentGesturesHolderIndex = 0;
            }
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
                List<GesturesHolderController> currentTimeFrameGesturesHolders = new List<GesturesHolderController>();
                currentTimeFrameGesturesHolders.AddRange(timeframe.movingGO.GetComponentsInChildren<GesturesHolderController>());
                currentTimeFrameGesturesHolders.AddRange(timeframe.staticGO.GetComponentsInChildren<GesturesHolderController>());
                currentTimeFrameGesturesHolders = currentTimeFrameGesturesHolders.OrderBy(gesturesHolder => gesturesHolder.transform.position.x).ToList();

                foreach (GesturesHolderController gesturesHolder in currentTimeFrameGesturesHolders)
                {
                    gesturesHolder.gameObject.SetActive(false);
                    foreach (Gesture gesture in gesturesHolder.gestures)
                        gesture.iconRenderer.gameObject.SetActive(false);
                }
                
                _timeFramesGesturesHolders.Add(currentTimeFrameGesturesHolders);
            }
            gesturesHolders = _timeFramesGesturesHolders[0];
        }
    
        private void ChangeGesturesHoldersList()
        {
            gesturesHolders = _timeFramesGesturesHolders[GameManager.Instance.level.currentTimeFrameIndex];
            currentGesturesHolderIndex = 0;
        }
    }
}
