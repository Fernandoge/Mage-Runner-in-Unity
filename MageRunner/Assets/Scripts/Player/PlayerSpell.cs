using UnityEngine;

public class PlayerSpell : MonoBehaviour
{
    public LayerMask noDestroyLayers;
    public ParticleSystem[] particlesToActivate;
    public ParticleSystem[] particlesToDetach;
    
    [System.NonSerialized] public float duration;

    private void OnDisable()
    {
        DetachParticles();
    }

    private void Update()
    {
        duration -= Time.deltaTime;
        if (duration <= 0)
            Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (((1 << collision.gameObject.layer) & noDestroyLayers) == 0)
        {
            ActivateParticles();
            Destroy(gameObject);
        }

        if (collision.CompareTag("Enemy"))
        {
            //TODO: Change this when enemy health is implemented
            ActivateParticles();
            //Destroy(collision.gameObject);
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
