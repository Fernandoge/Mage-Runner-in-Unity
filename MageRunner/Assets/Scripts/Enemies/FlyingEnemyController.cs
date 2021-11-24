using System.Collections;
using MageRunner.Gestures;
using MageRunner.Managers.GameManager;
using UnityEngine;

namespace MageRunner.Enemies
{
    public class FlyingEnemyController : EnemyController
    {
        [Header("Flying Enemy Fields")]
        [SerializeField] private float _flyingSpeed;
        [SerializeField] private GesturesHolderController _gesturesHolderController;
        
        private void OnEnable()
        {
            _gesturesHolderController.gesturesActivation += OnGesturesActivation;
            if (GameManager.Instance.player != null) 
                GameManager.Instance.player.playerDash += OnPlayerDash;
        }
        
        private void OnDisable()
        {
            _gesturesHolderController.gesturesActivation -= OnGesturesActivation;
            if (GameManager.Instance.player != null)
                GameManager.Instance.player.playerDash -= OnPlayerDash;
            
            GameManager.Instance.level.flyingEnemiesGesturesHolderController.Remove(_gesturesHolderController);
        }

        private void OnGesturesActivation()
        {
            transform.SetParent(GameManager.Instance.level.movingObjects);
            _gesturesHolderController.isMoving = true;
            GameManager.Instance.level.flyingEnemiesGesturesHolderController.Add(_gesturesHolderController);
        }

        private void OnPlayerDash() => StartCoroutine(MoveToIdlePosition());

        private IEnumerator MoveToIdlePosition()
        {
            while (_gesturesHolderController.distanceBetweenPlayerX <= _gesturesHolderController.distanceToSpawn)
            {
                transform.Translate(Vector2.right * (_flyingSpeed * Time.deltaTime));
                yield return null;    
            }
        }
    }
}
