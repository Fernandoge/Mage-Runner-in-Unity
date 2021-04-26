using MageRunner.Managers.GameManager;
using UnityEngine;

namespace MageRunner.Pickups
{
    public class CurrencyPickup : Pickup
    {
        [SerializeField] private int _currencyAddedValue;
        [SerializeField] private float _animationSpeed;
    
        private Transform _animationTarget;

        private void Start()
        {
            _animationTarget = GameManager.Instance.currencyText.transform;
        }

        protected override void PickObject() => StartCoroutine(MoveToTarget(_animationTarget, _animationSpeed));

        protected override void FinishAnimation()
        {
            base.FinishAnimation();
            GameManager.Instance.UpdateCurrency(_currencyAddedValue);
        }
    }
}
