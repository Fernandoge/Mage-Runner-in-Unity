using MageRunner.Managers.GameManager;
using UnityEngine;

namespace MageRunner.Levels
{
    public class MovingParticles : MonoBehaviour
    {
        private bool _usesVelocityOverLifetime;
        private ParticleSystem.VelocityOverLifetimeModule _particleSystemVelocityModule;
        private float originalVelocity;
    
        private void Start()
        {
            GameManager.Instance.level.movingParticles.Add(this);
            _particleSystemVelocityModule = GetComponent<ParticleSystem>().velocityOverLifetime;
            originalVelocity = _particleSystemVelocityModule.xMultiplier;
        
            if (_particleSystemVelocityModule.enabled)
            {
                _usesVelocityOverLifetime = true;
                _particleSystemVelocityModule.enabled = false;
            }
        }

        public void EnableVelocityOverLifetime()
        {
            if (_usesVelocityOverLifetime)
                _particleSystemVelocityModule.enabled = true;
        }
    
        public void DisableVelocityOverLifetime()
        {
            if (_usesVelocityOverLifetime)
                _particleSystemVelocityModule.enabled = false;
        }

        public void ModifyVelocityOverLifetimeSpeed(float value)
        {
            // _particleSystemVelocityModule.xMultiplier = (value + _particleSystemVelocityModule.xMultiplier) - (value - GameManager.Instance.level.movingSpeed);
            _particleSystemVelocityModule.xMultiplier = (value + _particleSystemVelocityModule.xMultiplier) - (GameManager.Instance.level.movingSpeed - _particleSystemVelocityModule.xMultiplier);
        }

        public void ResetVelocityOverLifetimeSpeed()
        {
            _particleSystemVelocityModule.xMultiplier = originalVelocity;
        }
    }
}
