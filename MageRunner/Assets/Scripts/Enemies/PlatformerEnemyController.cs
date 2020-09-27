using System;
using UnityEngine;

public class PlatformerEnemyController : EnemyController
{

    [Header("Platformer Fields")]
    [SerializeField] private bool _followPlayer;
    [SerializeField] private bool _patrol;
    [SerializeField] private float _patrolSpeed;
    [SerializeField] private int _collisionDamage;
    [SerializeField] private Transform _groundDetector;
    // [SerializeField] private Collider2D _platformCollider;

    private bool _movingLeft = true;
    private Vector2 _direction = Vector2.left;
    private LayerMask _notGroundLayerMask;
    
    protected bool movingLeft => _movingLeft;

    protected override void Start()
    {
        base.Start();
        _notGroundLayerMask = 1 << LayerMask.NameToLayer("Ground") | 1 << LayerMask.NameToLayer("BottomGround");
    }

    protected override void Update()
    {
        base.Update();
        if (_patrol)
            Patrol();
        if (_followPlayer)
            FollowPlayer();
    }
    
    private void ChangeDirection(bool flipSprite)
    {
        _movingLeft = ! _movingLeft;
        _direction = _movingLeft ? Vector2.left : Vector2.right;
        _groundDetector.transform.localPosition = new Vector2(-_groundDetector.transform.localPosition.x, _groundDetector.transform.localPosition.y);
        if (flipSprite)
           spriteRenderer.flipX = !spriteRenderer.flipX;
    }

    private void Patrol()
    {
        transform.Translate(_direction * (_patrolSpeed * Time.deltaTime));

        RaycastHit2D groundInfo = Physics2D.Raycast(_groundDetector.position, Vector2.down, 0.2f, _notGroundLayerMask);
        if (groundInfo.collider == null)
            ChangeDirection(! _followPlayer);
    }

    private void FollowPlayer()
    {
        if (_movingLeft && distanceBetweenPlayerX > 0)
            ChangeDirection(true);
    }
    
    
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.layer != LayerMask.NameToLayer("Player"))
            return;
        
        if (GameManager.Instance.player.stateHandler.isFastFalling)
        {
            Destroy(gameObject);
            GameManager.Instance.player.rigidBody.velocity = Vector2.up * GameManager.Instance.player.jumpForce;
            GameManager.Instance.player.Running();
        }
        else
        {
            Stats playerStats = col.GetComponent<Stats>();
            playerStats.HandleAttack(_collisionDamage, EElement.Neutral);
        }
    }
}