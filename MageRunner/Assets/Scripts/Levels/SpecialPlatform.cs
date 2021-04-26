using MageRunner.Enemies;
using UnityEngine;

namespace MageRunner.Levels
{
    public class SpecialPlatform : MonoBehaviour
    {
        [SerializeField] private PlatformerEnemyController _platformerEnemy;
    
        private void OnCollisionEnter2D(Collision2D col)
        {
            if (_platformerEnemy == null || col.gameObject.layer != LayerMask.NameToLayer("Player"))
                return;
        
            foreach (ContactPoint2D point in col.contacts)
            {
                if (point.normal.y <= -0.9f)
                    _platformerEnemy.PlatformInteract();
            }
        }
    }
}
