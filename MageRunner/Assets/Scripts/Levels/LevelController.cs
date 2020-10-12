using System;
using System.Collections.Generic;
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
    [SerializeField] private Text _currencyText;

    private int _currentTimeFrameIndex;
    private int _currency;
    private bool _looping;
    private float _repeatingStartX;
    private float _repeatingEndX;

    public int currentTimeFrameIndex => _currentTimeFrameIndex;
    
    private void Start()
    {
        GameManager.Instance.level = this;
        EnemiesManager.Instance.InitializeEnemies();
        Camera.main.GetComponent<ScaleWidthCamera>().ScaleLevelCamera(transform);
    } 
    
    private void Update()
    {
        //TODO: Optimize the game objects positions if they are high values to avoid floating-point precision limitations
        
        if (GameManager.Instance.level.isMoving)
            movingObjects.Translate(Vector2.right * movingSpeed * Time.deltaTime);
        
        if (_looping && movingObjects.transform.localPosition.x >= _repeatingEndX)
        {
            float distanceReturned = _repeatingEndX - _repeatingStartX;
            transform.position = new Vector2(transform.position.x + distanceReturned, transform.position.y);
            movingObjects.transform.localPosition = new Vector2(_repeatingStartX, movingObjects.transform.localPosition.y);
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

    public void UpdateCurrency(int value)
    {
        _currency += value;
        _currencyText.text = _currency.ToString();
    }

    public void ChangeTimeFrame()
    {
        timeFrames[currentTimeFrameIndex].movingGO.SetActive(false);
        timeFrames[currentTimeFrameIndex].staticGO.SetActive(false);
        GameManager.Instance.level.movingObjects.position = Vector3.zero;
        _currentTimeFrameIndex += 1;
        timeFrames[currentTimeFrameIndex].movingGO.SetActive(true);
        timeFrames[currentTimeFrameIndex].staticGO.SetActive(true);
        EnemiesManager.Instance.ChangeEnemiesList();
    }
}
