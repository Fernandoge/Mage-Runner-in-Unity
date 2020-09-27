using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyController : MonoBehaviour
{
    [Header("Base Enemy Fields")]
    [SerializeField] private float _distanceToSpawn;
    [SerializeField] private bool _enablesLevelLoop;
    
    private SpriteRenderer _spriteRenderer;
    private BoxCollider2D _boxCollider;
    private float _distanceBetweenPlayerX;

    protected SpriteRenderer spriteRenderer => _spriteRenderer;
    protected float distanceBetweenPlayerX => _distanceBetweenPlayerX;
    
    private void OnDestroy()
    {
        if (_enablesLevelLoop)
            GameManager.Instance.level.StopLooping();
    }

    protected virtual void Start()
    {
        
        _boxCollider = GetComponent<BoxCollider2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _boxCollider.enabled = false;
        _spriteRenderer.enabled = false;
    }
    

    protected virtual void Update()
    {
        _distanceBetweenPlayerX = GameManager.Instance.player.transform.position.x - transform.position.x;
        
        if (_spriteRenderer.enabled == false && _distanceBetweenPlayerX <= _distanceToSpawn)
        {
            _spriteRenderer.enabled = true;
            _boxCollider.enabled = true;
            //transform.SetParent(GameManager.Instance.level.movingObjects);
            if (_enablesLevelLoop)
                GameManager.Instance.level.StartLooping();
        }
        
#if UNITY_EDITOR
        DebugDistanceOnClick();
#endif
        
    }
    
    private void DebugDistanceOnClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity);
            if (hit.collider?.transform == transform)
            {
                print(_distanceBetweenPlayerX);
            }
        }
    }
}
