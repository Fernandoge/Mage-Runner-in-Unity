using UnityEngine;

public class Attack : MonoBehaviour
{
    public float speed;
    public Rigidbody2D rigBody;
    
    [SerializeField] private int _damage;
    [SerializeField] private EElement _element;
    [SerializeField] private float _durationNoVisible;
    [SerializeField] private bool _reduceDurationVisible;
    [SerializeField] private LayerMask _destroyLayers;
    [SerializeField] private ParticleSystem[] _particlesToActivate;
    [SerializeField] private ParticleSystem[] _particlesToDetach;
        
    [System.NonSerialized] public bool preparingReflect = false;
    
    private bool _isVisible;
    
    private void OnDisable() => DetachParticles();

    private void OnBecameVisible() => _isVisible = true;

    private void OnBecameInvisible() => _isVisible = false;

    private void Update()
    {
        if (preparingReflect && transform.position.x > GameManager.Instance.player._shooterSpellOriginalPos.x)
            transform.Translate(Vector2.left * rigBody.velocity * (Time.deltaTime / 40));

        if (_isVisible && _reduceDurationVisible == false)
            return;
        
        _durationNoVisible -= Time.deltaTime;
        if (_durationNoVisible <= 0)
            Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("ReflectAura"))
        {
            rigBody.simulated = false;
            preparingReflect = true;
            transform.SetParent(collision.transform.parent);
            GameManager.Instance.player.reflectedAttacks.Add(this);
        }
        
        if (collision.gameObject.layer != LayerMask.NameToLayer("Player"))
        {
            if (GameManager.Instance.player.stateHandler.isBlocking)
                Destroy(gameObject);
        }
        
        if (collision.CompareTag("StatsHolder"))
        {
            Stats collisionStats = collision.GetComponent<Stats>();
            collisionStats.HandleAttack(_damage, _element);
        }
        
        if (((1 << collision.gameObject.layer) & _destroyLayers) != 0)
        {
            ActivateParticles();
            Destroy(gameObject);
        }
    }

    private void ActivateParticles()
    {
        foreach (ParticleSystem particle in _particlesToActivate)
        {
            particle.gameObject.SetActive(true);
        }
    }
    
    private void DetachParticles()
    {
        foreach (ParticleSystem particle in _particlesToDetach)
        {
            particle.transform.SetParent(null);
            Destroy(particle.gameObject, particle.main.startLifetimeMultiplier);
            if (particle.main.loop == true)
                particle.Stop();
        }
    }
}
