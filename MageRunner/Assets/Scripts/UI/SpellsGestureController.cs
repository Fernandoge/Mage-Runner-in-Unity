using UnityEngine;

namespace MageRunner.UI
{
    public class SpellsGestureController : MonoBehaviour
    {
        [SerializeField] private AnimatorOverrideController gestureAnimator;
    
        [SerializeField] private AnimationClip fireball;
        [SerializeField] private AnimationClip ice;

        public void SwitchAnimation(AnimationClip animationClip)
        {
            gestureAnimator["Base Hand"] = animationClip;
        }
    }
}
