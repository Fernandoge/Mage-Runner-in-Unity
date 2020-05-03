using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer != LayerMask.NameToLayer("PlayerSpell"))
        {
            Destroy(gameObject);
        }
        
        if (collision.CompareTag("Player"))
        {
            //TODO: Change this when player health is implemented
            LevelController currentLevel = FindObjectOfType<LevelController>();
            currentLevel.ResetLevel();
        }
    }
}
