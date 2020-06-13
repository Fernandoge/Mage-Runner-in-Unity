using System;
using UnityEngine;

public class Attack : MonoBehaviour
{
    public float speed;
    public float durationNoVisible;
    public bool reduceDurationVisible;
    public LayerMask noDestroyLayer;
    public Rigidbody2D rigBody;
    public ParticleSystem[] particlesToActivate;
    public ParticleSystem[] particlesToDetach;

    [System.NonSerialized] public bool preparingReflect = false;
    
    private bool _isVisible;
    
    private void OnDisable() => DetachParticles();

    private void OnBecameVisible() => _isVisible = true;

    private void OnBecameInvisible() => _isVisible = false;

    private void Update()
    {
        if (preparingReflect && transform.position.x > GameManager.Instance.player._shooterSpellOriginalPos.x)
            transform.Translate(Vector2.left * rigBody.velocity * (Time.deltaTime / 40));

        if (_isVisible && reduceDurationVisible == false)
            return;
        
        durationNoVisible -= Time.deltaTime;
        if (durationNoVisible <= 0)
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
        
        // return if the attack collides with a configured no Destroy layer
        if (((1 << collision.gameObject.layer) & noDestroyLayer) != 0)
            return;
        
        ActivateParticles();
        Destroy(gameObject);

        if (collision.CompareTag("StatsHolder"))
        {
            //TODO: Change this when stats are implemented
        }
    }

    private void ActivateParticles()
    {
        foreach (ParticleSystem particle in particlesToActivate)
        {
            particle.gameObject.SetActive(true);
        }
    }
    
    private void DetachParticles()
    {
        foreach (ParticleSystem particle in particlesToDetach)
        {
            particle.transform.SetParent(null);
            Destroy(particle.gameObject, particle.main.startLifetimeMultiplier);
            if (particle.main.loop == true)
                particle.Stop();
        }
    }
}
