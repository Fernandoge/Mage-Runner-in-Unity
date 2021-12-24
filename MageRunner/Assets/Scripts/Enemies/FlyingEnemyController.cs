using System.Collections;
using MageRunner.Gestures;
using MageRunner.Managers.GameManager;
using MageRunner.Scenes;
using UnityEngine;

namespace MageRunner.Enemies
{
    public class FlyingEnemyController : EnemyController
    {
        [Header("Flying Enemy Fields")]
        [SerializeField] private int _idealPosition;
        [SerializeField] private float _flyingSpeed;
        [SerializeField] private GesturesHolderController _gesturesHolderController;

        private FlyingEnemyAreasColumn _enemyAreasColumn;
        private FlyingEnemyArea _enemyArea;
        
        private void OnEnable()
        {
            _gesturesHolderController.deactivate += OnDeactivate;
            if (GameManager.Instance.player != null) 
                GameManager.Instance.player.playerDash += OnPlayerDash;

        }
        
        private void OnDisable()
        {
            _gesturesHolderController.deactivate -= OnDeactivate;
            if (GameManager.Instance.player != null)
                GameManager.Instance.player.playerDash -= OnPlayerDash;
        }

        private void OnPlayerDash() => StartCoroutine(MoveToIdlePosition(" el dashsito"));

        private IEnumerator MoveToIdlePosition(string sdf)
        {
            Vector3 destiny = new Vector3(0, 0, Mathf.Abs(GameManager.Instance.mainCamera.transform.position.z));
            while (Vector3.Distance(transform.localPosition,destiny) > 0)
            {
                transform.localPosition = Vector3.MoveTowards(transform.localPosition, destiny, _flyingSpeed * Time.deltaTime);
                yield return null;
            }
            _gesturesHolderController.ActivateGestures();
        }

        // Called in Spawn Animation
        public void OnSpawn()
        {
            GameManager.Instance.level.flyingEnemiesGesturesHolderController.Add(_gesturesHolderController);
            _enemyArea = FindPosition();
            _enemyArea.isPositionOccupied = true;
            transform.SetParent(_enemyArea.area);
            _gesturesHolderController.isMoving = true;

            StartCoroutine(MoveToIdlePosition("el spawnsito"));
        }

        private void OnDeactivate()
        {
            _enemyArea.isPositionOccupied = false;
            _enemyAreasColumn.isColumnOccupied = false;
            GameManager.Instance.level.flyingEnemiesGesturesHolderController.Remove(_gesturesHolderController);
        }

        private FlyingEnemyArea FindPosition()
        {
            if (_idealPosition == 0)
                _idealPosition = Random.Range(1, 4);
            
            LevelSceneController levelScene = GameManager.Instance.levelScene;
            for (int i = 0; i < levelScene.flyingEnemyAreas.Length; i++)
            {
                if (levelScene.flyingEnemyAreas[i].isColumnOccupied)
                    continue;
                
                _enemyAreasColumn = levelScene.flyingEnemyAreas[i];

                if (_enemyAreasColumn.enemyAreas[_idealPosition].isPositionOccupied == false)
                    return _enemyAreasColumn.enemyAreas[_idealPosition];

                // check above the ideal position
                for (int j = _idealPosition - 1; j >= 0; j--)
                    if (_enemyAreasColumn.enemyAreas[j].isPositionOccupied == false)
                        return _enemyAreasColumn.enemyAreas[j];
                
                
                // check below the ideal position
                for (int j = _idealPosition + 1; j < _enemyAreasColumn.enemyAreas.Length; j++)
                    if (_enemyAreasColumn.enemyAreas[j].isPositionOccupied == false)
                        return _enemyAreasColumn.enemyAreas[j];
                
                _enemyAreasColumn.isColumnOccupied = true;
            }
            
            Debug.LogError("No position for this flying enemy! " + gameObject);
            return new FlyingEnemyArea();
        }
    }
}
