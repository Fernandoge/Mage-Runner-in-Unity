using MageRunner.Managers.GameManager;
using UnityEngine;

namespace MageRunner.Pickups
{
    public class ManaPickup : Pickup
    {
        [SerializeField] private int _manaRestoreValue;
        [SerializeField] private float _animationSpeed; 
    
        private Transform _animationTarget;

        private void Start() => _animationTarget = GameManager.Instance.player.manaController.transform;

        protected override void PickObject() => StartCoroutine(MoveToTarget(_animationTarget, _animationSpeed));

        protected override void FinishAnimation()
        {
            base.FinishAnimation();
            GameManager.Instance.player.manaController.UpdateMana(_manaRestoreValue);
        }
    }
}
