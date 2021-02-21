using UnityEngine;

public class PlatformerShooterEnemyController : PlatformerEnemyController
{
    [Header("Shooter Fields")]
    
    [Tooltip("Distance required to shoot")]
    [SerializeField] private float _minDistanceToShoot;
    
    [Tooltip("Shoots will only occur if the distance is lower than this value")]
    [SerializeField] private float _maxDistanceToShoot;
    
    [SerializeField] private float _minFireRate;
    [SerializeField] private float _maxFireRate;
    
    [Tooltip("Higher values means less accuracy, the value configured is how the look angle to the target can be randomized when shooting a bullet")]
    [SerializeField] private int _aimAccuracyHandicap;
    
    [SerializeField] private GameObject _weapon;
    [SerializeField] private PlayerAttackSpell _objectToShoot;
    
    private float _fireRate;

    protected override void Start()
    {
        base.Start();
        _fireRate = Random.Range(_minFireRate, _maxFireRate);
    }

    protected override void Update()
    {
        base.Update();
        if (gesturesHolderController.distanceBetweenPlayerX > _minDistanceToShoot && gesturesHolderController.distanceBetweenPlayerX < _maxDistanceToShoot)
        {
            if (_fireRate > 0)
                _fireRate -= Time.deltaTime;
            else if (GameManager.Instance.level.isMoving)
                Shoot();
        }
    }
    
    private void Shoot()
    {
        Vector2 weaponLookDirection = GameManager.Instance.player.transform.position - _weapon.transform.position;
        float weaponLookAngle = Mathf.Atan2(weaponLookDirection.y, weaponLookDirection.x) * Mathf.Rad2Deg;
        float weaponLookAngleRandomized = weaponLookAngle + Random.Range( - _aimAccuracyHandicap, _aimAccuracyHandicap);
        _weapon.transform.rotation = Quaternion.Euler(0f, 0f, weaponLookAngleRandomized);

        GameObject shootedObject = Instantiate(_objectToShoot.gameObject, _weapon.transform.position, _weapon.transform.rotation, transform.parent);
        PlayerAttackSpell shootedObjectComponent = shootedObject.GetComponent<PlayerAttackSpell>();
        // shootedObjectComponent.shooterLayer = gameObject.layer;
        float speedAddition = GameManager.Instance.level.isMoving == false ? 0f : GameManager.Instance.level.movingSpeed / 2;
        shootedObjectComponent.rigBody.velocity = _weapon.transform.right * (_objectToShoot.speed + speedAddition);
        
        _fireRate = Random.Range(_minFireRate, _maxFireRate);
    }
}
