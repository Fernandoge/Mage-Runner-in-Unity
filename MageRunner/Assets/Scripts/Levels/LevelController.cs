using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelController : MonoBehaviour
{
    public float movingSpeed;
    
    private void Start()
    {
        GameManager.Instance.level = this;
        enabled = false;
    }

    void Update()
    {
        transform.Translate(Vector2.left * movingSpeed * Time.deltaTime);
    }

    public void ResetLevel()
    {
        Scene currentScene = SceneManager.GetActiveScene(); 
        SceneManager.LoadScene(currentScene.name);
    }
}
