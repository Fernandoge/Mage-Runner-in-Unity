using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpell : MonoBehaviour
{
    public LayerMask noDestroyLayers;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (((1 << collision.gameObject.layer) & noDestroyLayers) == 0)
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
