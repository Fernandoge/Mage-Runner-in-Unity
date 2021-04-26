using System.Collections;
using UnityEngine;

namespace MageRunner.Pickups
{
    public abstract class Pickup : MonoBehaviour
    {
        protected abstract void PickObject();
    

        private void OnTriggerEnter2D(Collider2D col)
        {
            if (col.gameObject.layer != LayerMask.NameToLayer("Player"))
                return;
        
            PickObject();
        }

        protected IEnumerator MoveToTarget(Transform target, float speed)
        {
            while (Vector2.Distance(transform.position, target.position) > 0.4f)
            {
                transform.position = Vector2.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
                yield return null;    
            }
        
            FinishAnimation();
        }

        protected virtual void FinishAnimation()
        {
            Destroy(gameObject);
        }
    }
}