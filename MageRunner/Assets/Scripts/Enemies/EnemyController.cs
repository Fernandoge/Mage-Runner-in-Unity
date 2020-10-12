using System;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("Base Enemy Fields")]
    [SerializeField] private float _distanceToSpawn;
    [SerializeField] bool _enablesLevelLoop;
    
    private SpriteRenderer _spriteRenderer;
    private BoxCollider2D _boxCollider;
    private float _distanceBetweenPlayerX;
    
    public float distanceToSpawn => _distanceToSpawn;
    public bool enablesLevelLoop => _enablesLevelLoop;
    protected SpriteRenderer spriteRenderer => _spriteRenderer;
    protected float distanceBetweenPlayerX => _distanceBetweenPlayerX;
    
    private void OnDestroy()
    {
        if (_enablesLevelLoop)
            GameManager.Instance.level.StopLooping();
    }

    protected virtual void Start() => _spriteRenderer = GetComponent<SpriteRenderer>();


    protected virtual void Update()
    {
        _distanceBetweenPlayerX = transform.position.x - GameManager.Instance.player.transform.position.x;

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
