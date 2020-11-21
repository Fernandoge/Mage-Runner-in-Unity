using System;
using UnityEngine;

public class Stats : MonoBehaviour
{
    [SerializeField] private EElement _element;
    [SerializeField] private int _healthpoints;
    [SerializeField] private Transform _healthpointsBarHolder;

    private float _currentHealthpoints;

    private void Start() => _currentHealthpoints = _healthpoints;

    public virtual void HandleAttack(int attackDamage, EElement attackElement)
    {
        _currentHealthpoints -= attackDamage * GameManager.Instance.elementsMultipliersDict[(_element, attackElement)];
        float barValue = _currentHealthpoints / _healthpoints;
        _healthpointsBarHolder.localScale = new Vector3(barValue, _healthpointsBarHolder.localScale.y, _healthpointsBarHolder.localScale.z);

        if (_healthpointsBarHolder.parent.gameObject.activeSelf == false)
            _healthpointsBarHolder.parent.gameObject.SetActive(true);
        
        if (_currentHealthpoints <= 0)
            DestroyGameObject();
    }

    public virtual void DestroyGameObject()
    {
        Destroy(gameObject);
    }
}
