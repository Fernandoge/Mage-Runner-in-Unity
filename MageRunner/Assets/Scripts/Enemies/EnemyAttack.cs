using System;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    public Rigidbody2D rigBody;
    public float speed;
    public LayerMask noDestroyLayers;

    [System.NonSerialized]
    public bool preparingReflect;

    private void Update()
    {
        if (preparingReflect)
        {
            transform.Translate(Vector2.right * Time.deltaTime / 4);
        }
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
            GameManager.Instance.level.ResetLevel();
        }
    }

    private void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}
