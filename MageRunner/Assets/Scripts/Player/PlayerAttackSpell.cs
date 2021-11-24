using System.Collections;
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
        private GesturesHolderController _target;

        private void OnDisable() => ActivateParticles();

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
            // Handle attack to interact with the attackTarget gestures

            if (collider.gameObject != _target.gameObject)
                return;
            
            if (_reduceDurationVisible == false)
                Destroy(gameObject);

            _target.activeGestures -= 1;
                
            if (_target.activeGestures != 0) 
                return;
                
            // TODO: Improve gestures holder dead
            _target.gameObject.SetActive(false);
                
            if (collider.GetComponent<ForceFtueGesture>() != null)
                FtueManager.Instance.CurrentFtueDestroyedStep();
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
