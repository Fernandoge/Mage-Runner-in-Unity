using MageRunner.Combat;
using MageRunner.Managers.GameManager;
using UnityEngine;

namespace MageRunner.Enemies
{
    public class EnemyAttack : MonoBehaviour
    {
        [SerializeField] private int attackDamage;
        public Rigidbody2D rigBody;
    
        private void OnTriggerEnter2D(Collider2D collider)
        {
            int collisionLayer = collider.gameObject.layer;

            // Player Block
            if (collisionLayer == LayerMask.NameToLayer("Player"))
            {
                if (GameManager.Instance.player.stateHandler.isBlocking)
                    Destroy(gameObject);
                else
                    DamagePlayer();
            }
        }

        private void DamagePlayer()
        {
            HealthController playerHealth = GameManager.Instance.player.healthController;
            playerHealth.UpdateHealthpoints(-attackDamage);
        }
    }
}
