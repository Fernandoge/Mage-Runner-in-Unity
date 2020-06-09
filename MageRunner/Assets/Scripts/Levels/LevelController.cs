using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public struct RepeatingSegment
{
    public GameObject segment;
    public float StartX;
    public float EndX;
}

public class LevelController : MonoBehaviour
{
    public float movingSpeed;
    public Transform movingObjects;
    public List<RepeatingSegment> repeatingSegments;

    private bool _looping;
    private float _repeatingStartX;
    private float _repeatingEndX;

    private void Start() => GameManager.Instance.level = this;

    private void Update()
    {
        //TODO: Optimize the game objects positions if they are high values to avoid floating-point precision limitations
        
        if (GameManager.Instance.player.isMoving)
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
        _repeatingStartX = repeatingSegments[0].StartX;
        _repeatingEndX = repeatingSegments[0].EndX;
        _looping = true;
    }

    public void StopLooping()
    {
        _looping = false;
        repeatingSegments.RemoveAt(0);
    }
    
    public void ResetLevel()
    {
        Scene currentScene = SceneManager.GetActiveScene(); 
        SceneManager.LoadScene(currentScene.name);
    }
}
