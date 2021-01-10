using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelController : MonoBehaviour
{
    public float movingSpeed;
    public Transform movingObjects;
    public TimeFrame[] timeFrames;

    [NonSerialized] public bool isMoving;
    
    [SerializeField] private List<RepeatingSegment> _repeatingSegments;

    private int _currentTimeFrameIndex;
    private int _currency;
    private bool _looping;
    private float _repeatingStartX;
    private float _repeatingEndX;
    private List<List<GesturesHolder>> _timeFramesGesturesHolders = new List<List<GesturesHolder>>();
    private List<GesturesHolder> _gesturesHolders = new List<GesturesHolder>();
    private int _currentGesturesHolderIndex;
    private float _distanceBetweenPlayerX;

    public int currentTimeFrameIndex => _currentTimeFrameIndex;

    private void Awake() => GameManager.Instance.level = this;

    private void Start()
    {
        InitializeGesturesHolders();
        GameManager.Instance.player.transform.SetParent(movingObjects);
        Camera mainCamera = Camera.main;
        mainCamera.GetComponent<ScaleWidthCamera>().ScaleLevelCamera(transform);
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
        
        GesturesHolder currentGesturesHolder = _gesturesHolders[_currentGesturesHolderIndex];
        _distanceBetweenPlayerX = currentGesturesHolder.transform.position.x - GameManager.Instance.player.transform.position.x;
        if (_distanceBetweenPlayerX <= currentGesturesHolder.DistanceToSpawn)
        {
            _currentGesturesHolderIndex += 1;
            currentGesturesHolder.gameObject.SetActive(true);
            currentGesturesHolder.LoadGestures();
            // if (currentEnemy.enablesLevelLoop)
            //     GameManager.Instance.level.StartLooping();
        }
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
        _currentTimeFrameIndex += 1;
        timeFrames[currentTimeFrameIndex].movingGO.SetActive(true);
        timeFrames[currentTimeFrameIndex].staticGO.SetActive(true);
        ChangeGesturesHoldersList();
    }
    
    private void InitializeGesturesHolders()
    {
        foreach (TimeFrame timeframe in GameManager.Instance.level.timeFrames)
        {
            List<GesturesHolder> currentTimeFrameGesturesHolder = new List<GesturesHolder>();
            currentTimeFrameGesturesHolder.AddRange(timeframe.movingGO.GetComponentsInChildren<GesturesHolder>());
            currentTimeFrameGesturesHolder.AddRange(timeframe.staticGO.GetComponentsInChildren<GesturesHolder>());
            currentTimeFrameGesturesHolder = currentTimeFrameGesturesHolder.OrderBy(gesturesHolder => gesturesHolder.transform.position.x).ToList();
            
            foreach (GesturesHolder gesturesHolder in currentTimeFrameGesturesHolder)
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
