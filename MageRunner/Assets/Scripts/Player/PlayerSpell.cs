using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpell : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer != LayerMask.NameToLayer("EnemyAttack"))
        {
            Destroy(gameObject);
        }

        if (collision.CompareTag("Enemy"))
        {
            //TODO: Change this when enemy health is implemented
            Destroy(collision.gameObject);
        }
    }
}
