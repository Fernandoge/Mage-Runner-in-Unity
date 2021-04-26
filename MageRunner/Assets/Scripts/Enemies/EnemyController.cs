using UnityEngine;

namespace MageRunner.Enemies
{
    public class EnemyController : MonoBehaviour
    {
        // [Header("Base Enemy Fields")]
        private SpriteRenderer _spriteRenderer;
        private BoxCollider2D _boxCollider;
        private float _distanceBetweenPlayerX;
    
        protected SpriteRenderer spriteRenderer => _spriteRenderer;

        protected virtual void Start() => _spriteRenderer = GetComponent<SpriteRenderer>();
    }
}
