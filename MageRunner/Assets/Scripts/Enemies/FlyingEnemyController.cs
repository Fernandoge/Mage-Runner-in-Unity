using System.Collections;
using DG.Tweening;
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

        private FlyingEnemyAreasColumn _enemyAreasColumn;
        private FlyingEnemyArea _enemyArea;
        
        private void OnEnable()
        {
            gesturesHolderController.deactivate += OnDeactivate;
            if (GameManager.Instance.player != null) 
                GameManager.Instance.player.playerDash += OnPlayerDash;

        }
        
        private void OnDisable()
        {
            gesturesHolderController.deactivate -= OnDeactivate;
            if (GameManager.Instance.player != null)
                GameManager.Instance.player.playerDash -= OnPlayerDash;
        }

        private void OnPlayerDash()
        {
            if (gesturesHolderController.gesturesLoaded)
                StartCoroutine(MoveToIdlePosition());
        }

        private IEnumerator MoveToIdlePosition()
        {
            Vector3 destiny = new Vector3(0, 0, Mathf.Abs(GameManager.Instance.mainCamera.transform.position.z));
            Tween moveTween = transform.DOLocalMove(destiny, 1);

            yield return moveTween.WaitForCompletion();
            
            gesturesHolderController.ActivateGestures();
        }

        // Called in Spawn Animation
        public void OnSpawn()
        {
            GameManager.Instance.level.flyingEnemiesGesturesHolderController.Add(gesturesHolderController);
            _enemyArea = FindPosition();
            _enemyArea.isPositionOccupied = true;
            transform.SetParent(_enemyArea.transform);
            gesturesHolderController.isMoving = true;

            StartCoroutine(MoveToIdlePosition());
        }

        private void OnDeactivate()
        {
            _enemyArea.isPositionOccupied = false;
            _enemyAreasColumn.isColumnOccupied = false;
            GameManager.Instance.level.flyingEnemiesGesturesHolderController.Remove(gesturesHolderController);
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
