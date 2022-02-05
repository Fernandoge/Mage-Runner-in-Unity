using MageRunner.Managers.GameManager;
using UnityEngine;

namespace MageRunner.Levels
{
    public class MovingParticle : MonoBehaviour
    {
        private bool _usesVelocityOverLifetime;
        private ParticleSystem.VelocityOverLifetimeModule _particleSystemVelocityModule;
        private ParticleSystem.MainModule _particleSystemMainModule;
        private float originalVelocity;
    
        private void Start()
        {
            GameManager.Instance.level.movingParticles.Add(this);
            ParticleSystem particleSystem = GetComponent<ParticleSystem>();
            _particleSystemMainModule = particleSystem.main;
            _particleSystemVelocityModule = particleSystem.velocityOverLifetime;
            originalVelocity = _particleSystemVelocityModule.xMultiplier;

            if (!_particleSystemVelocityModule.enabled) 
                return;
            
            _usesVelocityOverLifetime = true;
            _particleSystemVelocityModule.enabled = false;
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
            _particleSystemVelocityModule.xMultiplier = value * _particleSystemVelocityModule.xMultiplier / GameManager.Instance.level.movingSpeed;
        }

        public void ResetVelocityOverLifetimeSpeed()
        {
            _particleSystemVelocityModule.xMultiplier = originalVelocity;
        }
    }
}
