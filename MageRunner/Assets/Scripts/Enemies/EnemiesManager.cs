using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemiesManager : MonoBehaviour
{
    public static EnemiesManager Instance;
    
    private List<List<EnemyController>> _timeFramesEnemies = new List<List<EnemyController>>();
    private List<EnemyController> _enemies = new List<EnemyController>();
    private int _currentEnemyIndex;
    private bool _isEnemyListSorted;
    private bool _spawningEnemy;
    private float _distanceBetweenPlayerX;
    
    private void Awake() => Instance = this;

    private void Update()
    {
        if (_currentEnemyIndex == _enemies.Count)
            return;
        
        EnemyController currentEnemy = _enemies[_currentEnemyIndex];
        
        _distanceBetweenPlayerX = currentEnemy.transform.position.x - GameManager.Instance.player.transform.position.x;

        if (_distanceBetweenPlayerX <= currentEnemy.distanceToSpawn)
        {
            _currentEnemyIndex += 1;
            currentEnemy.gameObject.SetActive(true);
            if (currentEnemy.enablesLevelLoop)
                GameManager.Instance.level.StartLooping();
        }
    }

    public void InitializeEnemies()
    {
        foreach (TimeFrame timeframe in GameManager.Instance.level.timeFrames)
        {
            List<EnemyController> currentTimeFrameEnemies = new List<EnemyController>();
            currentTimeFrameEnemies.AddRange(timeframe.movingGO.GetComponentsInChildren<EnemyController>());
            currentTimeFrameEnemies.AddRange(timeframe.staticGO.GetComponentsInChildren<EnemyController>());
            currentTimeFrameEnemies = currentTimeFrameEnemies.OrderBy(enemy => enemy.transform.position.x).ToList();
            
            foreach (EnemyController enemy in currentTimeFrameEnemies)
                enemy.gameObject.SetActive(false);
            
            _timeFramesEnemies.Add(currentTimeFrameEnemies);
        }

        _enemies = _timeFramesEnemies[0];
    }

    public void ChangeEnemiesList()
    {
        _enemies = _timeFramesEnemies[GameManager.Instance.level.currentTimeFrameIndex];
        _currentEnemyIndex = 0;
    }
}
