using System;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    public float speed;
    public float durationInvisible;
    public LayerMask noDestroyLayers;
    public Rigidbody2D rigBody;

    [System.NonSerialized] public bool preparingReflect;

    [SerializeField] private float _auxDuration;
    private bool _isVisible;

    private void Update()
    {
        if (preparingReflect && transform.position.x > GameManager.Instance.player._shooterSpellOriginalPos.x)
            transform.Translate(Vector2.left * rigBody.velocity * (Time.deltaTime / 40));

        if (_isVisible)
            return;
        
        _auxDuration -= Time.deltaTime;
        if (_auxDuration <= 0)
            Destroy(gameObject);
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (((1 << collision.gameObject.layer) & noDestroyLayers) == 0)
        {
            Destroy(gameObject);
        }

        if (collision.gameObject.layer == LayerMask.NameToLayer("ReflectAura"))
        {
            rigBody.simulated = false;
            preparingReflect = true;
            transform.SetParent(collision.transform.parent);
            GameManager.Instance.player.reflectedAttacks.Add(this);
        }

        if (collision.CompareTag("Player") == false)
            return;
        
        if (GameManager.Instance.player.stateHandler.isBlocking)
        {
            Destroy(gameObject);
        } 
        else
        {
            //TODO: Change this when player health is implemented
            //GameManager.Instance.level.ResetLevel();
        }
    }

    private void OnBecameVisible()
    {
        _auxDuration = durationInvisible;
        _isVisible = true;
    }

    private void OnBecameInvisible()
    {
        _isVisible = false;
    }
}
