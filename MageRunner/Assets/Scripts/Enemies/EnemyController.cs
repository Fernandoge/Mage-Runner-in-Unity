using MageRunner.Gestures;
using UnityEngine;

namespace MageRunner.Enemies
{
    public class EnemyController : MonoBehaviour
    {
        // [Header("Base Enemy Fields")]
        private SpriteRenderer _spriteRenderer;
        private GesturesHolderController _gesturesHolderController;
        private Animator _animator;

        protected SpriteRenderer spriteRenderer => _spriteRenderer;
        protected GesturesHolderController gesturesHolderController => _gesturesHolderController;
        protected Animator animator => _animator;

        protected virtual void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _gesturesHolderController = GetComponent<GesturesHolderController>();
            _animator = GetComponent<Animator>();
        }
    }
}
