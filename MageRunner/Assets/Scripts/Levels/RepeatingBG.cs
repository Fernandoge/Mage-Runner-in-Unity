using System;
using UnityEngine;

namespace MageRunner.Levels
{
    public class RepeatingBG : MovingBG
    {
        public float startX;
        public float endX;

        [NonSerialized] public Vector3 originalLocalPosition;
        [NonSerialized] public float originalStartX;
        [NonSerialized] public float originalEndX;

        protected override void Start()
        {
            base.Start();
            originalStartX = startX;
            originalEndX = endX;
            originalLocalPosition = transform.localPosition;
        }
        
        protected override void Update()
        {
            base.Update();
        
            if (transform.localPosition.x <= endX)
            {
                float diff = transform.localPosition.x - endX;
                transform.localPosition = new Vector2(startX + diff, transform.localPosition.y);
            }
        }

        public void ResetOriginalValues()
        {
            startX = originalStartX;
            endX = originalEndX;
            transform.localPosition = originalLocalPosition;
        }
    }
}
