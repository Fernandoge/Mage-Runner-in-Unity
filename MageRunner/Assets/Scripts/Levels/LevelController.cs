using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public struct RepeatingPlatform
{
    public GameObject platformsHolder;
    public float StartX;
    public float EndX;
}

public class LevelController : MonoBehaviour
{
    public float movingSpeed;
    public Transform movingObjects;
    public Transform instantiatedObjects;
    public List<RepeatingPlatform> repeatingPlatforms;

    private bool _looping;
    private float _repeatingStartX;
    private float _repeatingEndX;

    private void Start() => GameManager.Instance.level = this;

    private void Update()
    {
        //TODO: Optimize the game objects positions if they are high values to avoid floating-point precision limitations
        
        if (GameManager.Instance.player.moving)
            movingObjects.Translate(Vector2.right * movingSpeed * Time.deltaTime);
        
        if (_looping && movingObjects.transform.position.x >= _repeatingEndX)
        {
            float distanceReturned = _repeatingEndX - _repeatingStartX;
            movingObjects.transform.position = new Vector2(_repeatingStartX, movingObjects.transform.position.y);
            instantiatedObjects.transform.position = new Vector2(instantiatedObjects.transform.position.x - distanceReturned, instantiatedObjects.transform.position.y);
        }
    }

    public void StartLooping()
    {
        _repeatingStartX = repeatingPlatforms[0].StartX;
        _repeatingEndX = repeatingPlatforms[0].EndX;
        _looping = true;
    }

    public void StopLooping()
    {
        _looping = false;
        repeatingPlatforms.RemoveAt(0);
    }
    
    public void ResetLevel()
    {
        Scene currentScene = SceneManager.GetActiveScene(); 
        SceneManager.LoadScene(currentScene.name);
    }
}
