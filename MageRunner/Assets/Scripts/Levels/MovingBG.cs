using MageRunner.Managers.GameManager;
using UnityEngine;

namespace MageRunner.Levels
{
    public class MovingBG : MonoBehaviour
    {
        public float speed;
    
        private void Start() => GameManager.Instance.level.movingBgs.Add(this);
    
        protected virtual void Update()
        {
            if (GameManager.Instance.level.isMoving == false)
                return;
        
            transform.Translate(Vector2.left * speed * Time.deltaTime);
        }
    }
}