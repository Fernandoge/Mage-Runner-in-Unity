using System;
using UnityEngine;

public class Attack : MonoBehaviour
{
    public float speed;
    public Rigidbody2D rigBody;
    
    [System.NonSerialized] public int shooterLayer;
    [System.NonSerialized] public bool preparingReflect;

    [SerializeField] private int _damage;
    [SerializeField] private EElement _element;
    [SerializeField] private float _durationNoVisible;
    [SerializeField] private bool _reduceDurationVisible;
    [SerializeField] private LayerMask _destroyLayers;
    [SerializeField] private ParticleSystem[] _particlesToActivate;
    
    private bool _isVisible;

    private void OnDisable() => ActivateParticles();

    private void OnBecameVisible() => _isVisible = true;

    private void OnBecameInvisible() => _isVisible = false;

    private void Update()
    {
        if (preparingReflect && transform.localPosition.x > GameManager.Instance.player._shooterSpellOriginalPos.x)
            transform.Translate(Vector2.left * rigBody.velocity * (Time.deltaTime / 40));

        if (_isVisible && _reduceDurationVisible == false)
            return;
        
        _durationNoVisible -= Time.deltaTime;
        if (_durationNoVisible <= 0)
            Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        int collisionLayer = collision.gameObject.layer;
            
        // Player Reflect
        if (collisionLayer == LayerMask.NameToLayer("ReflectAura"))
        {
            rigBody.simulated = false;
            preparingReflect = true;
            transform.SetParent(collision.transform.parent);
            GameManager.Instance.player.reflectedAttacks.Add(this);
        }
        
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
            
            Stats collisionStats = collision.GetComponent<Stats>();
            collisionStats.HandleAttack(_damage, _element);
        }
        
        // End the OnTrigger destroying the attack if the collision layer is part of the destroy layers
        if (((1 << collisionLayer) & _destroyLayers) != 0)
            Destroy(gameObject);
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
