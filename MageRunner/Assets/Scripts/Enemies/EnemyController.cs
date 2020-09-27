using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private float _distanceToSpawn;
    [SerializeField] private float _minDistanceToShoot; 
    [SerializeField] private float _maxDistanceToShoot;
    [SerializeField] private float _minFireRate;
    [SerializeField] private float _maxFireRate;
    [SerializeField] private bool _enablesLevelLoop;
    [SerializeField] private GameObject _weapon;
    [SerializeField] private Attack _objectToShoot;

    protected SpriteRenderer spriteRenderer;
    private float _fireRate;
    private BoxCollider2D _boxCollider;

    protected void Start()
    {
        _fireRate = Random.Range(_minFireRate, _maxFireRate);
        _boxCollider = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        _boxCollider.enabled = false;
        spriteRenderer.enabled = false;
    }
    
    private void OnDestroy()
    {
        if (_enablesLevelLoop)
            GameManager.Instance.level.StopLooping();
    }

    protected virtual void Update()
    {
        float distanceBetweenPlayer = Vector3.Distance(transform.position, GameManager.Instance.player.transform.position);

        if (spriteRenderer.enabled == false && distanceBetweenPlayer <= _distanceToSpawn)
        {
            spriteRenderer.enabled = true;
            _boxCollider.enabled = true;
            //transform.SetParent(GameManager.Instance.level.movingObjects);
            if (_enablesLevelLoop)
                GameManager.Instance.level.StartLooping();
        }

        if (distanceBetweenPlayer >= _minDistanceToShoot && distanceBetweenPlayer <= _maxDistanceToShoot)
        {
            if (_fireRate > 0)
                _fireRate -= Time.deltaTime;
            else
                Shoot();
        }
    }

    private void Shoot()
    {
        Vector2 lookDirection = GameManager.Instance.player.transform.position - _weapon.transform.position;
        float lookAngle = Mathf.Atan2(lookDirection.y, lookDirection.x) * Mathf.Rad2Deg;
        float lookAngleRandomized = lookAngle + Random.Range(-20, 20);
        _weapon.transform.rotation = Quaternion.Euler(0f, 0f, lookAngleRandomized);

        GameObject shootedObject = Instantiate(_objectToShoot.gameObject, _weapon.transform.position, _weapon.transform.rotation, transform.parent);
        float speedAddition = GameManager.Instance.player.isMoving == false ? 0f : GameManager.Instance.level.movingSpeed / 2;
        shootedObject.GetComponent<Rigidbody2D>().velocity = _weapon.transform.right * (_objectToShoot.speed + speedAddition);

        _fireRate = Random.Range(_minFireRate, _maxFireRate);
    }
}
