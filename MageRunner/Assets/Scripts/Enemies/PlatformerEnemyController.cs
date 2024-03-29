﻿using MageRunner.Gestures;
using MageRunner.Managers.GameManager;
using MageRunner.Player;
using UnityEngine;

namespace MageRunner.Enemies
{
    public class PlatformerEnemyController : EnemyController
    {
        [Header("Platformer Fields")] 
        [SerializeField] private bool _facingLeft;
        [SerializeField] private bool _lookPlayer;
        [SerializeField] private bool _patrol;
        [SerializeField] private float _patrolSpeed;
        [SerializeField] private int _collisionDamage;
        [SerializeField] private Transform _groundDetector;
        
        private Vector2 _direction = Vector2.left;
        private LayerMask _notGroundLayerMask;

        protected virtual void Start()
        {
            _direction = _facingLeft ? Vector2.left : Vector2.right;
            _notGroundLayerMask = 1 << LayerMask.NameToLayer("Ground") | 1 << LayerMask.NameToLayer("BottomGround");
        }

        protected virtual void Update()
        {
            if (_patrol)
                Patrol();
            if (_lookPlayer)
                LookPlayer();
        }

        public virtual void PlatformInteract() {}

        private void ChangeDirection(bool flipSprite)
        {
            _facingLeft = ! _facingLeft;
            _direction = _facingLeft ? Vector2.left : Vector2.right;
            _groundDetector.transform.localPosition = new Vector2(-_groundDetector.transform.localPosition.x, _groundDetector.transform.localPosition.y);
            if (flipSprite)
                spriteRenderer.flipX = !spriteRenderer.flipX;
        }

        private void Patrol()
        {
            transform.Translate(_direction * (_patrolSpeed * Time.deltaTime));

            RaycastHit2D groundInfo = Physics2D.Raycast(_groundDetector.position, Vector2.down, 0.2f, _notGroundLayerMask);
            if (groundInfo.collider == null)
                ChangeDirection(! _lookPlayer);
        }

        protected void LookPlayer()
        {
            if (_facingLeft && gesturesHolderController.distanceBetweenPlayerX < 0 || _facingLeft == false && gesturesHolderController.distanceBetweenPlayerX > 0)
                ChangeDirection(true);
        }
    
    
        private void OnTriggerEnter2D(Collider2D col)
        {
            if (col.gameObject.layer != LayerMask.NameToLayer("Player"))
                return;
        
            if (GameManager.Instance.player.stateHandler.isFastFalling)
            {
                Destroy(gameObject);
                GameManager.Instance.player.stateHandler.EnableState(EPlayerState.GroundJumping);
                GameManager.Instance.player.Running();
            }
            //TODO: platformer enemy collision
            else
            {
                // Kill Player
            
                // Stats playerStats = col.GetComponent<Stats>();
                // playerStats.HandleAttack(_collisionDamage, EElement.Neutral);
            }
        }
    }
}