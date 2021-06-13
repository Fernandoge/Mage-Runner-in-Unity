using MageRunner.Gestures;
using UnityEngine;

namespace MageRunner.Player
{
    public class PlayerAttackSpell : MonoBehaviour
    {
        public float speed;
        public Rigidbody2D rigBody;
    
        [System.NonSerialized] public GameObject target;
        [System.NonSerialized] public bool preparingReflect;
    
        [SerializeField] private float _durationNoVisible;
        [SerializeField] private bool _reduceDurationVisible;
        [SerializeField] private ParticleSystem[] _particlesToActivate;
    
        private bool _isVisible;

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
            int collisionLayer = collider.gameObject.layer;

            // Handle attack to interact with the target gestures
            if (collider.gameObject == target)
            {
                Destroy(gameObject);
                GesturesHolderController gesturesHolderController = collider.GetComponent<GesturesHolderController>();
            
                if (gesturesHolderController == null) 
                    return;
            
                gesturesHolderController.activeGestures -= 1;
                if (gesturesHolderController.activeGestures == 0)
                    collider.gameObject.SetActive(false);
            }
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
