using System;
using System.Collections;
using MageRunner.Enemies;
using MageRunner.FTUE;
using MageRunner.Gestures;
using MageRunner.Managers.FtueManager;
using MageRunner.Managers.GameManager;
using UnityEngine;

namespace MageRunner.Player
{
    public class PlayerAttackSpell : MonoBehaviour
    {
        public Rigidbody2D rigBody;
        
        [SerializeField] private float _durationNoVisible;
        [SerializeField] private bool _reduceDurationVisible;
        [SerializeField] private ParticleSystem[] _particlesToActivate;
        
        private bool _isVisible;
        private int _healthpoints;
        private GesturesHolderController _target;

        private void OnDisable()
        {
            ActivateParticles();
            GameManager.Instance.gameActivePlayerSpells.Remove(this);
        }

        private void OnBecameVisible() => _isVisible = true;

        private void OnBecameInvisible() => _isVisible = false;

        private void Update()
        {
            if (_isVisible && _reduceDurationVisible == false)
                return;
        
            _durationNoVisible -= Time.deltaTime;
            if (_durationNoVisible <= 0)
                Destroy(gameObject);
        }

        private void OnTriggerEnter2D(Collider2D collider)
        {
            if (collider.gameObject != _target.gameObject)
                return;
            
            if (!_reduceDurationVisible)
                Destroy(gameObject);

            _target.activeGestures -= 1;
                
            if (_target.activeGestures != 0) 
                return;

            if (_target.infiniteGestures)
                RemoveHP();
            else
            {
                // TODO: Improve gestures holder dead
                _target.Deactivate();
            }

            if (collider.GetComponent<ForceFtueGesture>() != null)
                FtueManager.Instance.CurrentFtueDestroyedStep();
        }

        private void RemoveHP()
        {
            _target.currentHealthpoints -= 1;
            if (_target.currentHealthpoints <= 0)
                _target.ZeroHpCallback();
        }
        
        public void StartMovingToTarget(GesturesHolderController attackTarget, float speed, GameObject playerSpellShooter)
        {
            _target = attackTarget;
            rigBody.velocity = playerSpellShooter.transform.right * speed;
        }

        private void ActivateParticles()
        {
            foreach (ParticleSystem particle in _particlesToActivate)
            {
                if (particle.gameObject.activeSelf == false)
                    particle.gameObject.SetActive(true);
            
                particle.transform.SetParent(transform.parent);
                Destroy(particle.gameObject, particle.main.startLifetimeMultiplier);
                if (particle.main.loop)
                    particle.Stop();
            }
        }
    }
}
