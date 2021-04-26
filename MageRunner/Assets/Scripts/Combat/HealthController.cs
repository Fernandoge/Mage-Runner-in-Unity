using UnityEngine;

namespace MageRunner.Combat
{
    public class HealthController : MonoBehaviour
    {
        [SerializeField] private Transform _healthpointsBarHolder;
        [SerializeField] private Transform _healthpointsBarBackground;
        public float _currentHealthpoints;
        public int _totalHealthpoints;
    
        public void Initialize(int playerHealthpoints)
        {
            _totalHealthpoints = playerHealthpoints;
            _currentHealthpoints = _totalHealthpoints;
        }
    
        public void UpdateHealthpoints(int value)
        {
            _currentHealthpoints = _currentHealthpoints + value > _totalHealthpoints ? _totalHealthpoints : _currentHealthpoints + value;
            float barValue = _currentHealthpoints / _totalHealthpoints < 0 ? 0 : _currentHealthpoints / _totalHealthpoints;

            _healthpointsBarHolder.localScale = new Vector3(barValue, _healthpointsBarHolder.localScale.y, _healthpointsBarHolder.localScale.z);
        
            if (_currentHealthpoints <= 0)
                Debug.LogError("PLAYER DIED, WIP");
        }
    }
}
