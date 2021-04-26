using UnityEngine;

namespace MageRunner.Levels
{
    public class RepeatingBG : MovingBG
    {
        public float startX;
        public float endX;

        protected override void Update()
        {
            base.Update();
        
            if (transform.localPosition.x <= endX)
            {
                float diff = transform.localPosition.x - endX;
                transform.localPosition = new Vector2(startX + diff, transform.localPosition.y);
            }
        }
    }
}
