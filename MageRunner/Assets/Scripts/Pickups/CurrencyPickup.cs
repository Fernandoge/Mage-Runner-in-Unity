using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrencyPickup : Pickup
{
    [SerializeField] private int _currencyAddedValue;
    [SerializeField] private float _animationSpeed;
    [SerializeField] private Transform _animationTarget;
    
    protected override void PickObject()
    {
        StartCoroutine(MoveToTarget(_animationTarget, _animationSpeed));
    }

    protected override void FinishAnimation()
    {
        base.FinishAnimation();
        GameManager.Instance.level.UpdateCurrency(_currencyAddedValue);
    }
}
