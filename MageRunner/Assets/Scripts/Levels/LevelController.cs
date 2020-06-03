using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelController : MonoBehaviour
{
    public float movingSpeed;
    public Transform movingObjects;

    private void Start() => GameManager.Instance.level = this;

    private void Update()
    {
        if (GameManager.Instance.player.moving)
            movingObjects.Translate(Vector2.right * movingSpeed * Time.deltaTime);
    }

    public void ResetLevel()
    {
        Scene currentScene = SceneManager.GetActiveScene(); 
        SceneManager.LoadScene(currentScene.name);
    }
}
