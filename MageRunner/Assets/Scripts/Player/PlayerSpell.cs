using UnityEngine;

public class PlayerSpell : MonoBehaviour
{
    public float speed;
    public Rigidbody2D rigBody;
    
    [System.NonSerialized] public int shooterLayer;
    [System.NonSerialized] public bool preparingReflect;

    [SerializeField] private int _damage;
    [SerializeField] private float _durationNoVisible;
    [SerializeField] private bool _reduceDurationVisible;
    [SerializeField] private ParticleSystem[] _particlesToActivate;
    
    private bool _isVisible;

    public int damage => _damage;

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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        int collisionLayer = collision.gameObject.layer;

        // Player Block
        if (collisionLayer != LayerMask.NameToLayer("Player"))
        {
            if (GameManager.Instance.player.stateHandler.isBlocking)
                Destroy(gameObject);
        }
        
        // Handle attack according to the collision stats
        if (collision.CompareTag("StatsHolder"))
        {
            if (collisionLayer == LayerMask.NameToLayer("Enemy") && shooterLayer == LayerMask.NameToLayer("Enemy"))
                return;
            
            // Kill Player or do gesture interaction
            
            
            // Stats collisionStats = collision.GetComponent<Stats>();
            // collisionStats.HandleAttack(damage, element);
        }
        
        // End the OnTrigger destroying the attack if the collision layer is part of the destroy layers
        // if (((1 << collisionLayer) & _destroyLayers) != 0)
        //     Destroy(gameObject);
    }

    private void ActivateParticles()
    {
        foreach (ParticleSystem particle in _particlesToActivate)
        {
            if (particle.gameObject.activeSelf == false)
                particle.gameObject.SetActive(true);
            
            particle.transform.SetParent(null);
            Destroy(particle.gameObject, particle.main.startLifetimeMultiplier);
            if (particle.main.loop)
                particle.Stop();
        }
    }
}
